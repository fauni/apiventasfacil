using BusinessLogic.Services;
using Core.DTOs;
using Core.DTOs.SalesOrder;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace BusinessLogic.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly IParameterService _parameterService;
        private readonly ISapSessionService _sapSessionService;
        private readonly IConnectionStringService _connectionStringService;
        private SapConnectionSettings _settings;

        public SalesOrderRepository(
            IParameterService parameterService,
            ISapSessionService sapSessionService,
            IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _sapSessionService = sapSessionService;
            _connectionStringService = connectionStringService;
        }

        private async Task<string> GetConnectionStringAsync()
        {
            var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");
            return _connectionStringService.BuildSqlServerConnectionString(
                parameters.ServerDatabase,
                parameters.Database,
                parameters.UserDatabase,
                parameters.PasswordDatabase);
        }

        public async Task<SalesOrderSearchResponse> SearchSalesOrdersAsync(SalesOrderSearchRequest request)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();

                // Consulta para el encabezado de órdenes con filtros dinámicos
                var whereClause = new StringBuilder("WHERE 1=1");
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    whereClause.Append(" AND (T0.DocNum LIKE @SearchTerm OR T0.CardCode LIKE @SearchTerm OR T0.CardName LIKE @SearchTerm)");
                    parameters.Add("SearchTerm", $"%{request.SearchTerm}%");
                }

                if (request.DateFrom.HasValue)
                {
                    whereClause.Append(" AND T0.TaxDate >= @DateFrom");
                    parameters.Add("DateFrom", request.DateFrom.Value);
                }

                if (request.DateTo.HasValue)
                {
                    whereClause.Append(" AND T0.TaxDate <= @DateTo");
                    parameters.Add("DateTo", request.DateTo.Value);
                }

                if (!string.IsNullOrEmpty(request.CardCode))
                {
                    whereClause.Append(" AND T0.CardCode = @CardCode");
                    parameters.Add("CardCode", request.CardCode);
                }

                if (request.SlpCode.HasValue)
                {
                    whereClause.Append(" AND T0.SlpCode = @SlpCode");
                    parameters.Add("SlpCode", request.SlpCode.Value);
                }

                if (!string.IsNullOrEmpty(request.DocStatus))
                {
                    whereClause.Append(" AND T0.DocStatus = @DocStatus");
                    parameters.Add("DocStatus", request.DocStatus);
                }

                // Consulta de conteo
                var countQuery = $@"
                    SELECT COUNT(DISTINCT T0.DocEntry)
                    FROM ORDR T0 
                    INNER JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry 
                    INNER JOIN OSLP T2 ON T0.SlpCode = T2.SlpCode 
                    INNER JOIN OCTG T3 ON T0.GroupNum = T3.GroupNum 
                    INNER JOIN NNM1 T4 ON T0.Series = T4.Series
                    {whereClause}";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var totalRecords = await connection.QuerySingleAsync<int>(countQuery, parameters);

                // Consulta principal con paginación
                var offset = (request.PageNumber - 1) * request.PageSize;
                parameters.Add("Offset", offset);
                parameters.Add("PageSize", request.PageSize);

                var mainQuery = $@"
                    SELECT DISTINCT 
                        T0.DocEntry, T0.DocNum, T0.TaxDate, T0.DocDate, T0.CardCode, T0.CardName, 
                        T0.DocType, T0.SlpCode, T2.SlpName, T0.GroupNum, T3.PymntGroup, 
                        T0.U_LB_NIT, T0.U_LB_RazonSocial, T0.DiscPrcnt, T0.VatSum, 
                        T0.DocTotal, T0.DocCur, T0.Comments, T0.DocStatus
                    FROM ORDR T0 
                    INNER JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry 
                    INNER JOIN OSLP T2 ON T0.SlpCode = T2.SlpCode 
                    INNER JOIN OCTG T3 ON T0.GroupNum = T3.GroupNum 
                    INNER JOIN NNM1 T4 ON T0.Series = T4.Series
                    {whereClause}
                    ORDER BY T0.DocEntry DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var orders = await connection.QueryAsync<SalesOrderView>(mainQuery, parameters);

                return new SalesOrderSearchResponse
                {
                    Orders = orders.ToList(),
                    TotalRecords = totalRecords,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error buscando órdenes de venta: {ex.Message}", ex);
            }
        }

        public async Task<SalesOrderView> GetSalesOrderByIdAsync(int docEntry)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Consulta para el encabezado
                var headerQuery = @"
                    SELECT T0.DocEntry, T0.DocNum, T0.TaxDate, T0.DocDate, T0.CardCode, T0.CardName, 
                           T0.DocType, T0.SlpCode, T2.SlpName, T0.GroupNum, T3.PymntGroup, 
                           T0.U_LB_NIT, T0.U_LB_RazonSocial, T0.DiscPrcnt, T0.VatSum, 
                           T0.DocTotal, T0.DocCur, T0.Comments, T0.DocStatus
                    FROM ORDR T0 
                    INNER JOIN OSLP T2 ON T0.SlpCode = T2.SlpCode 
                    INNER JOIN OCTG T3 ON T0.GroupNum = T3.GroupNum 
                    INNER JOIN NNM1 T4 ON T0.Series = T4.Series
                    WHERE T0.DocEntry = @DocEntry";

                var order = await connection.QuerySingleOrDefaultAsync<SalesOrderView>(headerQuery, new { DocEntry = docEntry });

                if (order == null)
                {
                    throw new Exception($"Orden de venta con DocEntry {docEntry} no encontrada");
                }

                // Consulta para las líneas
                var linesQuery = @"
                    SELECT T0.LineNum, T0.ItemCode, T1.ItemName, T0.U_descitemfacil, T0.Quantity, 
                           T0.PriceAfVAT, T0.Currency, T0.DiscPrcnt, T0.LineTotal, T0.GTotal,
                           T0.WhsCode, T2.WhsName, T0.UomCode, T0.LineStatus, 
                           T0.U_TFE_codUMfact, T0.U_TFE_nomUMfact
                    FROM RDR1 T0 
                    INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode 
                    INNER JOIN OWHS T2 ON T0.WhsCode = T2.WhsCode 
                    WHERE T0.DocEntry = @DocEntry
                    ORDER BY T0.LineNum";

                var lines = await connection.QueryAsync<SalesOrderLineView>(linesQuery, new { DocEntry = docEntry });
                order.Lines = lines.ToList();

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo orden de venta con DocEntry {docEntry}: {ex.Message}", ex);
            }
        }

        public async Task<List<SalesOrderView>> GetSalesOrdersByCustomerAsync(string cardCode, int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var offset = (pageNumber - 1) * pageSize;

                var query = @"
                    SELECT DISTINCT 
                        T0.DocEntry, T0.DocNum, T0.TaxDate, T0.DocDate, T0.CardCode, T0.CardName, 
                        T0.DocType, T0.SlpCode, T2.SlpName, T0.GroupNum, T3.PymntGroup, 
                        T0.U_LB_NIT, T0.U_LB_RazonSocial, T0.DiscPrcnt, T0.VatSum, 
                        T0.DocTotal, T0.DocCur, T0.Comments, T0.DocStatus
                    FROM ORDR T0 
                    INNER JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry 
                    INNER JOIN OSLP T2 ON T0.SlpCode = T2.SlpCode 
                    INNER JOIN OCTG T3 ON T0.GroupNum = T3.GroupNum 
                    INNER JOIN NNM1 T4 ON T0.Series = T4.Series
                    WHERE T0.CardCode = @CardCode
                    ORDER BY T0.DocEntry DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var orders = await connection.QueryAsync<SalesOrderView>(query, new
                {
                    CardCode = cardCode,
                    Offset = offset,
                    PageSize = pageSize
                });

                return orders.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo órdenes de venta para cliente {cardCode}: {ex.Message}", ex);
            }
        }

        public async Task<List<SalesOrderView>> GetSalesOrdersBySalesPersonAsync(int slpCode, int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                var connectionString = await GetConnectionStringAsync();
                var offset = (pageNumber - 1) * pageSize;

                var query = @"
                    SELECT DISTINCT 
                        T0.DocEntry, T0.DocNum, T0.TaxDate, T0.DocDate, T0.CardCode, T0.CardName, 
                        T0.DocType, T0.SlpCode, T2.SlpName, T0.GroupNum, T3.PymntGroup, 
                        T0.U_LB_NIT, T0.U_LB_RazonSocial, T0.DiscPrcnt, T0.VatSum, 
                        T0.DocTotal, T0.DocCur, T0.Comments, T0.DocStatus
                    FROM ORDR T0 
                    INNER JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry 
                    INNER JOIN OSLP T2 ON T0.SlpCode = T2.SlpCode 
                    INNER JOIN OCTG T3 ON T0.GroupNum = T3.GroupNum 
                    INNER JOIN NNM1 T4 ON T0.Series = T4.Series
                    WHERE T0.SlpCode = @SlpCode
                    ORDER BY T0.DocEntry DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var orders = await connection.QueryAsync<SalesOrderView>(query, new
                {
                    SlpCode = slpCode,
                    Offset = offset,
                    PageSize = pageSize
                });

                return orders.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obteniendo órdenes de venta para vendedor {slpCode}: {ex.Message}", ex);
            }
        }

        public async Task<string> CreateSalesOrderAsync(SalesOrderDto orderDto)
        {
            try
            {
                var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
                };

                var cookie = await _sapSessionService.GetSessionCookieAsync();
                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(parameters.ServiceLayerUrl)
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Cookie", cookie);

                // Obtener información adicional del cliente si no está presente
                await EnrichOrderWithCustomerData(orderDto, parameters);

                // CORREGIDO: Procesar las líneas de forma síncrona
                var documentLines = new List<object>();
                foreach (var line in orderDto.DocumentLines)
                {
                    // Obtener descripción del item si no se especifica
                    var itemDescription = !string.IsNullOrEmpty(line.U_descitemfacil)
                        ? line.U_descitemfacil
                        : await GetItemDescription(line.ItemCode, parameters);

                    documentLines.Add(new
                    {
                        ItemCode = line.ItemCode,
                        Quantity = line.Quantity,
                        PriceAfterVAT = line.PriceAfterVAT,
                        TaxCode = line.TaxCode ?? orderDto.DefaultTaxCode ?? "IVA",
                        DiscountPercent = line.DiscountPercent,
                        UoMEntry = line.UoMEntry,
                        // ShipDate = line.ShipDate ?? orderDto.U_fecharegistroapp,
                        WarehouseCode = line.WarehouseCode ?? orderDto.DefaultWarehouseCode,
                        U_descitemfacil = itemDescription,
                        U_PrecioVenta = line.U_PrecioVenta ?? line.PriceAfterVAT,
                        U_PrecioItemVenta = line.U_PrecioItemVenta ?? line.PriceAfterVAT,
                        U_TFE_codUMfact = line.U_TFE_codUMfact ?? "80",
                        U_TFE_nomUMfact = line.U_TFE_nomUMfact ?? "FRA"
                    });
                }

                // Crear el objeto para Service Layer con el formato correcto
                var sapOrder = new
                {
                    DocEntry = (int?)null, // Explícitamente null
                    DocDate = orderDto.DocDate,
                    DocDueDate = orderDto.DocDate, // AGREGADO: Campo que faltaba
                    CardCode = orderDto.CardCode,
                    Comments = orderDto.Comments,
                    Series = orderDto.Series,
                    SalesPersonCode = orderDto.SalesPersonCode,
                    ContactPersonCode = orderDto.ContactPersonCode,
                    PaymentGroupCode = orderDto.PaymentGroupCode,
                    DocumentLines = documentLines, // Usar la lista ya procesada
                    U_usrventafacil = orderDto.U_usrventafacil,
                    U_latitud = orderDto.U_latitud,
                    U_longitud = orderDto.U_longitud,
                    U_fecharegistroapp = orderDto.U_fecharegistroapp,
                    U_horaregistroapp = orderDto.U_horaregistroapp,
                    CardForeignName = orderDto.CardForeignName,
                    U_codigocliente = orderDto.U_codigocliente,
                    U_LB_RazonSocial = orderDto.U_LB_RazonSocial,
                    U_NIT = orderDto.U_NIT,
                    U_LB_NIT = orderDto.U_LB_NIT
                };

                var jsonSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented // Para debugging
                };

                var json = JsonConvert.SerializeObject(sapOrder, jsonSettings);

                // Log del JSON para debugging
                System.Diagnostics.Debug.WriteLine("JSON que se enviará a SAP:");
                System.Diagnostics.Debug.WriteLine(json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("Orders", content);
                var body = await response.Content.ReadAsStringAsync();

                // Log de la respuesta
                System.Diagnostics.Debug.WriteLine($"SAP Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"SAP Response Body: {body}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error SAP Service Layer: {response.StatusCode} - {body}");
                }

                // Parsear respuesta para obtener DocEntry
                var createdOrder = JsonConvert.DeserializeObject<dynamic>(body);
                var docEntry = createdOrder?.DocEntry?.ToString() ?? "Creado exitosamente";

                return $"Orden creada exitosamente. DocEntry: {docEntry}";
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error de conexión con SAP Service Layer: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear orden en SAP: {ex.Message}", ex);
            }
            finally
            {
                await _sapSessionService.LogoutAsync();
            }
        }

        /// <summary>
        /// Enriquece la orden con datos del cliente desde la base de datos
        /// </summary>
        private async Task EnrichOrderWithCustomerData(SalesOrderDto orderDto, SapConnectionSettings parameters)
        {
            try
            {
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = @"
                    SELECT CardName, CardFName, LicTradNum, GroupNum
                    FROM OCRD 
                    WHERE CardCode = @CardCode";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var customerData = await connection.QuerySingleOrDefaultAsync(query, new { CardCode = orderDto.CardCode });

                if (customerData != null)
                {
                    // Llenar campos faltantes con datos del cliente
                    if (string.IsNullOrEmpty(orderDto.U_LB_RazonSocial))
                        orderDto.U_LB_RazonSocial = customerData.CardName;

                    if (string.IsNullOrEmpty(orderDto.CardForeignName))
                        orderDto.CardForeignName = customerData.CardFName;

                    if (string.IsNullOrEmpty(orderDto.U_LB_NIT) && !string.IsNullOrEmpty(customerData.LicTradNum))
                    {
                        orderDto.U_LB_NIT = customerData.LicTradNum;
                        orderDto.U_NIT = customerData.LicTradNum;
                    }

                    if (!orderDto.PaymentGroupCode.HasValue)
                        orderDto.PaymentGroupCode = customerData.GroupNum;
                }
            }
            catch (Exception ex)
            {
                // Log pero no fallar por esto
                System.Diagnostics.Debug.WriteLine($"Warning: No se pudo obtener datos adicionales del cliente: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la descripción del item desde la base de datos
        /// </summary>
        private async Task<string> GetItemDescription(string itemCode, SapConnectionSettings parameters)
        {
            try
            {
                var connectionString = _connectionStringService.BuildSqlServerConnectionString(
                    parameters.ServerDatabase,
                    parameters.Database,
                    parameters.UserDatabase,
                    parameters.PasswordDatabase);

                var query = "SELECT ItemName FROM OITM WHERE ItemCode = @ItemCode";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var itemName = await connection.QuerySingleOrDefaultAsync<string>(query, new { ItemCode = itemCode });
                return itemName ?? itemCode; // Si no encuentra, retorna el código
            }
            catch
            {
                return itemCode; // En caso de error, retorna el código
            }
        }
    }
}

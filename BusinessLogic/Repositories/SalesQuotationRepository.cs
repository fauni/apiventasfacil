using Core.DTOs;
using Core.DTOs.SalesQuotation;
using Core.Interfaces;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Text;

namespace BusinessLogic.Repositories
{
    public class SalesQuotationRepository : ISalesQuotationRepository
    {
        private readonly IParameterService _parameterService;
        private readonly ISapSessionService _sapSessionService;
        private readonly IConnectionStringService _connectionStringService;
        private SapConnectionSettings _settings;

        public SalesQuotationRepository(IParameterService parameterService, ISapSessionService sapSessionService, IConnectionStringService connectionStringService)
        {
            _parameterService = parameterService;
            _sapSessionService = sapSessionService;
            _connectionStringService = connectionStringService;
        }

        public async Task<string> CreateSalesQuotationAsync(SalesQuotationDto quotationDto)
        {
            _settings = await _parameterService.GetParametersByGroupAsync("ireilab");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            var cookie = await _sapSessionService.GetSessionCookieAsync();
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_settings.ServiceLayerUrl)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Cookie", cookie);

            var json = JsonConvert.SerializeObject(quotationDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("Quotations", content);
            var body = await response.Content.ReadAsStringAsync();
            await _sapSessionService.LogoutAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error SAP: " + body);

            return body;
        }

        public async Task<List<SalesQuotationView>> GetSalesQuotationsAsync()
        {
            var parameters = await _parameterService.GetParametersByGroupAsync("ireilab");

            // var connectionString = $"Server=localhost;Database={db};User Id={user};Password={password};TrustServerCertificate=True;";
            var connectionString = _connectionStringService.BuildSqlServerConnectionString(parameters.ServerDatabase, parameters.Database, parameters.UserDatabase, parameters.PasswordDatabase);

            var query = @"
                    SELECT TOP 20 DocEntry, DocNum, TaxDate, DocDate, CardCode, CardName, U_LB_RazonSocial, U_NIT, 
                    Comments, SlpCode, DocTotal, U_VF_TiempoEntrega, U_VF_ValidezOferta, U_VF_FormaPago 
                    FROM OQUT ORDER BY DocEntry DESC";
            var queryLines = @"
                SELECT ItemCode, Dscription, Quantity, UomCode, PriceAfVAT, LineTotal, GTotal 
                FROM QUT1 WHERE DocEntry = @DocEntry";

            var result = new List<SalesQuotationView>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var commandHeader = new SqlCommand(query, connection);
                using var readerHeader = await commandHeader.ExecuteReaderAsync();

                while (await readerHeader.ReadAsync())
                {
                    result.Add(new SalesQuotationView
                    {
                        DocEntry = readerHeader.IsDBNull(readerHeader.GetOrdinal("DocEntry")) ? 0 : readerHeader.GetInt32(readerHeader.GetOrdinal("DocEntry")),
                        DocNum = readerHeader.IsDBNull(readerHeader.GetOrdinal("DocNum")) ? "" : readerHeader.GetInt32(readerHeader.GetOrdinal("DocNum")).ToString(),
                        DocDate = readerHeader.IsDBNull(readerHeader.GetOrdinal("DocDate")) ? DateTime.MinValue : readerHeader.GetDateTime(readerHeader.GetOrdinal("DocDate")),
                        TaxDate = readerHeader.IsDBNull(readerHeader.GetOrdinal("TaxDate")) ? DateTime.MinValue : readerHeader.GetDateTime(readerHeader.GetOrdinal("TaxDate")),
                        CardCode = readerHeader.IsDBNull(readerHeader.GetOrdinal("CardCode")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("CardCode")),
                        CardName = readerHeader.IsDBNull(readerHeader.GetOrdinal("CardName")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("CardName")),
                        U_LB_RazonSocial = readerHeader.IsDBNull(readerHeader.GetOrdinal("U_LB_RazonSocial")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("U_LB_RazonSocial")),
                        U_NIT = readerHeader.IsDBNull(readerHeader.GetOrdinal("U_NIT")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("U_NIT")),
                        Comments = readerHeader.IsDBNull(readerHeader.GetOrdinal("Comments")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("Comments")),
                        SlpCode = readerHeader.IsDBNull(readerHeader.GetOrdinal("SlpCode")) ? "" : readerHeader.GetInt32(readerHeader.GetOrdinal("SlpCode")).ToString(),
                        DocTotal = readerHeader.IsDBNull(readerHeader.GetOrdinal("DocTotal")) ? 0 : readerHeader.GetDecimal(readerHeader.GetOrdinal("DocTotal")),
                        U_VF_TiempoEntrega = readerHeader.IsDBNull(readerHeader.GetOrdinal("U_VF_TiempoEntrega")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("U_VF_TiempoEntrega")),
                        U_VF_ValidezOferta = readerHeader.IsDBNull(readerHeader.GetOrdinal("U_VF_ValidezOferta")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("U_VF_ValidezOferta")),
                        U_VF_FormaPago = readerHeader.IsDBNull(readerHeader.GetOrdinal("U_VF_FormaPago")) ? "" : readerHeader.GetString(readerHeader.GetOrdinal("U_VF_FormaPago")),
                        Lines = new List<SalesQuotationLineView>()
                    });
                }

                readerHeader.Close();

                foreach (var quotation in result)
                {
                    using var commandLine = new SqlCommand(queryLines, connection);
                    commandLine.Parameters.Add(new SqlParameter("@DocEntry", quotation.DocEntry));

                    using var readerLine = await commandLine.ExecuteReaderAsync();
                    while (await readerLine.ReadAsync())
                    {
                        quotation.Lines.Add(new SalesQuotationLineView
                        {
                            ItemCode = readerLine.IsDBNull(readerLine.GetOrdinal("ItemCode")) ? "" : readerLine.GetString(readerLine.GetOrdinal("ItemCode")),
                            Dscription = readerLine.IsDBNull(readerLine.GetOrdinal("Dscription")) ? "" : readerLine.GetString(readerLine.GetOrdinal("Dscription")),
                            Quantity = readerLine.IsDBNull(readerLine.GetOrdinal("Quantity")) ? 0 : readerLine.GetDecimal(readerLine.GetOrdinal("Quantity")),
                            UomCode = readerLine.IsDBNull(readerLine.GetOrdinal("UomCode")) ? "" : readerLine.GetString(readerLine.GetOrdinal("UomCode")),
                            PriceAfVAT = readerLine.IsDBNull(readerLine.GetOrdinal("PriceAfVAT")) ? 0 : readerLine.GetDecimal(readerLine.GetOrdinal("PriceAfVAT")),
                            LineTotal = readerLine.IsDBNull(readerLine.GetOrdinal("LineTotal")) ? 0 : readerLine.GetDecimal(readerLine.GetOrdinal("LineTotal")),
                            GTotal = readerLine.IsDBNull(readerLine.GetOrdinal("GTotal")) ? 0 : readerLine.GetDecimal(readerLine.GetOrdinal("GTotal"))
                        });
                    }
                    readerLine.Close();
                }
            }
            catch (Exception ex)
            {
                // Loguea o lanza una excepción más descriptiva
                throw new Exception("Error al consultar cotizaciones de venta: " + ex.Message, ex);
            }

            return result;
        }

        /*
        Este metodo obtiene las cotizaciones de venta desde el Service Layer de SAP. Todavia falta revisar los campos que retorna.
        */
        public async Task<List<SalesQuotationView>> GetSalesQuotationsFromServiceLayerAsync()
        {
            _settings = await _parameterService.GetParametersByGroupAsync("ireilab");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };

            var cookie = await _sapSessionService.GetSessionCookieAsync();
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_settings.ServiceLayerUrl)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("Quotations?$top=20&$orderby=DocEntry desc");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener cotizaciones desde Service Layer: " + body);

            var parsed = JsonConvert.DeserializeObject<SapQuotationResponse>(body);
            var quotations = parsed?.Value ?? new List<SalesQuotationView>();

            // 2. Por cada cotización obtenemos sus líneas
            foreach (var quotation in quotations)
            {
                var detalleResponse = await client.GetAsync($"Quotations({quotation.DocEntry})");
                var detalleBody = await detalleResponse.Content.ReadAsStringAsync();

                if (detalleResponse.IsSuccessStatusCode)
                {
                    var detalle = JsonConvert.DeserializeObject<SalesQuotationDetailDto>(detalleBody);
                    quotation.Lines = detalle?.DocumentLines ?? new List<SalesQuotationLineView>();
                } else
                {
                    quotation.Lines = new List<SalesQuotationLineView>();
                }
            }

            await _sapSessionService.LogoutAsync();
            return quotations;
        }
    }
}

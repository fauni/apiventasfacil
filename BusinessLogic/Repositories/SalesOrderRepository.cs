using Core.DTOs;
using Core.DTOs.SalesOrder;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
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
        private SapConnectionSettings _settings;

        public SalesOrderRepository(
            IParameterService parameterService,
            ISapSessionService sapSessionService)
        {
            _parameterService = parameterService;
            _sapSessionService = sapSessionService;
        }

        public async Task<SalesOrderResponseDto> CreateSalesOrderAsync(SalesOrderDto salesOrderDto)
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

            try
            {
                // Preparar el cuerpo de la solicitud para SAP Service Layer
                var sapOrderPayload = new
                {
                    DocEntry = salesOrderDto.DocEntry,
                    DocDate = salesOrderDto.DocDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                    DocDueDate = salesOrderDto.DocDueDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                    CardCode = salesOrderDto.CardCode,
                    Comments = salesOrderDto.Comments,
                    Series = salesOrderDto.Series,
                    SalesPersonCode = salesOrderDto.SalesPersonCode,
                    ContactPersonCode = salesOrderDto.ContactPersonCode,
                    PaymentGroupCode = salesOrderDto.PaymentGroupCode,
                    DocumentLines = salesOrderDto.DocumentLines.Select(line => new
                    {
                        ItemCode = line.ItemCode,
                        Quantity = line.Quantity,
                        TaxCode = line.TaxCode,
                        PriceAfterVAT = line.PriceAfterVAT,
                        DiscountPercent = line.DiscountPercent,
                        UoMEntry = line.UoMEntry,
                        ShipDate = line.ShipDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                        WarehouseCode = line.WarehouseCode,
                        U_descitemfacil = line.U_descitemfacil,
                        U_PrecioVenta = line.U_PrecioVenta,
                        U_PrecioItemVenta = line.U_PrecioItemVenta,
                        U_TFE_codUMfact = line.U_TFE_codUMfact,
                        U_TFE_nomUMfact = line.U_TFE_nomUMfact
                    }).ToList(),
                    U_usrventafacil = salesOrderDto.U_usrventafacil,
                    U_latitud = salesOrderDto.U_latitud,
                    U_longitud = salesOrderDto.U_longitud,
                    U_fecharegistroapp = salesOrderDto.U_fecharegistroapp.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                    U_horaregistroapp = salesOrderDto.U_horaregistroapp.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                    CardForeignName = salesOrderDto.CardForeignName,
                    U_codigocliente = salesOrderDto.U_codigocliente,
                    U_LB_RazonSocial = salesOrderDto.U_LB_RazonSocial,
                    U_NIT = salesOrderDto.U_NIT,
                    U_LB_NIT = salesOrderDto.U_LB_NIT
                };

                var jsonContent = JsonConvert.SerializeObject(sapOrderPayload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("Orders", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var sapResponse = JsonConvert.DeserializeObject<dynamic>(body);

                    return new SalesOrderResponseDto
                    {
                        DocEntry = (int)sapResponse.DocEntry,
                        DocNum = sapResponse.DocNum?.ToString() ?? "",
                        Message = "Orden de venta creada exitosamente",
                        Success = true
                    };
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(body);
                    var errorMessage = errorResponse?.error?.message?.value?.ToString() ?? "Error desconocido";

                    throw new Exception($"Error de SAP Service Layer: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear orden de venta en SAP: {ex.Message}", ex);
            }
            finally
            {
                await _sapSessionService.LogoutAsync();
                client?.Dispose();
            }
        }

        public async Task<List<SalesOrderViewDto>> GetSalesOrdersAsync()
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

            try
            {
                var response = await client.GetAsync("Orders?$top=5&$orderby=DocEntry desc");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Error al obtener órdenes desde Service Layer: " + body);

                var parsed = JsonConvert.DeserializeObject<SapOrdersResponse>(body);
                return parsed?.value?.Select(ConvertToSalesOrderView).ToList() ?? new List<SalesOrderViewDto>();
            }
            finally
            {
                await _sapSessionService.LogoutAsync();
                client?.Dispose();
            }
        }

        public async Task<SalesOrderViewDto?> GetSalesOrderByIdAsync(int docEntry)
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

            try
            {
                var response = await client.GetAsync($"Orders({docEntry})");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                var sapOrder = JsonConvert.DeserializeObject<dynamic>(body);
                return ConvertToSalesOrderView(sapOrder);
            }
            finally
            {
                await _sapSessionService.LogoutAsync();
                client?.Dispose();
            }
        }

        private static SalesOrderViewDto ConvertToSalesOrderView(dynamic sapOrder)
        {
            return new SalesOrderViewDto
            {
                DocEntry = (int)sapOrder.DocEntry,
                DocNum = sapOrder.DocNum?.ToString() ?? "",
                DocDate = DateTime.Parse(sapOrder.DocDate?.ToString() ?? DateTime.Now.ToString()),
                DocDueDate = DateTime.Parse(sapOrder.DocDueDate?.ToString() ?? DateTime.Now.ToString()),
                CardCode = sapOrder.CardCode?.ToString() ?? "",
                CardName = sapOrder.CardName?.ToString() ?? "",
                DocTotal = (decimal)(sapOrder.DocTotal ?? 0),
                DocStatus = sapOrder.DocumentStatus?.ToString() ?? "",
                SalesPersonName = sapOrder.SalesPersonCode?.ToString() ?? "",
                Comments = sapOrder.Comments?.ToString() ?? ""
            };
        }

        private class SapOrdersResponse
        {
            public List<dynamic> value { get; set; } = new List<dynamic>();
        }
    }
}

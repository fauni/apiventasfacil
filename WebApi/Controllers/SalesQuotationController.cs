using Core.DTOs.SalesQuotation;
using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SalesQuotationController : ControllerBase
    {
        private readonly ISalesQuotationService _service;

        public SalesQuotationController(ISalesQuotationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuotations()
        {
            var quotations = await _service.GetSalesQuotationsAsync();
            return Ok(ApiResponse<List<SalesQuotationView>>.Ok(quotations, "Lista de ofertas de venta obtenidas correctamente."));
        }

        [HttpGet("service-layer")]
        public async Task<IActionResult> GetFromServiceLayer()
        {
            var result = await _service.GetSalesQuotationsFromServiceLayerAsync();
            return Ok(ApiResponse<List<SalesQuotationView>>.Ok(result, "Cotizaciones obtenidas desde Service Layer"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuotation([FromBody] SalesQuotationDto dto)
        {
            var result = await _service.CreateSalesQuotationAsync(dto);
            return Ok(ApiResponse<string>.Ok(result, "Oferta de venta creada"));
        }
    }
}

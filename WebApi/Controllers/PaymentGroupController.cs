using BusinessLogic.Services;
using Core.DTOs.Customer;
using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentGroupController : ControllerBase
    {
        private readonly IPaymentGroupService _paymentGroupService;

        public PaymentGroupController(IPaymentGroupService paymentGroupService)
        {
            _paymentGroupService = paymentGroupService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPaymentGroup(
            [FromQuery] string searchTerm = "",
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                // Validar parametros
                if (pageSize > 50) pageSize = 50;
                if (pageSize < 1) pageSize = 20;
                if (pageNumber < 1) pageNumber = 1;
                var request = new PaymentGroupSearchRequest
                {
                    SearchTerm = searchTerm,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
                var result = await _paymentGroupService.SearchPaymentGroupAsync(request);
                return Ok(ApiResponse<PaymentGroupResponse>.Ok(result, "Condiciones de Pago obtenidos correctamente"));

            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al buscar Condiciones de Pago: {ex.Message}"));
            }
        }

        [HttpGet("{groupNum}")]
        public async Task<IActionResult> GetPaymentGroupByNum(string groupNum)
        {
            try
            {
                if (string.IsNullOrEmpty(groupNum))
                {
                    return BadRequest(ApiResponse<string>.Fail("El número de grupo es requerido."));
                }

                var paymentGroup = await _paymentGroupService.GetPaymentGroupByGroupNumAsync(groupNum);

                return Ok(ApiResponse<PaymentGroupDto>.Ok(paymentGroup, "Condición de pago obtenido correctamente"));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.Fail($"Condición de pago no encontrado: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentGroups()
        {
            try
            {
                var paymentGroups = await _paymentGroupService.GetAllPaymentGroupsAsync();
                return Ok(ApiResponse<List<PaymentGroupDto>>.Ok(paymentGroups, "Condiciones de Pago obtenidos correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener Condiciones de Pago: {ex.Message}"));
            }
        }
    }
}

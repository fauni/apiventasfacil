using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitOfMeasureController : ControllerBase
    {
        private readonly IUnitOfMeasureService _unitOfMeasureService;

        public UnitOfMeasureController(IUnitOfMeasureService unitOfMeasureService)
        {
            _unitOfMeasureService = unitOfMeasureService;
        }

        [HttpGet("item/{itemCode}")]
        public async Task<IActionResult> GetUnitOfMeasuresByItem(string itemCode)
        {
            try
            {
                if (string.IsNullOrEmpty(itemCode))
                {
                    return BadRequest(ApiResponse<string>.Fail("El código del item es requerido"));
                }

                var unitOfMeasures = await _unitOfMeasureService.GetUnitOfMeasuresByItemAsync(itemCode);
                return Ok(ApiResponse<object>.Ok(unitOfMeasures, "Unidades de medida obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener unidades de medida: {ex.Message}"));
            }
        }
    }
}

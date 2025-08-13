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
    public class TfeUnitOfMeasureController : ControllerBase
    {
        private readonly ITfeUnitOfMeasureService _tfeUnitOfMeasureService;

        public TfeUnitOfMeasureController(ITfeUnitOfMeasureService tfeUnitOfMeasureService)
        {
            _tfeUnitOfMeasureService = tfeUnitOfMeasureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTfeUnitsOfMeasure()
        {
            try
            {
                var tfeUnitsOfMeasure = await _tfeUnitOfMeasureService.GetTfeUnitsOfMeasureAsync();
                return Ok(ApiResponse<object>.Ok(tfeUnitsOfMeasure, "Unidades de medida TFE obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener unidades de medida TFE: {ex.Message}"));
            }
        }
    }
}

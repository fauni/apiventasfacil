using Core.Entities;
using Core.Interfaces;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametersController : ControllerBase
    {
        private readonly IParameterService _parameterService;
        private readonly ILogger<ParametersController> _logger;

        public ParametersController(IParameterService parameterService, ILogger<ParametersController> logger)
        {
            _parameterService = parameterService;
            _logger = logger;
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Parameter parameter)
        {
            var result = await _parameterService.CreateAsync(parameter);
            return Ok(ApiResponse<Parameter>.Ok(result, "Parametro creado correctamente"));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Obteniendo tdos los parametros");
            var result = await _parameterService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<Parameter>>.Ok(result, "Parametros obtenidos correctamente."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _parameterService.GetByIdAsync(id);
            if (result == null) 
            { 
                return NotFound(ApiResponse<Parameter>.Fail("Parametro no encontrado"));
            }
            
            return Ok(ApiResponse<Parameter>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Parameter parameter)
        {
            var result = await _parameterService.UpdateAsync(id, parameter);
            if (result == null)
            {
                return NotFound(ApiResponse<Parameter>.Fail("Parametro no encontrado"));
            }

            return Ok(ApiResponse<Parameter>.Ok(result, "Parametro actualizado correctamente."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _parameterService.DeleteAsync(id);
            if (!success) return NotFound(ApiResponse<string>.Fail("Parametro no encontrado."));
            return Ok(ApiResponse<string>.Ok("Parametro eliminado correctamente."));
        }
    }
}

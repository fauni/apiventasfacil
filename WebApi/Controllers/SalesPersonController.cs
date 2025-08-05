using Core.DTOs;
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
    public class SalesPersonController : ControllerBase
    {
        private readonly ISalesPersonService _salesPersonService;

        public SalesPersonController(ISalesPersonService salesPersonService)
        {
            _salesPersonService = salesPersonService;
        }

        /// <summary>
        /// Obtener un vendedor por su código
        /// </summary>
        /// <param name="slpCode">Código del vendedor</param>
        /// <returns>Datos del vendedor si existe y está activo</returns>
        [HttpGet("{slpCode}")]
        public async Task<IActionResult> GetSalesPersonByCode(int slpCode)
        {
            try
            {
                var salesPerson = await _salesPersonService.GetSalesPersonByCodeAsync(slpCode);
                if (salesPerson == null)
                {
                    return NotFound(ApiResponse<string>.Fail($"No se encontró un vendedor activo con código {slpCode}"));
                }
                return Ok(ApiResponse<SalesPersonDto>.Ok(salesPerson, "Vendedor obtenido correctamente."));
            } 
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.Ok(ex.Message));
            }
            catch(Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener vendedor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todos los vendedores activos
        /// </summary>
        /// <returns>Lista de vendedores activos</returns>
        [HttpGet]
        public async Task<IActionResult> GetActiveSalesPersons()
        {
            try
            {
                var salesPersons = await _salesPersonService.GetActiveSalesPersonsAsync();
                return Ok(ApiResponse<List<SalesPersonDto>>.Ok(salesPersons, "Vendedores obtenidos correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener vendedores: {ex.Message}"));
            }
        }

        /// <summary>
        /// Validar si un vendedor existe y está activo
        /// </summary>
        /// <param name="slpCode">Código del vendedor</param>
        /// <returns>True si el vendedor existe y está activo</returns>
        [HttpGet("{slpCode}/validate")]
        public async Task<IActionResult> ValidateSalesPerson(int slpCode)
        {
            try
            {
                var isValid = await _salesPersonService.ValidateSalesPersonAsync(slpCode);

                var message = isValid
                    ? "Vendedor válido y activo"
                    : "Vendedor no válido o inactivo";

                return Ok(ApiResponse<bool>.Ok(isValid, message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al validar vendedor: {ex.Message}"));
            }
        }
    }
}

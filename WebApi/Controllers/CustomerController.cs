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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Buscar clientes por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término para buscar en código, nombre o nombre completo</param>
        /// <param name="pageSize">Cantidad de registros por página (máximo 50)</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns>Lista paginada de clientes</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers(
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
                var request = new CustomerSearchRequest
                {
                    SearchTerm = searchTerm,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };

                var result = await _customerService.SearchCustomersAsync(request);
                return Ok(ApiResponse<CustomerSearchResponse>.Ok(result, "Clientes obtenidos correctamente"));
            } catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al buscar clientes: { ex.Message }"));
            }
        }

        /// <summary>
        /// Obtener cliente por código
        /// </summary>
        /// <param name="cardCode">Código del cliente</param>
        /// <returns>Datos del cliente</returns>
        [HttpGet("{cardCode}")]
        public async Task<IActionResult> GetCustomerByCode(string cardCode)
        {
            try
            {
                if (string.IsNullOrEmpty(cardCode))
                {
                    return BadRequest(ApiResponse<string>.Fail("El código del cliente es requerido"));
                }

                var customer = await _customerService.GetCustomerByCodeAsync(cardCode);

                return Ok(ApiResponse<CustomerDto>.Ok(customer, "Cliente obtenido correctamente"));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.Fail($"Cliente no encontrado: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todos los clientes (limitado a 100)
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();

                return Ok(ApiResponse<List<CustomerDto>>.Ok(customers, "Lista de clientes obtenida correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener clientes: {ex.Message}"));
            }
        }

        /// <summary>
        /// Buscar clientes con autocompletado (para dropdowns)
        /// </summary>
        /// <param name="term">Término de búsqueda</param>
        /// <returns>Lista simplificada de clientes</returns>
        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetCustomersAutocomplete([FromQuery] string term = "")
        {
            try
            {
                var request = new CustomerSearchRequest
                {
                    SearchTerm = term,
                    PageSize = 10, // Limitar para autocomplete
                    PageNumber = 1
                };

                var result = await _customerService.SearchCustomersAsync(request);

                // Simplificar respuesta para autocomplete
                var autocompleteResult = result.Customers.Select(c => new
                {
                    c.CardCode,
                    c.CardName,
                    c.CardFName,
                    DisplayText = $"{c.CardCode} - {c.CardName}"
                }).ToList();

                return Ok(ApiResponse<object>.Ok(autocompleteResult, "Clientes para autocompletado obtenidos"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error en autocompletado: {ex.Message}"));
            }
        }
    }
}

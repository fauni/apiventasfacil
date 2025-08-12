using Core.DTOs.Warehouse;
using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los almacenes activos
        /// </summary>
        /// <returns>Lista de almacenes activos</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllWarehouses()
        {
            try
            {
                var warehouses = await _warehouseService.GetAllWarehousesAsync();
                return Ok(ApiResponse<List<WarehouseDto>>.Ok(warehouses, "Almacenes obtenidos correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener almacenes");
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener almacenes: {ex.Message}"));
            }
        }

        /// <summary>
        /// Buscar almacenes con filtros y paginación
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda (código o nombre de almacén)</param>
        /// <param name="pageSize">Tamaño de página (máximo 50)</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns>Lista paginada de almacenes</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchWarehouses(
            [FromQuery] string searchTerm = "",
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                // Validar parámetros
                if (pageSize > 50) pageSize = 50;
                if (pageSize < 1) pageSize = 20;
                if (pageNumber < 1) pageNumber = 1;

                var request = new WarehouseSearchRequest
                {
                    SearchTerm = searchTerm,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };

                var result = await _warehouseService.SearchWarehousesAsync(request);
                return Ok(ApiResponse<WarehouseResponse>.Ok(result, "Almacenes obtenidos correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar almacenes");
                return BadRequest(ApiResponse<string>.Fail($"Error al buscar almacenes: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener un almacén específico por código
        /// </summary>
        /// <param name="whsCode">Código del almacén</param>
        /// <returns>Datos del almacén</returns>
        [HttpGet("{whsCode}")]
        public async Task<IActionResult> GetWarehouseByCode(string whsCode)
        {
            try
            {
                if (string.IsNullOrEmpty(whsCode))
                {
                    return BadRequest(ApiResponse<string>.Fail("El código del almacén es requerido"));
                }

                var warehouse = await _warehouseService.GetWarehouseByCodeAsync(whsCode);
                return Ok(ApiResponse<WarehouseDto>.Ok(warehouse, "Almacén obtenido correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener almacén {WhsCode}", whsCode);

                if (ex.Message.Contains("no encontrado"))
                {
                    return NotFound(ApiResponse<string>.Fail(ex.Message));
                }

                return BadRequest(ApiResponse<string>.Fail($"Error al obtener almacén: {ex.Message}"));
            }
        }
    }
}
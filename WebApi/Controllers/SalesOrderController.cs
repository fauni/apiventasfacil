using Core.DTOs.SalesOrder;
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
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderService _service;
        private readonly ILogger<SalesOrderController> _logger;

        public SalesOrderController(ISalesOrderService service, ILogger<SalesOrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Buscar órdenes de venta con filtros y paginación
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda (DocNum, CardCode, CardName)</param>
        /// <param name="dateFrom">Fecha desde</param>
        /// <param name="dateTo">Fecha hasta</param>
        /// <param name="cardCode">Código de cliente específico</param>
        /// <param name="slpCode">Código de vendedor específico</param>
        /// <param name="docStatus">Estado del documento (O=Abierto, C=Cerrado)</param>
        /// <param name="pageSize">Tamaño de página (máximo 50)</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns>Lista paginada de órdenes de venta</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchSalesOrders(
            [FromQuery] string searchTerm = "",
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string? cardCode = null,
            [FromQuery] int? slpCode = null,
            [FromQuery] string? docStatus = null,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Buscando órdenes de venta con término: {SearchTerm}", searchTerm);

                var request = new SalesOrderSearchRequest
                {
                    SearchTerm = searchTerm,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CardCode = cardCode,
                    SlpCode = slpCode,
                    DocStatus = docStatus,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };

                var result = await _service.SearchSalesOrdersAsync(request);
                return Ok(ApiResponse<SalesOrderSearchResponse>.Ok(result, "Órdenes de venta obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando órdenes de venta");
                return BadRequest(ApiResponse<object>.Fail($"Error buscando órdenes de venta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener una orden de venta específica por DocEntry
        /// </summary>
        /// <param name="docEntry">DocEntry de la orden</param>
        /// <returns>Detalle completo de la orden con sus líneas</returns>
        [HttpGet("{docEntry}")]
        public async Task<IActionResult> GetSalesOrderById(int docEntry)
        {
            try
            {
                _logger.LogInformation("Obteniendo orden de venta con DocEntry: {DocEntry}", docEntry);

                var result = await _service.GetSalesOrderByIdAsync(docEntry);
                return Ok(ApiResponse<SalesOrderView>.Ok(result, "Orden de venta obtenida correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo orden de venta con DocEntry: {DocEntry}", docEntry);
                return NotFound(ApiResponse<object>.Fail($"Orden de venta no encontrada: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener órdenes de venta por cliente
        /// </summary>
        /// <param name="cardCode">Código del cliente</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns>Lista de órdenes del cliente</returns>
        [HttpGet("customer/{cardCode}")]
        public async Task<IActionResult> GetSalesOrdersByCustomer(
            string cardCode,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Obteniendo órdenes de venta para cliente: {CardCode}", cardCode);

                var result = await _service.GetSalesOrdersByCustomerAsync(cardCode, pageSize, pageNumber);
                return Ok(ApiResponse<List<SalesOrderView>>.Ok(result, $"Órdenes de venta del cliente {cardCode} obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo órdenes de venta para cliente: {CardCode}", cardCode);
                return BadRequest(ApiResponse<object>.Fail($"Error obteniendo órdenes del cliente: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener órdenes de venta por vendedor
        /// </summary>
        /// <param name="slpCode">Código del vendedor</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns>Lista de órdenes del vendedor</returns>
        [HttpGet("salesperson/{slpCode}")]
        public async Task<IActionResult> GetSalesOrdersBySalesPerson(
            int slpCode,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Obteniendo órdenes de venta para vendedor: {SlpCode}", slpCode);

                var result = await _service.GetSalesOrdersBySalesPersonAsync(slpCode, pageSize, pageNumber);
                return Ok(ApiResponse<List<SalesOrderView>>.Ok(result, $"Órdenes de venta del vendedor {slpCode} obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo órdenes de venta para vendedor: {SlpCode}", slpCode);
                return BadRequest(ApiResponse<object>.Fail($"Error obteniendo órdenes del vendedor: {ex.Message}"));
            }
        }

        /// <summary>
        /// Crear una nueva orden de venta
        /// </summary>
        /// <param name="orderDto">Datos de la orden a crear</param>
        /// <returns>Resultado de la creación</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderDto orderDto)
        {
            try
            {
                _logger.LogInformation("Creando nueva orden de venta para cliente: {CardCode}", orderDto.CardCode);

                var result = await _service.CreateSalesOrderAsync(orderDto);
                return Ok(ApiResponse<string>.Ok(result, "Orden de venta creada correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando orden de venta");
                return BadRequest(ApiResponse<object>.Fail($"Error creando orden de venta: {ex.Message}"));
            }
        }
    }
}

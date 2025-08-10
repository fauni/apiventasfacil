using Core.DTOs.SalesOrder;
using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly ILogger<SalesOrderController> _logger;

        public SalesOrderController(
            ISalesOrderService salesOrderService,
            ILogger<SalesOrderController> logger)
        {
            _salesOrderService = salesOrderService;
            _logger = logger;
        }

        // <summary>
        /// Crear una nueva orden de venta en SAP
        /// </summary>
        /// <param name="salesOrderDto">Datos de la orden de venta</param>
        /// <returns>Respuesta con información de la orden creada</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderDto salesOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<string>.Fail("Datos de entrada inválidos"));
                }

                _logger.LogInformation("Creando orden de venta para cliente: {CardCode}", salesOrderDto.CardCode);

                var result = await _salesOrderService.CreateSalesOrderAsync(salesOrderDto);

                return Ok(ApiResponse<SalesOrderResponseDto>.Ok(result, "Orden de venta creada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear orden de venta");
                return BadRequest(ApiResponse<string>.Fail($"Error al crear orden de venta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todas las órdenes de venta
        /// </summary>
        /// <returns>Lista de órdenes de venta</returns>
        [HttpGet]
        public async Task<IActionResult> GetSalesOrders()
        {
            try
            {
                var salesOrders = await _salesOrderService.GetSalesOrdersAsync();
                return Ok(ApiResponse<List<SalesOrderViewDto>>.Ok(salesOrders, "Órdenes de venta obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes de venta");
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener órdenes de venta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener una orden de venta por su DocEntry
        /// </summary>
        /// <param name="docEntry">DocEntry de la orden</param>
        /// <returns>Datos de la orden de venta</returns>
        [HttpGet("{docEntry}")]
        public async Task<IActionResult> GetSalesOrderById(int docEntry)
        {
            try
            {
                var salesOrder = await _salesOrderService.GetSalesOrderByIdAsync(docEntry);

                if (salesOrder == null)
                {
                    return NotFound(ApiResponse<string>.Fail($"No se encontró la orden de venta con DocEntry: {docEntry}"));
                }

                return Ok(ApiResponse<SalesOrderViewDto>.Ok(salesOrder, "Orden de venta obtenida correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener orden de venta con DocEntry: {DocEntry}", docEntry);
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener orden de venta: {ex.Message}"));
            }
        }
    }
}

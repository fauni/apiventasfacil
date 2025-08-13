using Core.DTOs.Item;
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
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        /// <summary>
        /// Buscar items por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término para buscar en código o nombre</param>
        /// <param name="pageSize">Cantidad de registros por página (máximo 50)</param>
        /// <param name="pageNumber">Número de página</param>
        /// <returns>Lista paginada de items</returns>
        /// 
        [HttpGet("search")]
        public async Task<IActionResult> SearchItems([FromQuery] string searchTerm = "", [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
        {
            try
            {
                // Validar parámetros
                if (pageSize > 50) pageSize = 50;
                if (pageSize < 1) pageSize = 20;
                if (pageNumber < 1) pageNumber = 1;

                var request = new ItemSearchRequest
                {
                    SearchTerm = searchTerm,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
                var result = await _itemService.SearchItemsAsync(request);
                return Ok(ApiResponse<ItemSearchResponse>.Ok(result, "Ítems obtenidos correctamente"));
            } catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al buscar items: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener item por código
        /// </summary>
        /// <param name="itemCode">Código del item</param>
        /// <returns>Datos del item</returns>
        [HttpGet("{itemCode}")]
        public async Task<IActionResult> GetItemByCode(string itemCode)
        {
            try
            {
                if (string.IsNullOrEmpty(itemCode))
                {
                    return BadRequest(ApiResponse<string>.Fail("El código del item es requerido"));
                }

                var item = await _itemService.GetItemByCodeAsync(itemCode);
                return Ok(ApiResponse<ItemDto>.Ok(item, "Item obtenido correctamente"));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.Fail($"Item no encontrado: {ex.Message}"));
            }
        }

        /// <summary>
        /// Autocompletado de items
        /// </summary>
        /// <param name="term">Término de búsqueda</param>
        /// <returns>Lista simplificada de items</returns>
        [HttpGet("autocomplete")]
        public async Task<IActionResult> GetItemsAutocomplete([FromQuery] string term = "")
        {
            try
            {
                var suggestions = await _itemService.GetItemsAutocompleteAsync(term);
                return Ok(ApiResponse<object>.Ok(suggestions, "Autocompletado de items obtenido"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error en autocompletado: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener stock de un item por todos los almacenes
        /// </summary>
        /// <param name="itemCode">Código del item</param>
        /// <returns>Stock del item en todos los almacenes</returns>
        [HttpGet("{itemCode}/stock-by-warehouses")]
        public async Task<IActionResult> GetItemStockByWarehouses(string itemCode)
        {
            try
            {
                if (string.IsNullOrEmpty(itemCode))
                {
                    return BadRequest(ApiResponse<string>.Fail("El código del item es requerido"));
                }

                var result = await _itemService.GetItemStockByWarehousesAsync(itemCode);
                return Ok(ApiResponse<ItemWarehouseStockResponse>.Ok(result, "Stock por almacenes obtenido correctamente"));
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("no encontrado"))
                {
                    return NotFound(ApiResponse<string>.Fail(ex.Message));
                }

                return BadRequest(ApiResponse<string>.Fail($"Error al obtener stock por almacenes: {ex.Message}"));
            }
        }
    }
}

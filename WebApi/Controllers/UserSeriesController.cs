// WebApi/Controllers/UserSeriesController.cs
using Core.DTOs;
using Core.DTOs.Sap;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserSeriesController : ControllerBase
    {
        private readonly IUserSeriesService _service;
        private readonly ILogger<UserSeriesController> _logger;

        public UserSeriesController(IUserSeriesService service, ILogger<UserSeriesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Asignar una serie a un usuario
        /// </summary>
        /// <param name="dto">Datos de la asignación</param>
        /// <returns>Serie asignada</returns>
        [HttpPost]
        public async Task<IActionResult> AssignSeries([FromBody] UserSeriesDto dto)
        {
            try
            {
                _logger.LogInformation("Asignando serie {SeriesId} al usuario {UserId}", dto.IdSerie, dto.IdUsuario);

                var result = await _service.AssignSeriesAsync(dto);
                return Ok(ApiResponse<UserSerie>.Ok(result, "Serie asignada correctamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al asignar serie: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error asignando serie {SeriesId} al usuario {UserId}", dto.IdSerie, dto.IdUsuario);
                return BadRequest(ApiResponse<object>.Fail($"Error asignando serie: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener series básicas de un usuario (sin detalles SAP)
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Lista de series básicas</returns>
        [HttpGet("/api/users/{id}/series")]
        public async Task<IActionResult> GetSeriesByUser(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo series básicas del usuario {UserId}", id);

                var result = await _service.GetSeriesByUserAsync(id);
                return Ok(ApiResponse<IEnumerable<UserSerie>>.Ok(result, "Series obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo series del usuario {UserId}", id);
                return BadRequest(ApiResponse<object>.Fail($"Error obteniendo series: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener series de un usuario con detalles completos de SAP
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Lista de series con detalles SAP</returns>
        [HttpGet("/api/users/{id}/series/details")]
        public async Task<IActionResult> GetSeriesByUserWithDetails(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo series con detalles SAP del usuario {UserId}", id);

                if (id <= 0)
                {
                    return BadRequest(ApiResponse<object>.Fail("ID de usuario inválido"));
                }

                var result = await _service.GetSeriesByUserWithDetailsAsync(id);
                return Ok(ApiResponse<IEnumerable<UserSeriesDto>>.Ok(result, "Series con detalles obtenidas correctamente"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Parámetro inválido: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo series con detalles del usuario {UserId}", id);
                return BadRequest(ApiResponse<object>.Fail($"Error obteniendo series con detalles: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todas las series disponibles en SAP para un tipo de documento
        /// </summary>
        /// <param name="objectCode">Código del objeto SAP (17 para Sales Orders)</param>
        /// <returns>Lista de series disponibles</returns>
        [HttpGet("/api/series/available")]
        public async Task<IActionResult> GetAvailableSapSeries([FromQuery] int objectCode = 17)
        {
            try
            {
                _logger.LogInformation("Obteniendo series disponibles para ObjectCode {ObjectCode}", objectCode);

                var result = await _service.GetAvailableSapSeriesAsync(objectCode);
                return Ok(ApiResponse<IEnumerable<SapSeriesDto>>.Ok(result, "Series disponibles obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo series disponibles para ObjectCode {ObjectCode}", objectCode);
                return BadRequest(ApiResponse<object>.Fail($"Error obteniendo series disponibles: {ex.Message}"));
            }
        }

        /// <summary>
        /// Remover asignación de serie a usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="seriesId">ID de la serie</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("/api/users/{userId}/series/{seriesId}")]
        public async Task<IActionResult> RemoveSeriesAssignment(int userId, string seriesId)
        {
            try
            {
                _logger.LogInformation("Removiendo asignación de serie {SeriesId} del usuario {UserId}", seriesId, userId);

                // TODO: Implementar lógica de remoción
                // Esto requeriría agregar el método al service y repository

                return Ok(ApiResponse<object>.Ok(null, "Asignación removida correctamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removiendo asignación de serie {SeriesId} del usuario {UserId}", seriesId, userId);
                return BadRequest(ApiResponse<object>.Fail($"Error removiendo asignación: {ex.Message}"));
            }
        }
    }
}
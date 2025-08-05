using Core.DTOs.TermConditions;
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
    public class TermsConditionsController : ControllerBase
    {
        private readonly ITermsConditionsService _termsConditionsService;

        public TermsConditionsController(ITermsConditionsService termsConditionsService)
        {
            _termsConditionsService = termsConditionsService;
        }

        /// <summary>
        /// Obtener todas las formas de pago disponibles
        /// </summary>
        /// <returns>Lista de formas de pago</returns>
        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            try
            {
                var paymentMethods = await _termsConditionsService.GetPaymentMethodsAsync();
                return Ok(ApiResponse<object>.Ok(paymentMethods, "Formas de pago obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener formas de pago: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todos los tiempos de entrega disponibles
        /// </summary>
        /// <returns>Lista de tiempos de entrega</returns>
        [HttpGet("delivery-times")]
        public async Task<IActionResult> GetDeliveryTimes()
        {
            try
            {
                var deliveryTimes = await _termsConditionsService.GetDeliveryTimesAsync();
                return Ok(ApiResponse<object>.Ok(deliveryTimes, "Tiempos de entrega obtenidos correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener tiempos de entrega: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todas las validez de ofertas disponibles
        /// </summary>
        /// <returns>Lista de validez de ofertas</returns>
        [HttpGet("offer-validities")]
        public async Task<IActionResult> GetOfferValidities()
        {
            try
            {
                var offerValidities = await _termsConditionsService.GetOfferValiditiesAsync();
                return Ok(ApiResponse<object>.Ok(offerValidities, "Validez de ofertas obtenidas correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener validez de ofertas: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtener todos los términos y condiciones en una sola llamada
        /// </summary>
        /// <returns>Objeto con todos los términos y condiciones</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTermsConditions()
        {
            try
            {
                var termsConditions = await _termsConditionsService.GetAllTermsConditionsAsync();
                return Ok(ApiResponse<TermsConditionsDto>.Ok(termsConditions, "Términos y condiciones obtenidos correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail($"Error al obtener términos y condiciones: {ex.Message}"));
            }
        }
    }
}

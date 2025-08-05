using Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IPdfReportService _pdfReportService;

        public ReportsController(IPdfReportService pdfReportService)
        {
            _pdfReportService = pdfReportService;
        }

        [HttpGet("quotation/{docEntry}/pdf")]
        public async Task<IActionResult> GenerateQuotationPdf(int docEntry)
        {
            try
            {
                var pdfBytes = await _pdfReportService.GenerateQuotationPdfAsync(docEntry);

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"Cotizacion_{docEntry}.pdf"
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error generando PDF: {ex.Message}" });
            }
        }

        [HttpGet("quotation/{docEntry}/pdf/base64")]
        public async Task<IActionResult> GenerateQuotationPdfBase64(int docEntry)
        {
            try
            {
                var pdfBytes = await _pdfReportService.GenerateQuotationPdfAsync(docEntry);
                var base64String = Convert.ToBase64String(pdfBytes);

                return Ok(new
                {
                    data = base64String,
                    fileName = $"Cotizacion_{docEntry}.pdf",
                    contentType = "application/pdf"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error generando PDF: {ex.Message}" });
            }
        }
    }
}

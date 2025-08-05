using Core.Interfaces.Services;
using Core.Interfaces.Repositories;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using System.IO;

namespace BusinessLogic.Services
{
    public class PdfReportService: IPdfReportService
    {
        private readonly ISalesQuotationRepository _quotationRepository;

        public PdfReportService(ISalesQuotationRepository quotationRepository)
        {
            _quotationRepository = quotationRepository;
        }

        public async Task<byte[]> GenerateQuotationPdfAsync(int docEntry)
        {
            // Obtener datos de la cotización
            var quotations = await _quotationRepository.GetSalesQuotationsAsync();
            var quotation = quotations.FirstOrDefault(q => q.DocEntry == docEntry);

            if (quotation == null)
            {
                throw new Exception($"Cotización con DocEntry {docEntry} no encontrada");
            }

            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Configurar fuentes
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Header
            BuildHeader(document, quotation, boldFont);

            // Información del cliente
            BuildCustomerSection(document, quotation, boldFont, regularFont);

            // Información del documento
            BuildDocumentSection(document, quotation, boldFont, regularFont);

            // Términos y condiciones
            BuildTermsSection(document, quotation, boldFont, regularFont);

            // Líneas de productos
            BuildProductsSection(document, quotation, boldFont, regularFont);

            // Total
            BuildTotalSection(document, quotation, boldFont);

            // Footer
            BuildFooter(document, regularFont);

            document.Close();

            return memoryStream.ToArray();
        }

        private void BuildHeader(Document document, dynamic quotation, PdfFont boldFont)
        {
            var headerTable = new Table(2);
            headerTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Información de la cotización
            var quotationInfo = new Paragraph()
                .Add(new Text("COTIZACIÓN").SetFont(boldFont).SetFontSize(24).SetFontColor(ColorConstants.WHITE))
                .Add("\n")
                .Add(new Text($"Número: {quotation.DocNum}").SetFont(boldFont).SetFontSize(14).SetFontColor(ColorConstants.WHITE))
                .Add("\n")
                .Add(new Text($"Doc Entry: {quotation.DocEntry}").SetFontSize(12).SetFontColor(ColorConstants.WHITE));

            // Total
            var totalInfo = new Paragraph($"Bs. {quotation.DocTotal:N2}")
                .SetFont(boldFont)
                .SetFontSize(20)
                .SetFontColor(ColorConstants.WHITE)
                .SetTextAlignment(TextAlignment.RIGHT);

            var headerCell1 = new Cell().Add(quotationInfo)
                .SetBackgroundColor(ColorConstants.BLUE)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetPadding(20);

            var headerCell2 = new Cell().Add(totalInfo)
                .SetBackgroundColor(ColorConstants.BLUE)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetPadding(20);

            headerTable.AddCell(headerCell1);
            headerTable.AddCell(headerCell2);

            document.Add(headerTable);
            document.Add(new Paragraph("\n"));
        }

        private void BuildCustomerSection(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            document.Add(new Paragraph("INFORMACIÓN DEL CLIENTE")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10));

            var customerTable = new Table(2);
            customerTable.SetWidth(UnitValue.CreatePercentValue(100));

            AddInfoRow(customerTable, "Código:", quotation.CardCode?.ToString() ?? "", regularFont, boldFont);
            AddInfoRow(customerTable, "Nombre:", quotation.CardName?.ToString() ?? "", regularFont, boldFont);

            if (!string.IsNullOrEmpty(quotation.U_LB_RazonSocial?.ToString()))
                AddInfoRow(customerTable, "Razón Social:", quotation.U_LB_RazonSocial.ToString(), regularFont, boldFont);

            if (!string.IsNullOrEmpty(quotation.U_NIT?.ToString()))
                AddInfoRow(customerTable, "NIT:", quotation.U_NIT.ToString(), regularFont, boldFont);

            if (!string.IsNullOrEmpty(quotation.SlpName?.ToString()))
                AddInfoRow(customerTable, "Vendedor:", $"{quotation.SlpName} ({quotation.SlpCode})", regularFont, boldFont);

            document.Add(customerTable);
            document.Add(new Paragraph("\n"));
        }

        private void BuildDocumentSection(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            document.Add(new Paragraph("INFORMACIÓN DEL DOCUMENTO")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10));

            var docTable = new Table(2);
            docTable.SetWidth(UnitValue.CreatePercentValue(100));

            var docDate = DateTime.Parse(quotation.DocDate.ToString()).ToString("dd/MM/yyyy");
            var taxDate = DateTime.Parse(quotation.TaxDate.ToString()).ToString("dd/MM/yyyy");

            AddInfoRow(docTable, "Fecha Documento:", docDate, regularFont, boldFont);
            AddInfoRow(docTable, "Fecha Impuesto:", taxDate, regularFont, boldFont);

            if (!string.IsNullOrEmpty(quotation.Comments?.ToString()))
                AddInfoRow(docTable, "Comentarios:", quotation.Comments.ToString(), regularFont, boldFont);

            document.Add(docTable);
            document.Add(new Paragraph("\n"));
        }

        private void BuildTermsSection(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            bool hasTerms = !string.IsNullOrEmpty(quotation.U_VF_TiempoEntregaName?.ToString()) ||
                           !string.IsNullOrEmpty(quotation.U_VF_ValidezOfertaName?.ToString()) ||
                           !string.IsNullOrEmpty(quotation.U_VF_FormaPagoName?.ToString());

            if (!hasTerms) return;

            document.Add(new Paragraph("TÉRMINOS Y CONDICIONES")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10));

            var termsTable = new Table(2);
            termsTable.SetWidth(UnitValue.CreatePercentValue(100));

            if (!string.IsNullOrEmpty(quotation.U_VF_TiempoEntregaName?.ToString()))
                AddInfoRow(termsTable, "Tiempo de Entrega:",
                    $"{quotation.U_VF_TiempoEntregaName} (Código: {quotation.U_VF_TiempoEntrega})",
                    regularFont, boldFont);

            if (!string.IsNullOrEmpty(quotation.U_VF_ValidezOfertaName?.ToString()))
                AddInfoRow(termsTable, "Validez de Oferta:",
                    $"{quotation.U_VF_ValidezOfertaName} (Código: {quotation.U_VF_ValidezOferta})",
                    regularFont, boldFont);

            if (!string.IsNullOrEmpty(quotation.U_VF_FormaPagoName?.ToString()))
                AddInfoRow(termsTable, "Forma de Pago:",
                    $"{quotation.U_VF_FormaPagoName} (Código: {quotation.U_VF_FormaPago})",
                    regularFont, boldFont);

            document.Add(termsTable);
            document.Add(new Paragraph("\n"));
        }

        private void BuildProductsSection(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            document.Add(new Paragraph("PRODUCTOS")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetMarginBottom(10));

            var productsTable = new Table(6);
            productsTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Headers
            AddTableHeader(productsTable, "#", boldFont);
            AddTableHeader(productsTable, "Producto", boldFont);
            AddTableHeader(productsTable, "Cantidad", boldFont);
            AddTableHeader(productsTable, "UOM", boldFont);
            AddTableHeader(productsTable, "Precio", boldFont);
            AddTableHeader(productsTable, "Total", boldFont);

            // Data rows
            int index = 1;
            foreach (var line in quotation.Lines)
            {
                AddTableCell(productsTable, index.ToString(), regularFont);
                AddTableCell(productsTable, $"{line.ItemCode}\n{line.Dscription}", regularFont);
                AddTableCell(productsTable, line.Quantity.ToString("N2"), regularFont);
                AddTableCell(productsTable, line.UomCode?.ToString() ?? "", regularFont);
                AddTableCell(productsTable, $"Bs. {line.PriceAfVAT:N2}", regularFont);
                AddTableCell(productsTable, $"Bs. {line.GTotal:N2}", regularFont);
                index++;
            }

            document.Add(productsTable);
            document.Add(new Paragraph("\n"));
        }

        private void BuildTotalSection(Document document, dynamic quotation, PdfFont boldFont)
        {
            var totalTable = new Table(2);
            totalTable.SetWidth(UnitValue.CreatePercentValue(100));

            var totalLabel = new Cell().Add(new Paragraph("TOTAL GENERAL:")
                .SetFont(boldFont)
                .SetFontSize(16))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(15);

            var totalValue = new Cell().Add(new Paragraph($"Bs. {quotation.DocTotal:N2}")
                .SetFont(boldFont)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.RIGHT))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(15);

            totalTable.AddCell(totalLabel);
            totalTable.AddCell(totalValue);

            document.Add(totalTable);
        }

        private void BuildFooter(Document document, PdfFont regularFont)
        {
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Generado desde SAP Sales App")
                .SetFont(regularFont)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(regularFont)
                .SetFontSize(10)
                .SetFontColor(ColorConstants.GRAY)
                .SetTextAlignment(TextAlignment.CENTER));
        }

        // Helper methods
        private void AddInfoRow(Table table, string label, string value, PdfFont regularFont, PdfFont boldFont)
        {
            var labelCell = new Cell().Add(new Paragraph(label).SetFont(boldFont).SetFontSize(10));
            var valueCell = new Cell().Add(new Paragraph(value).SetFont(regularFont).SetFontSize(10));

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private void AddTableHeader(Table table, string text, PdfFont boldFont)
        {
            var cell = new Cell().Add(new Paragraph(text)
                .SetFont(boldFont)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetPadding(8);

            table.AddCell(cell);
        }

        private void AddTableCell(Table table, string text, PdfFont regularFont)
        {
            var cell = new Cell().Add(new Paragraph(text)
                .SetFont(regularFont)
                .SetFontSize(9))
                .SetPadding(6);

            table.AddCell(cell);
        }
    }
}

using Core.Interfaces.Services;
using Core.Interfaces.Repositories;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.IO.Image;
using iText.Layout.Borders;
using System.IO;

namespace BusinessLogic.Services
{
    public class PdfReportService : IPdfReportService
    {
        private readonly ISalesQuotationRepository _quotationRepository;

        public PdfReportService(ISalesQuotationRepository quotationRepository)
        {
            _quotationRepository = quotationRepository;
        }

        public async Task<byte[]> GenerateQuotationPdfAsync(int docEntry)
        {
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

            // Header con logo y número de cotización (formato EIREILAB)
            BuildEireilabHeader(document, quotation, boldFont, regularFont);

            BuildTitleHeader(document, quotation, boldFont, regularFont);

            // Información del cliente (formato EIREILAB)
            BuildEireilabCustomerInfo(document, quotation, boldFont, regularFont);

            // Tabla de productos (formato EIREILAB)
            BuildEireilabProductsTable(document, quotation, boldFont, regularFont);

            // Total y términos (formato EIREILAB)
            BuildEireilabTotalAndTerms(document, quotation, boldFont, regularFont);

            // Footer con contactos (formato EIREILAB)
            BuildEireilabFooter(document, regularFont);

            document.Close();
            return memoryStream.ToArray();
        }

        private void BuildEireilabHeader(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            // Header con logo y datos principales
            var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 40f, 40f, 30f }));
            headerTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Celda del logo EIREILAB
            var logoCell = CreateEireilabLogoCell()
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPaddingLeft(0);
            headerTable.AddCell(logoCell);

            // Espacio intermedio
            headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));

            // Celda derecha con datos de la cotización (cuadro amarillo)
            var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 100f }));
            infoTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Número de cotización con fondo amarillo
            var numeroCell = new Cell()
                .Add(new Paragraph()
                    .Add(new Text("N° "))
                    .Add(new Text($"CTZ-{quotation.DocNum ?? "001"}/2024"))
                    .SetFont(boldFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.LEFT))
                .SetBackgroundColor(ColorConstants.WHITE)
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1))
                .SetPadding(3);
            infoTable.AddCell(numeroCell);

            // Sucursal
            var sucursalCell = new Cell()
                .Add(new Paragraph()
                    .Add(new Text("SUCURSAL: ").SetFont(boldFont).SetFontSize(8))
                    .Add(new Text("LA PAZ").SetFont(regularFont).SetFontSize(8)))
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1))
                .SetPadding(3);
            infoTable.AddCell(sucursalCell);

            // Fecha
            var fechaCell = new Cell()
                .Add(new Paragraph()
                    .Add(new Text("FECHA: ").SetFont(boldFont).SetFontSize(8))
                    .Add(new Text(DateTime.Now.ToString("dd/MM/yyyy")).SetFont(regularFont).SetFontSize(8)))
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 1))
                .SetPadding(3);
            infoTable.AddCell(fechaCell);

            var rightCell = new Cell().Add(infoTable).SetBorder(Border.NO_BORDER);
            headerTable.AddCell(rightCell);

            document.Add(headerTable);
        }

        private void BuildTitleHeader(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            // Header con logo y datos principales
            var titleTable = new Table(UnitValue.CreatePercentArray(new float[] { 100f }));
            titleTable.SetWidth(UnitValue.CreatePercentValue(100));


            // Celda central con "COTIZACIÓN"
            var cotizacionCell = new Cell()
                .Add(new Paragraph("COTIZACIÓN")
                    .SetFont(boldFont)
                    .SetFontSize(20)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(10))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            titleTable.AddCell(cotizacionCell);

            document.Add(titleTable);
        }


        private Cell CreateEireilabLogoCell()
        {
            try
            {
                var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "ireilab.jpg");

                if (File.Exists(logoPath))
                {
                    var logoData = ImageDataFactory.Create(logoPath);
                    var logo = new Image(logoData);
                    logo.SetWidth(120);
                    logo.SetHeight(30);
                    logo.SetAutoScale(true);

                    return new Cell()
                        .Add(logo)
                        .SetBorder(Border.NO_BORDER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetTextAlignment(TextAlignment.LEFT);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando logo: {ex.Message}");
            }

            // Fallback: texto EIREILAB
            return new Cell()
                .Add(new Paragraph("EIREILAB S.R.L.")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12)
                    .Add("\nEQUIPOS, REACTIVOS E INSUMOS DE LABORATORIO"))
                .SetBorder(Border.NO_BORDER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
        }

        private void BuildEireilabCustomerInfo(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            // Información del cliente
            var clientTable = new Table(UnitValue.CreatePercentArray(new float[] { 35f, 65f }));
            clientTable
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetBorder(Border.NO_BORDER)
                .SetMarginTop(0)
                .SetMarginBottom(0);

            // Nombre de la institución
            AddClientInfoRow(clientTable, "NOMBRE DE LA INSTITUCIÓN:",
                quotation.CardName?.ToString() ?? "N/A", boldFont, regularFont);

            // Razón Social
            AddClientInfoRow(clientTable, "RAZÓN SOCIAL:",
                quotation.U_LB_RazonSocial?.ToString() ?? quotation.CardName?.ToString() ?? "N/A", boldFont, regularFont);

            // NIT
            AddClientInfoRow(clientTable, "NIT:",
                quotation.U_NIT?.ToString() ?? "N/A", boldFont, regularFont);

            // Teléfono (si existe)
            AddClientInfoRow(clientTable, "TELÉFONO / CELULAR",
                "N/A", boldFont, regularFont);

            // Email (si existe)
            AddClientInfoRow(clientTable, "E-mail:",
                "N/A", boldFont, regularFont);

            document.Add(clientTable);
            document.Add(new Paragraph("\n").SetMarginTop(5));
        }

        private void AddClientInfoRow(Table table, string label, string value, PdfFont boldFont, PdfFont regularFont)
        {
            var labelCell = new Cell()
                .Add(new Paragraph(label).SetFont(boldFont).SetFontSize(9))
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 0.5f))
                .SetPaddingTop(1)    // Reducido padding superior
                .SetPaddingBottom(1) // Reducido padding inferior
                .SetPaddingLeft(4)   // Mantenido padding izquierdo
                .SetPaddingRight(0)  // Eliminado padding derecho
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

            var valueCell = new Cell()
                .Add(new Paragraph(value).SetFont(regularFont).SetFontSize(9))
                .SetBorder(new SolidBorder(ColorConstants.WHITE, 0.5f))
                .SetPaddingTop(1)    // Reducido padding superior
                .SetPaddingBottom(1) // Reducido padding inferior
                .SetPaddingLeft(0)   // Eliminado padding izquierdo
                .SetPaddingRight(4)  // Mantenido padding derecho
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private void BuildEireilabProductsTable(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            // Tabla de productos exacta al formato EIREILAB
            var productsTable = new Table(UnitValue.CreatePercentArray(new float[] { 6f, 10f, 30f, 8f, 6f, 8f, 10f }));
            productsTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Headers con fondo gris
            var grayColor = new DeviceRgb(220, 220, 220);

            AddProductHeader(productsTable, "ÍTEM", boldFont, grayColor);
            AddProductHeader(productsTable, "COD.\nSIMEC", boldFont, grayColor);
            AddProductHeader(productsTable, "DESCRIPCIÓN", boldFont, grayColor);
            AddProductHeader(productsTable, "UNID.\nMEDIDA", boldFont, grayColor);
            AddProductHeader(productsTable, "CANT.", boldFont, grayColor);
            AddProductHeader(productsTable, "PRECIO\nUNITARIO", boldFont, grayColor);
            AddProductHeader(productsTable, "PRECIO\nTOTAL", boldFont, grayColor);

            // Filas de productos
            int index = 1;
            foreach (var line in quotation.Lines)
            {
                AddProductCell(productsTable, index.ToString(), regularFont);
                AddProductCell(productsTable, line.ItemCode?.ToString() ?? "N/A", regularFont);
                AddProductCell(productsTable, line.Dscription?.ToString() ?? "N/A", regularFont);
                AddProductCell(productsTable, line.UomCode?.ToString() ?? "N/A", regularFont);
                AddProductCell(productsTable, line.Quantity?.ToString("N0") ?? "0", regularFont);
                AddProductCell(productsTable, $"Bs {line.PriceAfVAT:N2}", regularFont, HorizontalAlignment.RIGHT);
                AddProductCell(productsTable, $"Bs {line.GTotal:N2}", regularFont, HorizontalAlignment.RIGHT);
                index++;
            }

            // Fila de TOTAL
            var totalRow = new Cell(1, 6)
                .Add(new Paragraph("TOTAL").SetFont(boldFont).SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                .SetPadding(5)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            productsTable.AddCell(totalRow);

            var totalValueCell = new Cell()
                .Add(new Paragraph($"Bs {quotation.DocTotal:N2}").SetFont(boldFont).SetFontSize(10))
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                .SetPadding(5)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            productsTable.AddCell(totalValueCell);

            var totalText = ConvertNumberToWords(quotation.DocTotal ?? 0);
            var totalNumberValueCell = new Cell(1,8)
                .Add(new Paragraph($"Son: {totalText}").SetTextAlignment(TextAlignment.RIGHT))
                .SetFont(boldFont)
                .SetFontSize(9)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                .SetPadding(0)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetHorizontalAlignment(HorizontalAlignment.LEFT);

            productsTable.AddCell(totalNumberValueCell);

            document.Add(productsTable);

            document.Add(new Paragraph("\n").SetMarginTop(3));
        }

        private void AddProductHeader(Table table, string text, PdfFont boldFont, Color backgroundColor)
        {
            var cell = new Cell()
                .Add(new Paragraph(text)
                    .SetFont(boldFont)
                    .SetFontSize(8)
                    .SetTextAlignment(TextAlignment.CENTER))
                .SetBackgroundColor(backgroundColor)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                .SetPadding(3)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            table.AddCell(cell);
        }

        private void AddProductCell(Table table, string text, PdfFont regularFont, HorizontalAlignment horizontalAlignment = HorizontalAlignment.CENTER)
        {
            var cell = new Cell()
                .Add(new Paragraph(text)
                    .SetFont(regularFont)
                    .SetFontSize(8)
                    .SetTextAlignment((TextAlignment?)horizontalAlignment))
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f))
                .SetPadding(3)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetHorizontalAlignment(horizontalAlignment);
            table.AddCell(cell);
        }

        private void BuildEireilabTotalAndTerms(Document document, dynamic quotation, PdfFont boldFont, PdfFont regularFont)
        {
            // Términos y condiciones en tabla
            var termsTable = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 25f, 25f, 25f }));
            termsTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Headers de términos
            AddTermHeader(termsTable, "Tiempo de entrega:", boldFont);
            AddTermHeader(termsTable, "Validez de la oferta:", boldFont);
            AddTermHeader(termsTable, "Forma de pago:", boldFont);
            AddTermHeader(termsTable, "OBSERVACIÓN", boldFont);

            // Valores de términos
            AddTermValue(termsTable, GetTermValue(quotation.U_VF_TiempoEntregaName, "INMEDIATO"), regularFont);
            AddTermValue(termsTable, GetTermValue(quotation.U_VF_ValidezOfertaName, "30 DIAS"), regularFont);
            AddTermValue(termsTable, GetTermValue(quotation.U_VF_FormaPagoName, "CREDITO"), regularFont);
            AddTermValue(termsTable, quotation.Comments?.ToString() ?? "", regularFont);

            document.Add(termsTable);

            // Mensaje estándar
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("SI USTED TIENE ALGUNA DUDA DE LA COTIZACIÓN, PÓNGASE EN CONTACTO CON NOSOTROS. ESPEREMOS SEA DE SU INTERÉS NUESTRA COTIZACIÓN.")
                .SetFont(regularFont)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(10)
                .SetMarginBottom(15));

            document.Add(new Paragraph("\n").SetMarginTop(5));

            // Firma
            document.Add(new Paragraph("_".PadRight(50, '_'))
                .SetFont(regularFont)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph(quotation.SlpName?.ToString() ?? "REPRESENTANTE DE VENTAS")
                .SetFont(boldFont)
                .SetFontSize(9)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(0));
        }

        private void AddTermHeader(Table table, string text, PdfFont boldFont)
        {
            var cell = new Cell()
                .Add(new Paragraph(text).SetFont(boldFont).SetFontSize(8))
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                .SetPadding(4)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            table.AddCell(cell);
        }

        private void AddTermValue(Table table, string text, PdfFont regularFont)
        {
            var cell = new Cell()
                .Add(new Paragraph(text).SetFont(regularFont).SetFontSize(8))
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                .SetPadding(4)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            table.AddCell(cell);
        }

        private string GetTermValue(object termName, string defaultValue)
        {
            return !string.IsNullOrEmpty(termName?.ToString()) ? termName.ToString() : defaultValue;
        }

        private void BuildEireilabFooter(Document document, PdfFont regularFont)
        {
            document.Add(new Paragraph("\n").SetMarginTop(15));

            // Footer con contactos múltiples (formato EIREILAB)
            var footerText = new Paragraph()
                .Add(new Text("Telf. 42266569 – 71743513\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Telf. 2228098 – 72005456\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Telf. 3302594 – 76754727\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Telf. 71232547 - 72879119\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Telf. 62850153 - 62850152").SetFont(regularFont).SetFontSize(6))
                .Add("\n")
                .Add(new Text("Calle: Héroes del Boqueron\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Calle: Francisco Miranda\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Avenida: Suarez Arana\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Calle: 1ro de mayo #31 /\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Calle: 15 de abril entre").SetFont(regularFont).SetFontSize(6))
                .Add("\n")
                .Add(new Text("#1476 / Zona Muyurina\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("#2198 / Zona Miraflores\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("esq. Calle Mandiore # 280\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Zona Central\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Junin y O´Connor #625").SetFont(regularFont).SetFontSize(6))
                .Add("\n")
                .Add(new Text("Cochabamba – Bolivia\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("La Paz - Bolivia\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Santa Cruz – Bolivia\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Sucre – Bolivia\t").SetFont(regularFont).SetFontSize(6))
                .Add(new Text("Tarija - Bolivia").SetFont(regularFont).SetFontSize(6))
                .Add("\n")
                .Add(new Text("E-mail: gatnacional@ireilab.com.bo").SetFont(regularFont).SetFontSize(6))
                .SetTextAlignment(TextAlignment.CENTER);

            document.Add(footerText);
        }

        private string ConvertNumberToWords(decimal number)
        {
            if (number == 0)
                return "CERO 00/100 Bolivianos";

            var integerPart = (int)Math.Floor(number);
            var decimalPart = (int)Math.Round((number - integerPart) * 100);

            string result = ConvertIntegerToWords(integerPart);

            // Manejo de singular/plural
            result += integerPart == 1 ? " BOLIVIANO" : " BOLIVIANOS";
            result += $" {decimalPart:00}/100";

            return result;
        }

        private string ConvertIntegerToWords(int number)
        {
            if (number == 0)
                return "CERO";

            if (number < 0)
                return "MENOS " + ConvertIntegerToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000000) + " MILLÓN" + (number / 1000000 > 1 ? "ES " : " ");
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000) + " MIL ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                int hundreds = number / 100;
                if (hundreds == 1 && number % 100 == 0)
                {
                    words += "CIEN";
                }
                else if (hundreds == 1)
                {
                    words += "CIENTO ";
                }
                else if (hundreds == 5)
                {
                    words += "QUINIENTOS ";
                }
                else if (hundreds == 7)
                {
                    words += "SETECIENTOS ";
                }
                else if (hundreds == 9)
                {
                    words += "NOVECIENTOS ";
                }
                else
                {
                    words += ConvertIntegerToWords(hundreds) + " CIENTOS ";
                }
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";

                var unitsMap = new[] { "CERO", "UN", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE", "DIEZ",
                              "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
                var tensMap = new[] { "CERO", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    if (number == 21)
                        words += "VEINTIÚN";
                    else if (number >= 22 && number <= 29)
                        words += "VEINTI" + unitsMap[number % 10];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += " Y " + unitsMap[number % 10];
                    }
                }
            }

            return words.Trim();
        }
    }
}
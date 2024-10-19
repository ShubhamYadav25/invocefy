using invocefy.Shared;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;

public class Invocefy
{
    public static void Main()
    {
        PdfDocument document = IntializeDocument();

        PdfPage page = CreatePageInDocument(document);

        // Get the graphics to draw on page
        XGraphics gfx = XGraphics.FromPdfPage(page);

        DrawLogo(page, 7, gfx);

        DrawCompanyInfo(gfx, page);

        DrawOrderNumber(gfx, page, "210");

        var (subTotal, xPricePos, yPos) = DrawItemInfo(gfx, page);

        var (totalWithTax, ySectionEndPos) = DrawTaxInfo(gfx, subTotal, xPricePos, yPos);

        var ySectionEndedPos = DrawTipAndFinalTotal(gfx, totalWithTax, ySectionEndPos);

        DrawRecipientName(gfx, ySectionEndedPos);

        DrawFooter(gfx);  

        // Save the document
        document.Save("C:\\Users\\sy783\\Downloads\\invocefy.pdf");

    }

    /**
     * Helper methods
     */
    public static PdfDocument IntializeDocument()
    {
        // Create a new PDF document
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Invocefy";
        return document;
    }

    public static PdfPage CreatePageInDocument(PdfDocument document)
    {
        // Create a custom-sized page (2.28" x 6")
        PdfPage page = document.AddPage();
        page.Width = XUnit.FromInch(3.15);  // 2.28 inches width
        page.Height = XUnit.FromInch(8);    // 6 inches height
        return page;
    }

    public static void DrawLogo(PdfPage page, int reduceFactor, XGraphics gfx)
    {
        // Load the logo image (replace with your actual image path)
        XImage logo = XImage.FromFile("C:\\Users\\sy783\\source\\repos\\invocefy\\invocefy\\Assets\\logo.jpg");

        // reducing Pixel instead of Point, so that reducefactor work well
        int newHeight = logo.PixelHeight / reduceFactor;
        int newWidth = logo.PixelWidth / reduceFactor;

        // TODO : do not use hardcoded logic
        int xPos = 100;
        int yPos = 17;

        // Draw the logo at the calculated position
        gfx.DrawImage(logo, xPos, yPos, newWidth, newHeight);
    }

    public static void DrawCompanyInfo(XGraphics gfx, PdfPage page)
    {
        // Set up content for the invoice
        gfx.DrawString("Bear Bar & Restro", Constant.xFontRegular, XBrushes.Black, new XRect(0, 47, page.Width, 20), XStringFormats.TopCenter);
        gfx.DrawString("200 Park Inn", Constant.xFontRegular, XBrushes.Black, new XRect(0, 55, page.Width, 20), XStringFormats.TopCenter);
        gfx.DrawString("Delhi 272207, India", Constant.xFontRegular, XBrushes.Black, new XRect(0, 65, page.Width, 20), XStringFormats.TopCenter);
        gfx.DrawString("+91 7887036667", Constant.xFontRegular, XBrushes.Black, new XRect(0, 75, page.Width, 20), XStringFormats.TopCenter);
    }

    public static void DrawOrderNumber(XGraphics gfx, PdfPage page, string orderNumber)
    {
        // Set up content for the invoice
        gfx.DrawString("Order: " + orderNumber, Constant.xFontBold, XBrushes.Black, new XRect(0, 90, page.Width, 20), XStringFormats.TopCenter);

    }

    public static (decimal subTotal, double xPricePos, double ySectionEndPos) DrawItemInfo(XGraphics gfx, PdfPage page)
    {
        decimal subTotal = 0;
        double xPos = 25;
        double yPos = 120;

        double rowHeight = 15;
        double colWidthInLast = 150;
        double colWidthInFistTwo = 28;

        // y pos for next section in draw
        double ySectionEndPos = 0;

        // Draw columns 
        gfx.DrawString("Qty", Constant.xFontRegular10, XBrushes.Black, new XPoint(xPos, yPos));
        gfx.DrawString("Item", Constant.xFontRegular10, XBrushes.Black, new XPoint(xPos + colWidthInFistTwo + 5, yPos));
        gfx.DrawString("Price", Constant.xFontRegular10, XBrushes.Black, new XPoint(xPos + colWidthInLast, yPos));

        string[,] data = new string[,]
        {
            {"2", "Bouillabaisse", "10" },
            {"2", "Quiche Lorraine", "10" },
            {"3", "Bourguignon", "10" }
        };

        for (int i = 0; i < data.GetLength(0); i++)
        {
            // count subtotal
            if (decimal.TryParse(data[i, 2], out decimal value))
            {
                subTotal += value;
            }

            double yCurrentPos = yPos + (i + 1) * rowHeight;
            ySectionEndPos = yCurrentPos;
            gfx.DrawString(data[i, 0], Constant.xFontRegular09, XBrushes.Black, new XPoint(xPos + 5, yCurrentPos));
            gfx.DrawString(data[i, 1], Constant.xFontRegular09, XBrushes.Black, new XPoint(xPos + colWidthInFistTwo + 3, yCurrentPos));
            gfx.DrawString(data[i, 2], Constant.xFontRegular09, XBrushes.Black, new XPoint(xPos + colWidthInLast + 5, yCurrentPos));
        }

        return (subTotal, (xPos + colWidthInLast) ,ySectionEndPos);
    }

    public static (decimal totalWithTax, double ySectionEndPos) DrawTaxInfo(XGraphics gfx, decimal subTotal, double xPricePos, double yPos)
    {
        double xMarginInBetween = 12;
        double xMarginInPrice = 8;
        xPricePos = (xPricePos - xMarginInPrice);

        // tax calculator
        decimal tax = 12;

        gfx.DrawString("SubTotal", Constant.xFontRegular10, XBrushes.Black, new XPoint(25, yPos + 40));
        gfx.DrawString("Tax", Constant.xFontRegular10, XBrushes.Black, new XPoint(25, yPos + 40 + xMarginInBetween));
        gfx.DrawString("Transaction", Constant.xFontRegular10, XBrushes.Black, new XPoint(25, yPos + 40 + 2 * xMarginInBetween));
        gfx.DrawString("Payment", Constant.xFontRegular10, XBrushes.Black, new XPoint(25, yPos + 40 + 3* xMarginInBetween));

        gfx.DrawString("Rs "+subTotal.ToString(), Constant.xFontRegular10, XBrushes.Black, new XPoint(xPricePos, yPos + 40));
        gfx.DrawString("Rs "+(tax.ToString()), Constant.xFontRegular10, XBrushes.Black, new XPoint(xPricePos, yPos + 40 + xMarginInBetween));
        gfx.DrawString("CASH", Constant.xFontRegular10, XBrushes.Black, new XPoint(xPricePos, yPos + 40 + 2 * xMarginInBetween));
        gfx.DrawString("Done", Constant.xFontRegular10, XBrushes.Black, new XPoint(xPricePos, yPos + 40 + 3 * xMarginInBetween));

        return (subTotal + tax, yPos + 40 + 3 * xMarginInBetween);
    }

    public static double DrawTipAndFinalTotal(XGraphics gfx, decimal totalWithTax, double yLastEndedPos)
    {
        double yMarginInBetween = 15;

        decimal tips = 17;
        decimal FinalTotal = totalWithTax + tips;

        gfx.DrawString("Tip:   ...........", Constant.xFontRegular10, XBrushes.Black, new XPoint(90, yLastEndedPos + 40));
        gfx.DrawString("Total: ...........", Constant.xFontBold10, XBrushes.Black, new XPoint(90, yLastEndedPos + 44 + yMarginInBetween));

        gfx.DrawString("Rs " + tips.ToString(), Constant.xFontRegular10, XBrushes.Black, new XPoint(140, yLastEndedPos + 37));
        gfx.DrawString("Rs "+ FinalTotal.ToString(), Constant.xFontBold10, XBrushes.Black, new XPoint(140, yLastEndedPos + 40 + yMarginInBetween));

        return (yLastEndedPos + 40 + yMarginInBetween);
    }

    public static void DrawRecipientName(XGraphics gfx, double yLastEndedPos)
    {
        double yMarginInBetween = 15;

        gfx.DrawString("Date: ", Constant.xFontRegular09, XBrushes.Black, new XPoint(25, yLastEndedPos + 40));
        gfx.DrawString("Recipient: ", Constant.xFontRegular09, XBrushes.Black, new XPoint(25, yLastEndedPos + 40 + yMarginInBetween));

        gfx.DrawString("19/10/2024", Constant.xFontRegular09, XBrushes.Black, new XPoint(90, yLastEndedPos + 40));
        gfx.DrawString("Shubham Yadav", Constant.xFontRegular09, XBrushes.Black, new XPoint(90, yLastEndedPos + 40 + yMarginInBetween));

        // TODO: Return last ended yPos

    }

    public static void DrawFooter(XGraphics gfx)
    {
        gfx.DrawString("CUSTOMER COPY", Constant.xFontRegular09, XBrushes.Black, new XPoint(70,  500));
        gfx.DrawString("THANKS FOR VISITING", Constant.xFontItalic09, XBrushes.Black, new XPoint(50, 515));

        gfx.DrawString("Book on:  https://bear-bar.com/book", Constant.xFontItalic08, XBrushes.Black, new XPoint(25, 530));

        gfx.DrawString("Delhi-Mumbai-Bangalore", Constant.xFontRegular08, XBrushes.Black, new XPoint(50, 545));
    }

}

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

        DrawItemInfo(gfx, page);

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
        gfx.DrawString("Order: "+ orderNumber, Constant.xFontBold, XBrushes.Black, new XRect(0, 90, page.Width, 20), XStringFormats.TopCenter);
        
    }

    public static void DrawItemInfo(XGraphics gfx, PdfPage page)
    {
        
    }



}

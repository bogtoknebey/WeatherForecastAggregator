using System;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;
using PdfSharp;
using TheArtOfDev;
using TheArtOfDev.HtmlRenderer;
using System.IO;
using System.Text;
using Telegram.Bot.Types.InputFiles;

namespace WeatherForecastAggregator.Bot
{
    public class PhotoConverter
    {
        public static PdfDocument ConvertHtmlToPDF(string htmlStr)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            PdfDocument pdf = PdfGenerator.GeneratePdf(htmlStr, PageSize.A4);
            return pdf;
        }


        public static void ConvertHtmlToImage(string htmlStr)
        {
            /*
            Bitmap m_Bitmap = new Bitmap(400, 600);
            PointF point = new PointF(0, 0);
            SizeF maxSize = new System.Drawing.SizeF(500, 500);

            HtmlRender.Render(Graphics.FromImage(m_Bitmap),
                                                    "<html><body><p>This is some html code</p>"
                                                    + "<p>This is another html line</p></body>",
                                                     point, maxSize);

            m_Bitmap.Save(@"C:\Test.png", ImageFormat.Png);
            */
        }
    }
}

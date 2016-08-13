using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class PdfHelper
    {
        public static void AddBackgroundImage(Stream inputPdfStream, Stream inputImageStream, Stream outputPdfStream)
        {
            //var memoryStream = new MemoryStream();
            var reader = new PdfReader(inputPdfStream);
            Image image = Image.GetInstance(inputImageStream);
            image.SetAbsolutePosition(0, 0);
            image.Alignment = Image.UNDERLYING;
            var stamper = new PdfStamper(reader, outputPdfStream);
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                var size = reader.GetPageSizeWithRotation(i);
                image.ScaleAbsolute(size);
                var pdfContentByte = stamper.GetUnderContent(i);
                pdfContentByte.AddImage(image);
            }
            //memoryStream.Position = 0;
            //memoryStream.CopyTo(outputPdfStream);
            //outputPdfStream.Position = 0;
            stamper.Close();
        }
    }
}

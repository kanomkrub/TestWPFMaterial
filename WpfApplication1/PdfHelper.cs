using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        //public static void AddBackgroundPdf(Stream inputPdfStream, Stream inputPdfBackGroundStream, Stream outputPdfStream)
        //{
        //        var bgImage = ExtractImage(pdfBackground);
        //        AddBackgroundImage(inputPdfStream, new MemoryStream(bgImage), outputPdfStream);
        //    }
        //}

        public static byte[] ExtractImage(Stream inputPdfBackGroundStream)
        {
            using (var bgReader = new PdfReader(inputPdfBackGroundStream))
            {
                var pg = bgReader.GetPageN(1);
                var res = PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES)) as PdfDictionary;
                var xobj = PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT)) as PdfDictionary;
                //if (xobj == null) continue;

                var keys = xobj.Keys;
                //if (keys.Count == 0) continue;

                var obj = xobj.Get(keys.ElementAt(0));
                //if (!obj.IsIndirect()) continue;

                var tg = PdfReader.GetPdfObject(obj) as PdfDictionary;
                var type = PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE)) as PdfName;
                //if (!PdfName.IMAGE.Equals(type)) continue;

                int XrefIndex = (obj as PRIndirectReference).Number;
                var pdfStream = bgReader.GetPdfObject(XrefIndex) as PRStream;
                return PdfReader.GetStreamBytesRaw(pdfStream);
                //var jpeg = Path.Combine(dir2, string.Format("{0:0000}.jpg", i));
                //File.WriteAllBytes(jpeg, data);
            }
        }

        public static void WritePDF(Dictionary<string, string> dic, Stream output)
        {
            var culture = CultureInfo.CreateSpecificCulture("th-TH");
            //var tempFileName = Path.GetTempFileName();
            //Document doc = new Document(new Rectangle(595, 420), 0, 0, 0, 0);
            Document doc = new Document(PageSize.A5.Rotate(), 20, 20, 10, 10);
            //var output = File.OpenWrite(tempFileName);
            var writer = PdfWriter.GetInstance(doc, output);
            //var Font = BaseFont.CreateFont()
            var baseFont = BaseFont.CreateFont("angsa.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var defaultFont = new Font(baseFont, 11, Font.NORMAL);
            var boldFont = new Font(baseFont, 11, Font.BOLD);
            var headerFont = new Font(baseFont, 17, Font.BOLD);
            doc.Open();
            var taxPersonId = string.IsNullOrWhiteSpace(dic["item53"]) ? "0994000165200" : dic["item53"];
            doc.Add(new Paragraph("ใบเสร็จรับเงิน/ใบกำกับภาษี", headerFont) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph("เลขประจำตัวผู้เสียภาษีอากร " + taxPersonId, defaultFont) { Alignment = Element.ALIGN_CENTER });
            
            //ct.SetSimpleColumn()
            var phrase1 = new Phrase(85);
            phrase1.Add(new Chunk("ชื่อผู้ใช้ไฟฟ้า     ", boldFont));
            phrase1.Add(new Chunk(dic.GetValue("item28"), defaultFont));
            doc.Add(phrase1);

            doc.Add(new Paragraph());
            var phrase2 = new Phrase();
            phrase2.Add(new Chunk("สถานที่ใช้ไฟฟ้า     ", boldFont));
            phrase2.Add(new Chunk(dic.GetValue("item29"), defaultFont));
            doc.Add(phrase2);

            doc.Add(new Paragraph());
            var phrase3 = new Phrase();
            phrase3.Add(new Chunk("บัญชีแสดงสัญญา     ", boldFont));
            phrase3.Add(new Chunk(dic.GetValue("item25"), defaultFont));
            doc.Add(phrase3);

            var phrase4 = new Phrase();
            phrase4.Add(new Chunk("          รหัสเครื่องวัดฯ     ", boldFont));
            phrase4.Add(new Chunk(dic.GetValue("item15"), defaultFont));
            doc.Add(phrase4);

            doc.Add(new Paragraph());
            doc.Add(new Paragraph(180, "ชำระผ่าน " + dic.GetValue("item55").Trim(), defaultFont));
            doc.Add(new Paragraph("FICA Doc. " + dic.GetValue("item48").Trim(), defaultFont));


            PdfContentByte cb = writer.DirectContent;

            ColumnText ct = new ColumnText(cb);
            ct.SetSimpleColumn(420, 260, 500, 380, 0, Element.ALIGN_LEFT);
            ct.AddElement(new Paragraph("เลขที่ " + dic.GetValue("item4"), defaultFont));
            ct.AddElement(new Paragraph("สำนักงานใหญ่", defaultFont));
            ct.AddElement(new Paragraph("วันที่ " + DateTime.ParseExact(dic.GetValue("item26"), "yyMMdd", culture).ToString("dd MMMM yyyy", culture), defaultFont)); //.ToString("dd/MM/yyyy")));
            ct.Go();

            ColumnText ct2 = new ColumnText(cb);
            ct2.SetSimpleColumn(20, 170, 85, 250, 0, Element.ALIGN_LEFT);
            ct2.AddElement(new Paragraph("วันจดครั้งหลัง ", boldFont) { Alignment = Element.ALIGN_LEFT });
            ct2.AddElement(new Paragraph(" ", boldFont) { Alignment = Element.ALIGN_LEFT });
            ct2.AddElement(new Paragraph(DateTime.ParseExact(dic.GetValue("item32"), "ddMMyy", culture).ToString("dd/MM/yy", culture), defaultFont) { Alignment = Element.ALIGN_LEFT });
            ct2.Go();

            ColumnText ct3 = new ColumnText(cb);
            ct3.SetSimpleColumn(85, 170, 150, 250, 0, Element.ALIGN_LEFT);
            ct3.AddElement(new Paragraph("เลขที่ใบแจ้ง ", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct3.AddElement(new Paragraph(" ", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct3.AddElement(new Paragraph(dic.GetValue("item33"), defaultFont) { Alignment = Element.ALIGN_CENTER });
            ct3.Go();


            ColumnText ct4 = new ColumnText(cb);
            ct4.SetSimpleColumn(150, 170, 200, 250, 0, Element.ALIGN_RIGHT);
            ct4.AddElement(new Paragraph("หน่วย ", boldFont) { Alignment = Element.ALIGN_RIGHT });
            ct4.AddElement(new Paragraph(" ", boldFont) { Alignment = Element.ALIGN_LEFT });
            ct4.AddElement(new Paragraph(dic.GetValue("item34").TrimStart('0'), defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct4.Go();


            ColumnText ct5 = new ColumnText(cb);
            ct5.SetSimpleColumn(200, 170, 270, 250, 0, Element.ALIGN_RIGHT);
            ct5.AddElement(new Paragraph("ค่าไฟฟ้า ", boldFont) { Alignment = Element.ALIGN_RIGHT });
            ct5.AddElement(new Paragraph(" ", boldFont) { Alignment = Element.ALIGN_LEFT });
            ct5.AddElement(new Paragraph(dic.GetValue("item35").Insert(11, ".").TrimStart('0'), defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct5.Go();

            ColumnText ct6 = new ColumnText(cb);
            ct6.SetSimpleColumn(270, 170, 320, 250, 0, Element.ALIGN_RIGHT);
            ct6.AddElement(new Paragraph("VAT 7% ", boldFont) { Alignment = Element.ALIGN_RIGHT });
            ct6.AddElement(new Paragraph(" ", boldFont) { Alignment = Element.ALIGN_LEFT });
            ct6.AddElement(new Paragraph(dic.GetValue("item36").Insert(11, ".").TrimStart('0'), defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct6.Go();

            ColumnText ct7 = new ColumnText(cb);
            ct7.SetSimpleColumn(320, 170, 390, 250, 0, Element.ALIGN_RIGHT);
            ct7.AddElement(new Paragraph("จำนวนเงิน ", boldFont) { Alignment = Element.ALIGN_RIGHT });
            ct7.AddElement(new Paragraph(" ", boldFont) { Alignment = Element.ALIGN_LEFT });
            ct7.AddElement(new Paragraph(dic.GetValue("item37").Insert(11, ".").TrimStart('0'), defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct7.Go();


            ColumnText ct8 = new ColumnText(cb);
            ct8.SetSimpleColumn(410, 170, 460, 250, 0, Element.ALIGN_CENTER);
            ct8.AddElement(new Paragraph("จำนวนวัน", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct8.AddElement(new Paragraph("คิดดอกเบี้ย", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct8.AddElement(new Paragraph(int.Parse(dic.GetValue("item52")).ToString(), defaultFont) { Alignment = Element.ALIGN_CENTER });
            ct8.Go();

            ColumnText ct9 = new ColumnText(cb);
            ct9.SetSimpleColumn(470, 170, 530, 250, 0, Element.ALIGN_RIGHT);
            ct9.AddElement(new Paragraph("ดอกเบี้ย ", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct9.AddElement(new Paragraph("ผิดนัด ", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct9.AddElement(new Paragraph(double.Parse(dic.GetValue("item51").Replace(' ', '0')).ToString("0.00"), defaultFont) { Alignment = Element.ALIGN_CENTER });
            ct9.Go();

            ColumnText ct10 = new ColumnText(cb);
            ct10.SetSimpleColumn(530, 170, 580, 250, 0, Element.ALIGN_RIGHT);
            ct10.AddElement(new Paragraph("FT", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct10.AddElement(new Paragraph("(บาท/หน่วย)", boldFont) { Alignment = Element.ALIGN_CENTER });
            ct10.AddElement(new Paragraph(dic.GetValue("item44"), defaultFont) { Alignment = Element.ALIGN_CENTER });
            ct10.Go();

            ColumnText ct11 = new ColumnText(cb);
            ct10.SetSimpleColumn(400, 40, 580, 180, 0, Element.ALIGN_RIGHT);
            ct10.AddElement(new Paragraph("รวมเงิน", defaultFont) { Alignment = Element.ALIGN_LEFT });
            ct10.AddElement(new Paragraph("รวมภาษีมูลค่าเพิ่ม ", defaultFont) { Alignment = Element.ALIGN_LEFT });
            ct10.AddElement(new Paragraph("รวม ", defaultFont) { Alignment = Element.ALIGN_LEFT });
            ct10.AddElement(new Paragraph("ดอกเบี้ยผิดนัด ", defaultFont) { Alignment = Element.ALIGN_LEFT });
            ct10.AddElement(new Paragraph("รวมทั้งสิ้น ", defaultFont) { Alignment = Element.ALIGN_LEFT });
            ct10.Go();

            ColumnText ct12 = new ColumnText(cb);
            ct10.SetSimpleColumn(330, 40, 570, 180, 0, Element.ALIGN_RIGHT);
            ct10.AddElement(new Paragraph(dic.GetValue("item35").Insert(11, ".").TrimStart('0') + "  บาท", defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct10.AddElement(new Paragraph(dic.GetValue("item36").Insert(11, ".").TrimStart('0') + "  บาท", defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct10.AddElement(new Paragraph(dic.GetValue("item37").Insert(11, ".").TrimStart('0') + "  บาท", defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct10.AddElement(new Paragraph(double.Parse(dic.GetValue("item52")) + "  บาท", defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct10.AddElement(new Paragraph(dic.GetValue("item43").Insert(11, ".").TrimStart('0') + "  บาท", defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct10.AddElement(new Paragraph("(" + dic.GetValue("item45").Trim() + ")", defaultFont) { Alignment = Element.ALIGN_RIGHT });
            ct10.Go();


            //var phrase5 = new Phrase();
            //phrase5.Add(new Chunk("วันจดครั้งหลัง    เลขที่ใบแจ้ง",boldFont));
            //doc.Add(phrase5);


            doc.Close();
        }
        public static Dictionary<string, string> ExtractText(string txtall)
        {
            //var txtall = File.ReadAllText(filePath, Encoding.UTF8);
            Dictionary<string, string> listItem = new Dictionary<string, string>();
            string value;
            if (!string.IsNullOrEmpty(txtall))
            {
                value = txtall.Substring(0, 10);
                listItem.Add("item1", value);

                value = txtall.Substring(10, 8);
                listItem.Add("item2", value);


                value = txtall.Substring(18, 6);
                listItem.Add("item3", value);


                value = txtall.Substring(24, 8);
                listItem.Add("item4", value);


                value = txtall.Substring(32, 4);
                listItem.Add("item5", value);


                value = txtall.Substring(36, 4);
                listItem.Add("item6", value);


                value = txtall.Substring(40, 4);
                listItem.Add("item7", value);


                value = txtall.Substring(44, 4);
                listItem.Add("item8", value);


                value = txtall.Substring(48, 4);
                listItem.Add("item9", value);


                value = txtall.Substring(52, 4);
                listItem.Add("item10", value);


                value = txtall.Substring(56, 4);
                listItem.Add("item11", value);


                value = txtall.Substring(60, 4);
                listItem.Add("item12", value);


                value = txtall.Substring(64, 4);
                listItem.Add("item13", value);


                value = txtall.Substring(68, 1);
                listItem.Add("item14", value);


                value = txtall.Substring(69, 10);
                listItem.Add("item15", value);


                value = txtall.Substring(79, 10);
                listItem.Add("item16", value);


                value = txtall.Substring(89, 100);
                listItem.Add("item17", value);


                value = txtall.Substring(189, 100);
                listItem.Add("item18", value);


                value = txtall.Substring(289, 100);
                listItem.Add("item19", value);


                value = txtall.Substring(389, 5);
                listItem.Add("item20", value);


                value = txtall.Substring(394, 40);
                listItem.Add("item21", value);


                value = txtall.Substring(434, 80);
                listItem.Add("item22", value);


                value = txtall.Substring(514, 15);
                listItem.Add("item23", value);


                value = txtall.Substring(529, 4);
                listItem.Add("item24", value);


                value = txtall.Substring(533, 16);
                listItem.Add("item25", value);


                value = txtall.Substring(549, 6);
                listItem.Add("item26", value);


                value = txtall.Substring(555, 15);
                listItem.Add("item27", value);


                value = txtall.Substring(570, 100);
                listItem.Add("item28", value);


                value = txtall.Substring(670, 100);
                listItem.Add("item29", value);


                value = txtall.Substring(770, 100);
                listItem.Add("item30", value);


                value = txtall.Substring(870, 10);
                listItem.Add("item31", value);


                value = txtall.Substring(880, 6);
                listItem.Add("item32", value);


                value = txtall.Substring(886, 12);
                listItem.Add("item33", value);


                value = txtall.Substring(898, 9);
                listItem.Add("item34", value);


                value = txtall.Substring(907, 13);
                listItem.Add("item35", value);


                value = txtall.Substring(920, 13);
                listItem.Add("item36", value);


                value = txtall.Substring(933, 13);
                listItem.Add("item37", value);


                value = txtall.Substring(946, 13);
                listItem.Add("item38", value);


                value = txtall.Substring(959, 13);
                listItem.Add("item39", value);


                value = txtall.Substring(972, 13);
                listItem.Add("item40", value);


                value = txtall.Substring(985, 13);
                listItem.Add("item41", value);


                value = txtall.Substring(998, 13);
                listItem.Add("item42", value);


                value = txtall.Substring(1011, 13);
                listItem.Add("item43", value);


                value = txtall.Substring(1024, 7);
                listItem.Add("item44", value);


                value = txtall.Substring(1031, 150);
                listItem.Add("item45", value);


                value = txtall.Substring(1181, 100);
                listItem.Add("item46", value);


                value = txtall.Substring(1281, 16);
                listItem.Add("item47", value);


                value = txtall.Substring(1297, 12);
                listItem.Add("item48", value);


                value = txtall.Substring(1309, 2);
                listItem.Add("item49", value);


                value = txtall.Substring(1311, 1);
                listItem.Add("item50", value);


                value = txtall.Substring(1312, 13);
                listItem.Add("item51", value);


                value = txtall.Substring(1325, 3);
                listItem.Add("item52", value);


                value = txtall.Substring(1328, 13);
                listItem.Add("item53", value);


                value = txtall.Substring(1341, 5);
                listItem.Add("item54", value);


                value = txtall.Substring(1346, 35);
                listItem.Add("item55", value);


                value = txtall.Substring(1381, 1);
                listItem.Add("item56", value);
            }
            return listItem;
        }
    }
    static class ExtensionHelper
    {
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            TValue result;
            return dic.TryGetValue(key, out result) ?
                result :
                default(TValue);
        }
    }
}

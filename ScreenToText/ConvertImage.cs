using IronOcr;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Tesseract;
using System.Text.Json;

namespace ScreenToText
{
    class ConvertImage
    {
        public static void ConvertImageToTextIron()
        {
            var Ocr = new AdvancedOcr()
            {
                CleanBackgroundNoise = false,
                EnhanceContrast = true,
                EnhanceResolution = false,
                Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
                ColorSpace = AdvancedOcr.OcrColorSpace.Color,
                DetectWhiteTextOnDarkBackgrounds = false,
                InputImageType = AdvancedOcr.InputTypes.Document,
                RotateAndStraighten = false,
                ReadBarCodes = false,
                ColorDepth = 4
            };
                //Person restoredPerson = await JsonSerializer.DeserializeAsync<Person>(fs);
                //Console.WriteLine($"Name: {restoredPerson.Name}  Age: {restoredPerson.Age}");
            // Ocr.Language = IronOcr.Languages.English.OcrLanguagePack;
            Ocr.Language = new IronOcr.Languages.MultiLanguage(IronOcr.Languages.English.OcrLanguagePack, IronOcr.Languages.Russian.OcrLanguagePack);
            /*new IronOcr.Languages.MultiLanguage(IronOcr.Languages.English.OcrLanguagePack, IronOcr.Languages.Russian.OcrLanguagePack);*/
            Image img = Clipboard.GetImage();
            var results = Ocr.Read(img);
            Console.WriteLine(results.ToString());
            Clipboard.SetText(results.ToString());
        }

        public static void ConvertImageToTextVS()
        {
            Image img = Clipboard.GetImage();
            Pix resultImg = PixConverter.ToPix(Get24bppRgb(img));
            // Image img = Clipboard.GetImage();
            var Ocr = new TesseractEngine("./datasets", "eng", EngineMode.Default);
            var results = Ocr.Process(resultImg);
            Console.WriteLine(results.GetText());
            Clipboard.SetText(results.GetText());
        }

        private static Bitmap Get24bppRgb(Image image)
        {
            var bitmap = new Bitmap(image);
            var bitmap24 = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bitmap24))
            {
                gr.DrawImage(bitmap, new Rectangle(0, 0, bitmap24.Width, bitmap24.Height));
            }
            return bitmap24;
        }
    }
}

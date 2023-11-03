namespace OxyPlot.SkiaSharp.Texts
{
    using System.Diagnostics;
    using System.Globalization;

    using global::SkiaSharp;

    using HarfBuzzSharp;

    using OxyPlot.Axes;

    [TestFixture]
    public class SkPdfExporterTests
    {
        static readonly string DestinationDirectory = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "PdfExporterTests_ExampleLibrary");

        private static SKTypeface koreaTypeface;

        [OneTimeSetUp]
        public void Init()
        {
            Directory.CreateDirectory(DestinationDirectory);

            var assembly = typeof(SkPdfExporterTests).Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                Debug.WriteLine(resourceName);
            }

            using (var stream =
                   assembly.GetManifestResourceStream("OxyPlot.SkiaSharp.Texts.Resources.NotoSansKR-Regular.otf"))
            {
                if (stream != null)
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, (int)stream.Length);
                    using (var fontStream = new MemoryStream(data))
                    {
                        koreaTypeface = SKTypeface.FromStream(fontStream);
                    }
                }
            }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Export_Unicode()
        {
            var plotModel = new PlotModel { Title = "Unicode Example" };

            var categoryAxis = new CategoryAxis
                                   {
                                       Position = AxisPosition.Bottom,
                                       Key = "CategoryAxis",
                                       ItemsSource = new[] { "한국", "β", "γ", "δ" },
                                       Font =
                                           "Noto Sans KR" // Choose a font that supports the Unicode characters you want.
                                   };
            plotModel.Axes.Add(categoryAxis);

            var valueAxis = new LinearAxis
                                {
                                    Position = AxisPosition.Left,
                                    Minimum = 0,
                                    Maximum = 10,
                                    Font =
                                        "Malgun Gothic" // Choose a font that supports the Unicode characters you want.
                                };
            plotModel.Axes.Add(valueAxis);

            using (var stream = new FileStream("Unicode_oxy.pdf", FileMode.Create))
            {
                var exporter = new PdfExporter { Width = 600, Height = 400 };
                exporter.Export(plotModel, stream);
            }
        }

        [Test]
        public void DrawString_CharacterMap()
        {
            using (var stream = new FileStream("Unicode_string96.pdf", FileMode.Create))
            {
                // (in points, where 1 point equals 1/72 inch).
                var width = 600;
                var height = 400;
                using var document = SKDocument.CreatePdf(stream);
                using var pdfCanvas = document.BeginPage(width, height);

                pdfCanvas.Clear(OxyColors.Undefined.ToSKColor());
                // Draw a red rectangle
                var redPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Fill };
                pdfCanvas.DrawRect(100, 100, 200, 100, redPaint);

                // Draw some text
                var textPaint = new SKPaint
                                    {
                                        Color = SKColors.Black,
                                        IsAntialias = true,
                                        Style = SKPaintStyle.Fill,
                                        TextSize = 40,
                                        Typeface = koreaTypeface
                                    };
                pdfCanvas.DrawText("Hello, SkiaSharp! 안녕하세요", 100, 300, textPaint);

                // Draw a blue circle
                var bluePaint = new SKPaint { Color = SKColors.Blue, Style = SKPaintStyle.Stroke, StrokeWidth = 5 };
                pdfCanvas.DrawCircle(150, 500, 50, bluePaint);

                // End the current page
                document.EndPage();

                // Finalize the document
                document.Close();
            }
        }
        [Test]
        public void DrawContextForString()
        {
            using (var stream = new FileStream("Unicode_context_model0.pdf", FileMode.Create))
            {
                // (in points, where 1 point equals 1/72 inch).
                var width = 600;
                var height = 400;
                using var document = SKDocument.CreatePdf(stream);
                using var pdfCanvas = document.BeginPage(width, height);

                const float dpiScale = 72f / 300;
                var context = new SkiaRenderContext
                {
                    RenderTarget = RenderTarget.VectorGraphic,
                    SkCanvas = pdfCanvas,
                    UseTextShaping = true,
                    
                    // DpiScale = dpiScale
                };

                

                pdfCanvas.Clear(OxyColors.Undefined.ToSKColor());
                // Draw a red rectangle
                var redPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Fill };
                pdfCanvas.DrawRect(100, 150, 200, 100, redPaint);

                context.DrawRectangle(
                    new OxyRect(100, 20, 200, 100),
                    OxyColors.AliceBlue,
                    OxyColors.Black,
                    1,
                    EdgeRenderingMode.Adaptive);

                // Draw some text
                var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    TextSize = 40,
                    // Typeface = koreaTypeface
                };

                DrawMultilineText(pdfCanvas, "Hello!\r\n안녕하세요", new SKPoint(100, 300), textPaint);
                // pdfCanvas.DrawText("Hello, SkiaSharp! 안녕하세요", 100, 300, textPaint);

                // Draw a blue circle
                var bluePaint = new SKPaint { Color = SKColors.Blue, Style = SKPaintStyle.Stroke, StrokeWidth = 5 };
                pdfCanvas.DrawCircle(150, 500, 50, bluePaint);

                // End the current page
                document.EndPage();

                // Finalize the document
                document.Close();
                // model.Render(context, new OxyRect(0, 0, width / dpiScale, height / dpiScale));
            }
        }


        [Test]
        public void DrawMultilineText()
        {
            using (var stream = new FileStream("Unicode_multiline.pdf", FileMode.Create))
            {
                // (in points, where 1 point equals 1/72 inch).
                var width = 600;
                var height = 400;
                using var document = SKDocument.CreatePdf(stream);
                using var pdfCanvas = document.BeginPage(width, height);

                pdfCanvas.Clear(OxyColors.Undefined.ToSKColor());
                // Draw a red rectangle
                var redPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Fill };
                pdfCanvas.DrawRect(100, 100, 200, 100, redPaint);

                // Draw some text
                var textPaint = new SKPaint
                                    {
                                        Color = SKColors.Black,
                                        IsAntialias = true,
                                        Style = SKPaintStyle.Fill,
                                        TextSize = 40,
                                        // Typeface = koreaTypeface
                                    };

                DrawMultilineText(pdfCanvas, "Hello!\r\n안녕하세요", new SKPoint(100, 300), textPaint);
                // pdfCanvas.DrawText("Hello, SkiaSharp! 안녕하세요", 100, 300, textPaint);

                // Draw a blue circle
                var bluePaint = new SKPaint { Color = SKColors.Blue, Style = SKPaintStyle.Stroke, StrokeWidth = 5 };
                pdfCanvas.DrawCircle(150, 500, 50, bluePaint);

                // End the current page
                document.EndPage();

                // Finalize the document
                document.Close();
            }
        }

        public static void DrawMultilineText(
            SKCanvas canvas,
            string text,
            SKPoint position,
            SKPaint paint)
        {
            // Split the text into lines based on newline characters
            var lines = text.Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None);

            float lineHeight = paint.TextSize + 5; // Assuming 5 units of spacing between lines
            float y = position.Y;

            foreach (var line in lines)
            {
                canvas.DrawText(line, position.X, y, paint);
                y += lineHeight;
            }
        }
    }
}

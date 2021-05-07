using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using System;
using System.Linq;
using static System.Configuration.ConfigurationManager;

using Path = System.IO.Path;

namespace SymPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                Create(args);
            }
        }

        static void Create(string[] files)
        {
            // HACK: 表紙が本文と同じサイズなら縦にしたい
            // 前提：
            // 　1枚目は表紙で本文の2枚分のサイズ（横）
            // 　2枚目以降は本文（縦）
            // 　表2用の白紙ファイルが必要

            // config取得、チェック
            var coverPageSize = GetCoverPageSize().Rotate();
            var mainPageSize = GetMainPageSize();
            var binding = GetBinding();
            var title = GetTitle();
            var author = GetAuthor();

            // PDFファイルを作成
            var outputPath = Path.Combine(Path.GetDirectoryName(files[0]), title + ".pdf");
            using (var writer = new PdfWriter(outputPath))
            using (var pdf = new PdfDocument(writer))
            using (var doc = new Document(pdf))
            {
                // タイトル、作成者
                var info = pdf.GetDocumentInfo();
                info.SetTitle(title);
                info.SetAuthor(author);
                info.SetCreator("SymPdf");

                // HACK: 見開きの設定
                var pref = new PdfViewerPreferences();
                pref.SetDirection(binding);
                pdf.GetCatalog().SetViewerPreferences(pref);

                int cnt = 0;
                foreach (var file in files.OrderBy(x => x))
                {
                    cnt++;

                    // 表紙と本文でサイズを変える
                    var size = (cnt == 1) ? coverPageSize : mainPageSize;

                    // 用紙サイズにあわせて画像を挿入
                    var page = pdf.AddNewPage(size);
                    var image = ImageDataFactory.Create(file);
                    var canvas = new PdfCanvas(page);
                    canvas.AddImageFittedIntoRectangle(image, page.GetPageSize(), false);
                }

                doc.Close();
            }
        }
        static string GetTitle()
        {
            var title = AppSettings["Title"];

            return title;
        }
        static string GetAuthor()
        {
            var title = AppSettings["Author"];

            return title;
        }

        static PageSize GetCoverPageSize()
        {
            var pageSize = AppSettings["CoverPageSize"];

            return GetPageSize(pageSize);
        }

        static PageSize GetMainPageSize()
        {
            var pageSize = AppSettings["MainPageSize"];

            return GetPageSize(pageSize);
        }

        static PageSize GetPageSize(string pageSize)
        {
            if (pageSize == "A3") { return PageSize.A3; }
            if (pageSize == "A4") { return PageSize.A4; }
            if (pageSize == "A5") { return PageSize.A5; }
            if (pageSize == "B4") { return PageSize.B4; }
            if (pageSize == "B5") { return PageSize.B5; }

            throw new Exception("用紙サイズが想定外です。");
        }

        static PdfViewerPreferences.PdfViewerPreferencesConstants GetBinding()
        {
            var binding = AppSettings["Binding"];

            if (binding == "Right") { return PdfViewerPreferences.PdfViewerPreferencesConstants.RIGHT_TO_LEFT; }
            if (binding == "Left") { return PdfViewerPreferences.PdfViewerPreferencesConstants.LEFT_TO_RIGHT; }

            throw new Exception("閉じ方向が想定外です。");
        }
    }
}

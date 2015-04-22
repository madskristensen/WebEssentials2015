using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio.Text;
using mshtml;
using System.Reflection;

namespace MadsKristensen.EditorExtensions.Markdown
{
    internal class MarkdownMargin : CompilingMarginBase
    {
        private HTMLDocument _document;
        private WebBrowser _browser;
        private const string _stylesheet = "WE-Markdown.css";
        private double _cachedPosition = 0,
                       _cachedHeight = 0,
                       _positionPercentage = 0;

        public MarkdownMargin(ITextDocument document)
            : base(WESettings.Instance.Markdown, document)
        { }

        public static string GetStylesheet()
        {
            string file = GetCustomStylesheetFilePath();
            string folder = GetFolder();
            string Csspath = Path.Combine(folder, "markdown\\margin\\highlight.css");
            string scriptPath = Path.Combine(folder, "markdown\\margin\\highlight.js");

            string linkFormat = "<link rel=\"stylesheet\" href=\"{0}\" />";
            string link = string.Format(CultureInfo.CurrentCulture, linkFormat, Csspath);

            string scriptFormat = "<script src=\"{0}\"></script>" +
                                  "<script>hljs.initHighlightingOnLoad();</script>";

            link += string.Format(CultureInfo.CurrentCulture, scriptFormat, scriptPath);

            if (File.Exists(file))
            {
                link += string.Format(CultureInfo.CurrentCulture, linkFormat, file);
                return link;
            }

            // Mimicks GitHub's styling
            return  link + "<style>body{font: 16px/1.5 'Helvetica Neue', Helvetica, 'Segoe UI', Arial, freesans, sans-serif} h1{font-size:36px; border-bottom: 1px solid #f1f1f1} h2{font-size:28px} h3{font-size:24px} h4{font-size:20px} pre{padding:16px} img{border:none} a{color:#4183c4}</style>";
        }

        private static string GetFolder()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly);
            return folder;
        }

        public static string GetCustomStylesheetFilePath()
        {
            string folder = ProjectHelpers.GetSolutionFolderPath();

            if (string.IsNullOrEmpty(folder))
                return null;

            return Path.Combine(folder, _stylesheet);
        }

        public async static Task CreateStylesheet()
        {
            string file = Path.Combine(ProjectHelpers.GetSolutionFolderPath(), _stylesheet);

            await FileHelpers.WriteAllTextRetry(file, "body { background: yellow; }");
            ProjectHelpers.GetSolutionItemsProject().ProjectItems.AddFromFile(file);
        }

        protected override void UpdateMargin(CompilerResult result)
        {
            if (_browser == null)
                return;
            // The Markdown compiler cannot return errors
            string html = String.Format(CultureInfo.InvariantCulture, @"<!DOCTYPE html>
                                        <html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
                                            <head>
                                                <meta charset=""utf-8"" />
                                                <base href=""file:///{0}/"">
                                                <title>Markdown Preview</title>
                                                {1}
                                            </head>
                                            <body>{2}</body>
                                        </html>",
                                        Path.GetDirectoryName(Document.FilePath).Replace("\\", "/"),
                                        GetStylesheet(),
                                        result.Result);

            if (_document == null)
            {
                _browser.NavigateToString(html);

                return;
            }

            _cachedPosition = _document.documentElement.getAttribute("scrollTop");
            _cachedHeight = Math.Max(1.0, _document.body.offsetHeight);
            _positionPercentage = _cachedPosition * 100 / _cachedHeight;

            _browser.NavigateToString(html);
        }

        protected override FrameworkElement CreatePreviewControl()
        {
            _browser = new WebBrowser();
            _browser.HorizontalAlignment = HorizontalAlignment.Stretch;
            _browser.LoadCompleted += (s, e) =>
            {
                _document = _browser.Document as HTMLDocument;
                _cachedHeight = _document.body.offsetHeight;
                _document.documentElement.setAttribute("scrollTop", _positionPercentage * _cachedHeight / 100);
            };

            return _browser;
        }
    }
}
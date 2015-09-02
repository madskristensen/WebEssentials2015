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
        private const string _stylesheetFileName = "WE-Markdown.css";
        private const string _htmlTemplateFileName = "WE-Markdown.html";
        private const string _defaultHtmlTemplate = @"<!DOCTYPE html>
<html lang=""en"">
    <head>
        <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"" />
        <meta charset=""utf-8"" />
        <!-- This is to make sure your relative image links show up nicely. -->
        <base href=""file:///{0}/"">
        <title>Markdown Preview</title>
        <!-- Here is where the custom style sheet is inserted as well as highlight.js setup code. -->
        {1}
    </head>
    <body>
        <!-- This is where the rendered html from your document is placed -->
        {2}
        <hr />
        <p>To customize this template, please head to <a href=""http://vswebessentials.com/features/markdown"" target=""_blank"">VSWebEssentials</a> for more information.</p>
    </body>
</html>";
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
            return link + @"<style>
                body{padding:0 15px; font: 16px/1.5 'Helvetica Neue', Helvetica, 'Segoe UI', Arial, freesans, sans-serif}
                body > h1:first-child {margin-top:0; padding-top:0}
                h1{font-size:36px; border-bottom: 1px solid #f1f1f1}
                h2{font-size:28px; border-bottom: 1px solid #f1f1f1}
                h3{font-size:24px}
                h4{font-size:20px}
                pre{padding:10px; border-radius:2px; line-height:19px; margin: inherit 0}
                code{padding:0 0.2em; background:rgba(0,0,0,0.04); border-radius:2px}
                blockquote{padding:0 15px; margin-left:0; color:#555; border-left:4px solid #ddd}
                img{border:none; max-width:100%; height:auto}
                ul{list-style-type:disc}
                ul ul {padding-left:50px}
                a{color:#4183c4}
                table{display:block; width:100%; overflow:auto; word-break:keep-all; border-collapse:collapse; border-spacing:0}
                tr:nth-child(2n){background-color:#f8f8f8}
                th, td {padding: 6px 13px; border: 1px solid #ddd}
                th{font-weight:bold}
            </style>";
        }

        private static string GetFolder()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly);
            return folder;
        }

        public static string GetCustomStylesheetFilePath()
        {
            return GetSolutionOrGlobalFile(_stylesheetFileName, WESettings.Instance.Markdown.GlobalPreviewCSSFile);
        }

        private static string GetSolutionOrGlobalFile(string solutionFileName, string globalFilePath)
        {
            var solutionFile = GetSolutionFile(solutionFileName);

            if (null == solutionFile || !File.Exists(solutionFile))
            {
                if (!string.IsNullOrEmpty(globalFilePath))
                    return globalFilePath;
            }

            return solutionFile;
        }

        private static string GetSolutionFile(string fileName)
        {
            string folder = ProjectHelpers.GetSolutionFolderPath();

            if (string.IsNullOrEmpty(folder))
                return null;

            return Path.Combine(folder, fileName);
        }

        public async static Task CreateStylesheet()
        {
            string file = Path.Combine(ProjectHelpers.GetSolutionFolderPath(), _stylesheetFileName);

            await FileHelpers.WriteAllTextRetry(file, "body { background: yellow; }");
            ProjectHelpers.GetSolutionItemsProject().ProjectItems.AddFromFile(file);
        }

        protected override void UpdateMargin(CompilerResult result)
        {
            if (_browser == null)
                return;

            var htmlFormatString = GetHtmlTemplate();
            var baseHref = Path.GetDirectoryName(Document.FilePath).Replace("\\", "/");
            var styleSheet = GetStylesheet();

            string html;

            try
            {
                // The Markdown compiler cannot return errors
                html = string.Format(CultureInfo.InvariantCulture, htmlFormatString,
                    baseHref,
                    styleSheet,
                    result.Result);
            }
            catch (Exception exp)
            {
                html = string.Format(CultureInfo.InvariantCulture, _defaultHtmlTemplate,
                    baseHref,
                    styleSheet,
                    result.Result + CreateExceptionBox(exp));
            }

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

        private string CreateExceptionBox(Exception exp)
        {
            return $@"<hr /><h3>Custom Html Template Error</h3>
<h4>{exp.Message}</h4>
<p>Below is a template you can use to get started<p>
<pre><code>{System.Web.HttpUtility.HtmlEncode(_defaultHtmlTemplate)}</code></pre>";
        }

        private string GetHtmlTemplate()
        {
            var templateFile = GetSolutionOrGlobalFile(_htmlTemplateFileName, WESettings.Instance.Markdown.GlobalPreviewHtmlTemplate);

            if (!string.IsNullOrEmpty(templateFile))
            {
                try
                {
                    var template = File.ReadAllText(templateFile);
                    return template;
                }
                catch { }
            }


            //-- Return default
            return _defaultHtmlTemplate;
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
using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace MadsKristensen.EditorExtensions.JSON
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("json")]
    [ContentType("javascript")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class LogoProvider : IWpfTextViewCreationListener
    {
        private const double _initOpacity = 0.4D;

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        private Dictionary<string, string> _map = new Dictionary<string, string>()
        {
            { ".bowerrc", "bower.png" },
            { "bower.json", "bower.png"},
            { "package.json", "npm.png"},
            { "project.json", "vs.png"},
            { "gruntfile.js", "grunt.png"},
            { "gulpfile.js", "gulp.png"},
        };

        public void TextViewCreated(IWpfTextView textView)
        {
            ITextDocument document;
            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
            {
                string fileName = Path.GetFileName(document.FilePath).ToLowerInvariant();

                if (string.IsNullOrEmpty(fileName) || !_map.ContainsKey(fileName))
                    return;

                bool isVisible = WESettings.Instance.General.ShowLogoWatermark;

                LogoAdornment highlighter = new LogoAdornment(textView, _map[fileName], isVisible, _initOpacity);
            }
        }
    }
}

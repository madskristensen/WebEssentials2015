using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("JavaScript")]
    [Order(After = "Default")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class TaskClassifierProvider : IClassifierProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry { get; set; }

        private static readonly Regex _gulp = new Regex("(?:(gulp.task\\())(?<name>(\"|')([^\"']+)(\\2))", RegexOptions.Compiled);
        private static readonly Regex _grunt = new Regex("(?:(grunt.registerTask\\())(?<name>(\"|')([^\"']+)(\\2))", RegexOptions.Compiled);

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            string fileName = textBuffer.GetFileName();

            if (string.IsNullOrEmpty(fileName))
                return null;

            if (Path.GetFileName(fileName).Equals("gulpfile.js", StringComparison.OrdinalIgnoreCase))
            {
                return textBuffer.Properties.GetOrCreateSingletonProperty(() => new TaskClassifier(Registry, "gulp.task(", _gulp));
            }
            else if (Path.GetFileName(fileName).Equals("gruntfile.js", StringComparison.OrdinalIgnoreCase))
            {
                return textBuffer.Properties.GetOrCreateSingletonProperty(() => new TaskClassifier(Registry, "grunt.registerTask(", _grunt));
            }

            return null;
        }
    }
}
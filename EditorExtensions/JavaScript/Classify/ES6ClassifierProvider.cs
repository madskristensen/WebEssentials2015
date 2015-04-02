using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("JavaScript")]
    [Order(After = "Default")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class ES6ClassifierProvider : IClassifierProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new ES6Classifier(Registry, textBuffer));
        }
    }
}
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("JavaScript")]
    [ContentType("node.js")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class JavaScriptSortPropertiesViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import(typeof(ITextStructureNavigatorSelectorService))]
        public ITextStructureNavigatorSelectorService Navigator { get; set; }
        
        [Import]
        public ICompletionBroker CompletionBroker { get; set; }

        [Import]
        internal IClassifierAggregatorService AggregatorService;

        private static FSPCache _fspCache;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);

            textView.Properties.GetOrCreateSingletonProperty(() => new MinifySelection(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new JavaScriptFindReferences(textViewAdapter, textView, Navigator));
            textView.Properties.GetOrCreateSingletonProperty(() => new ExtractToFile(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new NodeModuleGoToDefinition(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new ReferenceTagGoToDefinition(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new CommentCompletionCommandTarget(textViewAdapter, textView, AggregatorService));
            textView.Properties.GetOrCreateSingletonProperty(() => new CommentIndentationCommandTarget(textViewAdapter, textView, AggregatorService, CompletionBroker));

            if (_fspCache == null)
                _fspCache = new FSPCache();

            _fspCache.SyncIntellisenseFiles();
        }
    }
}


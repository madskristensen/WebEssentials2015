using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Core.ContentTypes;
using Microsoft.Web.Editor;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MadsKristensen.EditorExtensions.Css
{
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType(CssContentTypeDefinition.CssContentType)]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class CssConnectionListener : IWpfTextViewConnectionListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            if (!subjectBuffers.Any(b => b.ContentType.IsOfType(CssContentTypeDefinition.CssContentType)))
                return;

            var textViewAdapter = EditorAdaptersFactoryService.GetViewAdapter(textView);
            if (textViewAdapter == null)
                return;

            textView.Properties.GetOrCreateSingletonProperty(() => new ExtractToFile(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new CssAddMissingStandard(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new CssAddMissingVendor(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new CssRemoveDuplicates(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new MinifySelection(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new CssFindReferences(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new F1Help(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new CssSelectBrowsers(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new RetriggerTarget(textViewAdapter, textView));
            textView.Properties.GetOrCreateSingletonProperty(() => new ArrowsCommandTarget(textViewAdapter, textView));

            CssSchemaUpdater.CheckForUpdates();
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
        }
    }
}

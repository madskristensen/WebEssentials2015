using System;
using System.Linq;
using System.Runtime.InteropServices;
using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Tagging;

namespace MadsKristensen.EditorExtensions
{
    class CommentCompletionCommandTarget : CommandTargetBase<VSConstants.VSStd2KCmdID>
    {
        private static readonly Type jsTaggerType = typeof(Microsoft.VisualStudio.JSLS.JavaScriptLanguageService).Assembly.GetType("Microsoft.VisualStudio.JSLS.Classification.Tagger");
        private IClassifier _classifier;
        public CommentCompletionCommandTarget(IVsTextView adapter, IWpfTextView textView, IClassifierAggregatorService classifier)
            : base(adapter, textView, VSConstants.VSStd2KCmdID.TYPECHAR)
        {
            _classifier = classifier.GetClassifier(textView.TextBuffer);
        }

        protected override bool Execute(VSConstants.VSStd2KCmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (!WESettings.Instance.JavaScript.BlockCommentCompletion)
                return false;

            char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

            if (typedChar != '*')
                return false;

            return CompleteComment();
        }

        private bool CompleteComment()
        {
            int position = TextView.Caret.Position.BufferPosition.Position;

            if (position < 1)
                return false;

            SnapshotSpan span = new SnapshotSpan(TextView.TextBuffer.CurrentSnapshot, position - 1, 1);
            bool isComment = _classifier.GetClassificationSpans(span).Any(c => c.ClassificationType.IsOfType("comment"));
            bool isString = IsString(span);

            if (isComment || isString)
                return false;

            char prevChar = TextView.TextBuffer.CurrentSnapshot.ToCharArray(position - 1, 1)[0];

            // Abort if the previous characters isn't a forward-slash
            if (prevChar != '/' || isComment)
                return false;

            // Insert the typed character
            TextView.TextBuffer.Insert(position, "*");

            using (WebEssentialsPackage.UndoContext("Comment completion"))
            {
                // Use separate undo context for this, so it can be undone separately.
                TextView.TextBuffer.Insert(position + 1, "*/");
            }

            // Move the caret to the correct point
            SnapshotPoint point = new SnapshotPoint(TextView.TextBuffer.CurrentSnapshot, position + 1);
            TextView.Caret.MoveTo(point);

            return true;
        }

        private bool IsString(SnapshotSpan span)
        {
            if (span.Start < 2)
                return false;

            var spans = _classifier.GetClassificationSpans(span);

            return spans.Any(c => c.ClassificationType.IsOfType("string"));
        }

        protected override bool IsEnabled()
        {
            return true;
        }
    }
}
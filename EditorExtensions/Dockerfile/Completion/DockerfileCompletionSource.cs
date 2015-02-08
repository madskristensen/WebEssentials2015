using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Intel = Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.Web.Editor.Imaging;

namespace MadsKristensen.EditorExtensions.Dockerfile
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("text")]
    [Name("DockerfileCompletion")]
    class DockerfileCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            string filename = System.IO.Path.GetFileName(textBuffer.GetFileName());
            var textType = DockerfileClassifierProvider.GetTextType(filename);
            if (textType == TextType.Dockerfile)
            {
                return new DockerfileCompletionSource(textBuffer);
            }
            return null;
        }
    }

    class DockerfileCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;
        private static ImageSource _glyph = GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupVariable, StandardGlyphItem.GlyphItemPublic);

        public DockerfileCompletionSource(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                return;

            List<Intel.Completion> completions = new List<Intel.Completion>();
            foreach (string item in DockerfileClassifier.Valid.OrderBy(x => x))
            {
                completions.Add(new Intel.Completion(item, item, null, _glyph, item));
            }

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            var line = triggerPoint.Value.GetContainingLine();
            string text = line.GetText();
            int index = text.IndexOf(' ');
            int hash = text.IndexOf('#');
            SnapshotPoint start = triggerPoint.Value;
            
            if (hash > -1 && hash < triggerPoint.Value.Position || (index > -1 && (start - line.Start.Position) > index))
                return;

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint.Value), SpanTrackingMode.EdgeInclusive);

            completionSets.Add(new CompletionSet("Dockerfile", "Dockerfile", applicableTo, completions, Enumerable.Empty<Intel.Completion>()));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.JSLS;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    public class ES6Classifier : IClassifier
    {
        private IClassificationType _keyword;
        private IClassificationType _identifier;
        private ITagger<ClassificationTag> _tagger;

        private static Regex _regex = new Regex(@"(?<!(var|let|const))( |\t)(?<keyword>as|from)( |\t)", RegexOptions.Compiled);
        private static readonly Type _jsTaggerType = typeof(JavaScriptLanguageService).Assembly.GetType("Microsoft.VisualStudio.JSLS.Classification.Tagger");

        public ES6Classifier(IClassificationTypeRegistryService registry, ITextBuffer buffer)
        {
            _keyword = registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword);
            _identifier = registry.GetClassificationType(PredefinedClassificationTypeNames.Identifier);
            _tagger = buffer.Properties.GetProperty<ITagger<ClassificationTag>>(_jsTaggerType);
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> list = new List<ClassificationSpan>();

            string text = span.GetText();

            foreach (Match match in _regex.Matches(text))
            {
                var name = match.Groups["keyword"];

                var matchSpan = new SnapshotSpan(span.Snapshot, span.Start + name.Index, name.Length);
                var tags = _tagger.GetTags(new NormalizedSnapshotSpanCollection(matchSpan)).Select(s => s.Tag.ClassificationType);

                if (tags.Contains(_identifier))
                    list.Add(new ClassificationSpan(matchSpan, _keyword));
            }

            return list;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add { }
            remove { }
        }
    }
}
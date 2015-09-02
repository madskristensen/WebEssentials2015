using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.Markdown
{
    [Export(typeof(IClassifierProvider))]
    [Order(After = "Microsoft.Html.Editor.Classification.HtmlClassificationProvider")]
    [ContentType(MarkdownContentTypeDefinition.MarkdownContentType)]
    public class MarkdownClassifierProvider : IClassifierProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new MarkdownClassifier(Registry));
        }
    }

    public class MarkdownClassifier : IClassifier
    {
        // The beginning of the content area of a line (after any quote blocks)
        const string lineBegin = @"(?:^|\r?\n|\r)(?:(?: {0,3}>)+ {0,3})?";

        private static readonly Regex _reBold = new Regex(@"(?<Value>(\*\*|__)[^\s](?:.*?[^\s])?\1)");
        private static readonly Regex _reItalic = new Regex(@"(?<Value>((?<!\*)\*(?!\*)|(?<!_)_(?!_))[^\s](?:.*?[^\s])?\1)");
        private static readonly Regex _reQuote = new Regex(lineBegin + @"(?<Value> {0,3}>)+( {0,3}[^\r\n]+)(?:$|\r?\n|\r)");
        private static readonly Regex _reHeader = new Regex(lineBegin + @"(?<Value>([#]{1,6})\s[^#\r\n]+(\1(?!#))?)");
        private static readonly Regex _reCode = new Regex(@"(?<Value>((?<!`)`(?!`))[^\s](?:.*?[^\s])?\1)");

        private readonly IClassificationType codeType;
        private readonly IReadOnlyCollection<Tuple<Regex, IClassificationType>> typeRegexes;

        public MarkdownClassifier(IClassificationTypeRegistryService registry)
        {
            codeType = registry.GetClassificationType(MarkdownClassificationTypes.MarkdownCode);

            typeRegexes = new[] {
                Tuple.Create(_reBold, registry.GetClassificationType(MarkdownClassificationTypes.MarkdownBold)),
                Tuple.Create(_reItalic, registry.GetClassificationType(MarkdownClassificationTypes.MarkdownItalic)),
                Tuple.Create(_reHeader, registry.GetClassificationType(MarkdownClassificationTypes.MarkdownHeader)),
                Tuple.Create(_reQuote, registry.GetClassificationType(MarkdownClassificationTypes.MarkdownQuote)),
                Tuple.Create(_reCode, registry.GetClassificationType(MarkdownClassificationTypes.MarkdownCode))
            };
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (span == null || span.IsEmpty)
                return new List<ClassificationSpan>();

            var text = span.GetText();
            var spans = typeRegexes.SelectMany(t => ClassifyMatches(span, text, t.Item1, t.Item2));

            return new List<ClassificationSpan>(spans);
        }

        private static IEnumerable<ClassificationSpan> ClassifyMatches(SnapshotSpan span, string text, Regex regex, IClassificationType type)
        {
            Match match = regex.Match(text);

            while (match.Success)
            {
                var value = match.Groups["Value"];
                var result = new SnapshotSpan(span.Snapshot, span.Start + value.Index, value.Length);
                yield return new ClassificationSpan(result, type);

                match = regex.Match(text, match.Index + match.Length);
            }
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add { }
            remove { }
        }
    }
}

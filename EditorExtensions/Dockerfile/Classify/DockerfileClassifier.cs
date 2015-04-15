using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace MadsKristensen.EditorExtensions.Dockerfile
{
    public class DockerfileClassifier : IClassifier
    {
        private IClassificationType _keyword, _comment, _string, _symbol;
        private bool _isDockerfile = false;
        private TextType _textType;
        private static readonly HashSet<string> _valid = new HashSet<string>() { "FROM", "MAINTAINER", "RUN", "CMD", "EXPOSE", "ENV", "ADD", "COPY", "ENTRYPOINT", "VOLUME", "USER", "WORKDIR", "ONBUILD" };
        public static Regex String = new Regex(@"""(?<content>[^""]+)?""?", RegexOptions.Compiled);
        public static Regex Guid = new Regex(@"{{(?<content>[^}]+)}}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static HashSet<string> Valid { get { return _valid; } }

        public DockerfileClassifier(IClassificationTypeRegistryService registry)
        {
            _keyword = registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword);
            _comment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            _string = registry.GetClassificationType(PredefinedClassificationTypeNames.String);
            _symbol = registry.GetClassificationType(DockerfileClassificationTypes.Keyword);
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> list = new List<ClassificationSpan>();
            if (!_isDockerfile)
                return list;

            string text = span.GetText();
            int index = text.IndexOf("#", StringComparison.Ordinal);

            if (index > -1)
            {
                var result = new SnapshotSpan(span.Snapshot, span.Start + index, text.Length - index);
                list.Add(new ClassificationSpan(result, _comment));
            }

            if (_textType != TextType.Dockerfile)
                return list;

            if (index == -1 || index > 0)
            {
                string[] args = text.Split(' ');

                if (args.Length >= 2 && Valid.Contains(args[0].Trim().ToUpperInvariant()))
                {
                    var result = new SnapshotSpan(span.Snapshot, span.Start, args[0].Length);
                    list.Add(new ClassificationSpan(result, _keyword));
                }
            }

            // Strings
            var matches = String.Matches(text);
            foreach (Match match in matches)
            {
                if (index == -1 || match.Index < index)
                {
                    var result = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                    list.Add(new ClassificationSpan(result, _string));
                }
            }

            // tokens
            var tokenMatches = Guid.Matches(text);
            foreach (Match match in tokenMatches)
            {
                if (index == -1 || match.Index < index)
                {
                    var result = new SnapshotSpan(span.Snapshot, span.Start + match.Index, match.Length);
                    list.Add(new ClassificationSpan(result, _symbol));
                }
            }

            return list;
        }

        public void OnClassificationChanged(SnapshotSpan span, TextType type)
        {
            _isDockerfile = true;
            _textType = type;
            var handler = this.ClassificationChanged;

            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(span));
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}
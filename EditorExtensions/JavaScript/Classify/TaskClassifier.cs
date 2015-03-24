using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    public class TaskClassifier : IClassifier
    {
        private IClassificationType _formatDefinition;
        private string _searchText;
        private Regex _task;
        private static readonly Regex _binding = new Regex("(\"|')(?<value>[^'\"\\s]+)(\\1)", RegexOptions.Compiled);
        
        public TaskClassifier(IClassificationTypeRegistryService registry, string searchText, Regex regex)
        {
            _formatDefinition = registry.GetClassificationType(TaskClassificationTypes.Name);
            _searchText = searchText;
            _task = regex;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> list = new List<ClassificationSpan>();

            string text = span.GetText();

            if (text.StartsWith("/// <binding"))
            {
                foreach (Match match in _binding.Matches(text))
                {
                    var value = match.Groups["value"];
                    var result = new SnapshotSpan(span.Snapshot, span.Start + value.Index, value.Length);
                    list.Add(new ClassificationSpan(result, _formatDefinition));
                }                
            }
            else
            {
                int index = text.IndexOf(_searchText, StringComparison.Ordinal);

                if (index == -1)
                    return list;

                foreach (Match match in _task.Matches(text))
                {
                    var name = match.Groups["name"];
                    var result = new SnapshotSpan(span.Snapshot, span.Start + name.Index, name.Length);
                    list.Add(new ClassificationSpan(result, _formatDefinition));
                }
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
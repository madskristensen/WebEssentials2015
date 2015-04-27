using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Completion;

namespace MadsKristensen.EditorExtensions.Html
{
    [HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
    [Export(typeof(IHtmlCompletionListFilter))]
    [ContentType("jsx")]
    public class JsxCompletion : IHtmlCompletionListProvider, IHtmlCompletionListFilter
    {
        public string CompletionType
        {
            get
            {
                return CompletionTypes.Attributes;
            }
        }

        private static string[] _filter = new[] { "class", "for" };

        public void FilterCompletionList(IList<HtmlCompletion> completions, HtmlCompletionContext context)
        {
            foreach (var completion in completions)
            {
                if (_filter.Contains(completion.DisplayText))
                    completion.FilterType = CompletionEntryFilterTypes.NeverVisible;

                if (completion.DisplayText.StartsWith("on", StringComparison.Ordinal) && completion.DisplayText.Length  > 2)
                {
                    char third = completion.DisplayText[2];
                    completion.DisplayText = "on" + char.ToUpperInvariant(third) + completion.DisplayText.Substring(3);

                    completion.DisplayText = completion.DisplayText.Replace("change", "Change")
                                                                   .Replace("start", "Start")
                                                                   .Replace("stop", "Stop")
                                                                   .Replace("enter", "Enter")
                                                                   .Replace("leave", "Leave")
                                                                   .Replace("over", "Over")
                                                                   .Replace("end", "End")
                                                                   .Replace("play", "Play")
                                                                   .Replace("menu", "Menu")
                                                                   .Replace("input", "Input")
                                                                   .Replace("down", "Down")
                                                                   .Replace("out", "Out")
                                                                   .Replace("press", "Press")
                                                                   .Replace("up", "Up")
                                                                   .Replace("move", "Move")
                                                                   .Replace("update", "Update")
                                                                   .Replace("wheel", "Wheel")
                                                                   .Replace("metadata", "Metadata")
                                                                   .Replace("through", "Through")
                                                                   .Replace("click", "Click");
                }
            }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            List<HtmlCompletion> list = new List<HtmlCompletion>();
            list.Add(new SimpleHtmlCompletion("className", context.Session));

            if (context.Element.Name == "label")
                list.Add(new SimpleHtmlCompletion("htmlFor", context.Session));

            return list;
        }
    }
}

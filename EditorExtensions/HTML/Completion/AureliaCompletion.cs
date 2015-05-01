using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Completion;

namespace MadsKristensen.EditorExtensions.Html
{
    [HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
    [Export(typeof(IHtmlCompletionListFilter))]
    [ContentType("htmlx")]
    public class AureliaCompletion : IHtmlCompletionListProvider, IHtmlCompletionListFilter
    {
        private static BitmapFrame _icon = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/Resources/Images/aurelia.png", UriKind.RelativeOrAbsolute));

        public string CompletionType
        {
            get
            {
                return CompletionTypes.Attributes;
            }
        }

        public void FilterCompletionList(IList<HtmlCompletion> completions, HtmlCompletionContext context)
        {
            if (context.Attribute == null || !IsValid(context.Element))
                return;

            string name = context.Attribute.Name;

            if (!name.Contains("."))
                return;

            foreach (var completion in completions)
            {
                if (completion.IconSource != _icon)
                    completion.FilterType = CompletionEntryFilterTypes.NeverVisible;
            }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            List<HtmlCompletion> list = new List<HtmlCompletion>();

            bool isValid = IsValid(context.Element);
            string name = context.Attribute != null ? context.Attribute.Name : string.Empty;
            int dotIndex = name.IndexOf('.');

            if (isValid && dotIndex == -1 && 
                context.Element.Name != "html" && 
                context.Element.Name != "head" && 
                context.Element.Name != "body")
            {
                var entry = new SimpleHtmlCompletion("repeat.for", context.Session);
                entry.IconSource = _icon;
                list.Add(entry);
            }
            
            if (dotIndex == -1)
                return list;

            if (isValid && !name.StartsWith("repeat."))
            {
                list.Add(new SimpleHtmlCompletion("bind", "", name + "bind", _icon, context.Session));
                list.Add(new SimpleHtmlCompletion("two-way", "Force two-way data binding", name + "two-way", _icon, context.Session));
                list.Add(new SimpleHtmlCompletion("one-way", "Force one-way data binding", name + "one-way", _icon, context.Session));
                list.Add(new SimpleHtmlCompletion("one-time", "Force one-time data binding", name + "one-time", _icon, context.Session));
                list.Add(new SimpleHtmlCompletion("delegate", "Attaches a delegated event", name + "delegate", _icon, context.Session));
                list.Add(new SimpleHtmlCompletion("trigger", "Attaches an event to the element", name + "trigger", _icon, context.Session));
            }

            return list;
        }

        private static bool IsValid(ElementNode element)
        {
            if (element == null)
                return false;

            ElementNode parent = element.Parent;

            while (parent != null)
            {
                if (parent.Name == "template")
                    return true;

                parent = parent.Parent;
            }

            return false;
        }
    }
}

using Microsoft.Html.Core;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Core.ContentTypes;
using Microsoft.Web.Editor;
using System.Collections.Generic;
using System.Linq;

namespace MadsKristensen.EditorExtensions.Html
{
    [HtmlCompletionProvider(CompletionTypes.GroupValues, "*", "*")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class AriaIdCompletion : IHtmlCompletionListProvider, IHtmlTreeVisitor
    {
        private static readonly List<string> _attrs = new List<string>()
        {
            "aria-controls",
            "aria-describedby",
            "aria-labelledby"
        };

        public string CompletionType
        {
            get { return CompletionTypes.Values; }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            var list = new HashSet<string>();

            if (context.Attribute != null && _attrs.Contains(context.Attribute.Name))
            {
                context.Document.HtmlEditorTree.RootNode.Accept(this, list);
            }

            return list.Select(s => new SimpleHtmlCompletion(s, context.Session)).ToList<HtmlCompletion>();
        }

        public bool Visit(ElementNode element, object parameter)
        {
            if (element.HasAttribute("id"))
            {
                var list = (HashSet<string>)parameter;
                list.Add(element.GetAttribute("id").Value);
            }

            return true;
        }
    }
}

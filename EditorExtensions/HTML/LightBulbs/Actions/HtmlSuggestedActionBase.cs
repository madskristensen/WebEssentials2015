using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadsKristensen.EditorExtensions.Html
{
    internal abstract class HtmlSuggestedActionBase : SuggestedActionBase
    {
        public ElementNode Element { get; private set; }
        public AttributeNode Attribute { get; private set; }

        protected HtmlSuggestedActionBase(ITextView textView, ITextBuffer textBuffer, ElementNode element, string displayText)
            : this(textView, textBuffer, element, null, displayText)
        {
        }

        protected HtmlSuggestedActionBase(ITextView textView, ITextBuffer textBuffer, ElementNode element, AttributeNode attribute, string displayText)
            : base(textBuffer, textView, displayText)
        {
            Element = element;
            Attribute = attribute;
        }
    }
}

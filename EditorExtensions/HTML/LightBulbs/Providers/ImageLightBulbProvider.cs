using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Core.Tree.Utility;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Core.ContentTypes;

namespace MadsKristensen.EditorExtensions.Html
{
    [Export(typeof(IHtmlSuggestedActionProvider))]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    [Name("Html Image Light Bulb Provider")]
    internal class HtmlImageLightBulbProvider : IHtmlSuggestedActionProvider
    {
        public IEnumerable<ISuggestedAction> GetSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
        {
            AttributeNode src = element.GetAttribute("src");

            return new ISuggestedAction[] {
                    new HtmlBase64DecodeLightBulbAction(textView, textBuffer, element, src)
                };
        }

        public bool HasSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
        {
            if (!element.IsElement("img"))
                return false;

            AttributeNode src = element.GetAttribute("src");

            if (src == null)
                return false;

            return src.Value.StartsWith("data:image/", StringComparison.Ordinal);
        }
    }
}

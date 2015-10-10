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
	[Name("HtmlRemoveElementLightBulbProvider")]
	internal class HtmlRemoveElementLightBulbProvider : IHtmlSuggestedActionProvider
	{
        public IEnumerable<ISuggestedAction> GetSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
        {
            return new ISuggestedAction[] {
                new HtmlRemoveElementLightBulbAction(textView, textBuffer, element)
            };
		}

		public bool HasSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			if (element.IsRoot || element.EndTag == null || (!element.StartTag.Contains(caretPosition) && !element.EndTag.Contains(caretPosition)))
				return false;

			return element.InnerRange != null && element.GetText(element.InnerRange).Trim().Length > 0;
		}
	}
}

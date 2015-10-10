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
	[Name("Html Angular Controller Light Bulb Provider")]
	class HtmlAngularControllerLightBulbProvider : IHtmlSuggestedActionProvider
	{
		public IEnumerable<ISuggestedAction> GetSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			AttributeNode ngController = element.GetAttribute("ng-controller") ?? element.GetAttribute("data-ng-controller");

            return new ISuggestedAction[] {
                new HtmlAngularControllerLightBulbAction(textView, textBuffer, element, ngController)
            };
		}

		public bool HasSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			return element.HasAttribute("ng-controller") || element.HasAttribute("data-ng-controller");
        }
	}
}

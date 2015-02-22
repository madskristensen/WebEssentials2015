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
	[Order(Before = "Default")]
	[Name("HtmlStyleScriptLightBulbProvider")]
	internal class HtmlStyleScriptLightBulbProvider : IHtmlSuggestedActionProvider
	{
		public IEnumerable<ISuggestedAction> GetSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			if (HasSuggestedActions(textView, textBuffer, caretPosition, element, attribute, positionType))
			{
				yield return new HtmlMinifyLightBulbAction(textView, textBuffer, element);
				yield return new HtmlExtractLightBulbAction(textView, textBuffer, element);
			}
		}

		public bool HasSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			if (!element.StartTag.Contains(caretPosition))
				return false;

			if (element.InnerRange.Length < 5)
				return false;

			return element.IsStyleBlock() || element.IsJavaScriptBlock();
		}
	}	
}

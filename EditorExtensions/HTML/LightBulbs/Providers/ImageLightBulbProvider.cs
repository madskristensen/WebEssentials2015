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
	[Name("HtmlImageLightBulbProvider")]
	internal class HtmlImageLightBulbProvider : IHtmlSuggestedActionProvider
	{
		public IEnumerable<ISuggestedAction> GetSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			AttributeNode src = element.GetAttribute("src");

            if (src.Value.StartsWith("data:image/", StringComparison.Ordinal))
			{
				yield return new HtmlBase64DecodeLightBulbAction(textView, textBuffer, element, src);
			}

			//if (!src.Value.StartsWith("http:") && !src.Value.StartsWith("https:") && !src.Value.StartsWith("//"))
			//{
			//	yield return new HtmlOptimizeImageLightBulbAction(textView, textBuffer, element, src); 
			//}
		}

		public bool HasSuggestedActions(ITextView textView, ITextBuffer textBuffer, int caretPosition, ElementNode element, AttributeNode attribute, HtmlPositionType positionType)
		{
			if (element.Name != "img")
				return false;

			AttributeNode src = element.GetAttribute("src");

			return src != null && src.Value.Trim().Length > 0;
		}
	}
}

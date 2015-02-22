using System;
using System.Threading;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlBase64DecodeLightBulbAction : HtmlSuggestedActionBase
	{
		public HtmlBase64DecodeLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element, AttributeNode attribute)
			: base(textView, textBuffer, element, attribute)
		{		}

		public override string DisplayText
		{
			get { return "Save as File..."; }
		}

		public async override void Invoke(CancellationToken cancellationToken)
		{
			string mimeType = FileHelpers.GetMimeTypeFromBase64(Attribute.Value);
			string extension = FileHelpers.GetExtension(mimeType) ?? "png";

			var fileName = FileHelpers.ShowDialog(extension);

			if (!string.IsNullOrEmpty(fileName) && await FileHelpers.SaveDataUriToFile(Attribute.Value, fileName))
			{
				string relative = FileHelpers.RelativePath(TextBuffer.GetFileName(), fileName);

				using (WebEssentialsPackage.UndoContext((this.DisplayText)))
				using (ITextEdit edit = TextBuffer.CreateEdit())
				{
					edit.Replace(Attribute.ValueRangeUnquoted.ToSpan(), relative.ToLowerInvariant());
					edit.Apply();
				}
			}
		}
	}
}

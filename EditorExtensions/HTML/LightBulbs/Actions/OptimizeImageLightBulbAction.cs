using System;
using System.IO;
using System.Threading;
using MadsKristensen.EditorExtensions.Images;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlOptimizeImageLightBulbAction : HtmlSuggestedActionBase
	{
		public HtmlOptimizeImageLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element, AttributeNode attribute)
			: base(textView, textBuffer, element, attribute, "Optimize Image (lossless)")
		{ }
        
		public async override void Invoke(CancellationToken cancellationToken)
		{
			ImageCompressor compressor = new ImageCompressor();

			bool isDataUri = Attribute.Value.StartsWith("data:image/", StringComparison.Ordinal);

			if (isDataUri)
			{
				string dataUri = await compressor.CompressDataUriAsync(Attribute.Value);

				if (dataUri.Length < Attribute.Value.Length)
				{
					using (WebEssentialsPackage.UndoContext(this.DisplayText))
					using (ITextEdit edit = TextBuffer.CreateEdit())
					{
						Span span = Span.FromBounds(Attribute.ValueRangeUnquoted.Start, Attribute.ValueRangeUnquoted.End);
						edit.Replace(span, dataUri);
						edit.Apply();
					}
				}
			}
			else
			{
				var fileName = ImageQuickInfo.GetFullUrl(Attribute.Value, TextBuffer);

				if (string.IsNullOrEmpty(fileName) || !ImageCompressor.IsFileSupported(fileName) || !File.Exists(fileName))
					return;

				await compressor.CompressFilesAsync(fileName);
			}
		}
	}
}

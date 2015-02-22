using System.Threading;
using MadsKristensen.EditorExtensions.Optimization.Minification;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlMinifyLightBulbAction : HtmlSuggestedActionBase
	{
		public HtmlMinifyLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element)
			: base(textView, textBuffer, element)
		{ }

		public override string DisplayText
		{
			get { return "Minify " + (Element.IsStyleBlock() ? "CSS" : "JavaScript"); }
		}

		public override void Invoke(CancellationToken cancellationToken)
		{
			string text = Element.GetText(Element.InnerRange);
			IFileMinifier minifier = Element.IsScriptBlock() ? (IFileMinifier)new JavaScriptFileMinifier() : new CssFileMinifier();
			string result = minifier.MinifyString(text);

			using (WebEssentialsPackage.UndoContext((this.DisplayText)))
			{
				using (ITextEdit edit = TextBuffer.CreateEdit())
				{
					edit.Replace(Element.InnerRange.ToSpan(), result);
					edit.Apply();
				}
			}
		}
	}
}

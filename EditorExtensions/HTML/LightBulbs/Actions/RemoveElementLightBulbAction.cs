using System.Threading;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlRemoveElementLightBulbAction : HtmlSuggestedActionBase
	{
		private AttributeNode _src;
		public HtmlRemoveElementLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element)
			: base(textView, textBuffer, element)
		{
			_src = element.GetAttribute("src", true);
		}

		public override string DisplayText
		{
			get { return Element.Children.Count == 0 ? "Remove <" + Element.StartTag.Name + "> tag" : "Remove <" + Element.StartTag.Name + "> and Keep Children"; }
		}
		
		public override void Invoke(CancellationToken cancellationToken)
		{
			var content = Element.GetText(Element.InnerRange).Trim();
			int start = Element.Start;
			int length = content.Length;

			using (WebEssentialsPackage.UndoContext((this.DisplayText)))
			{
				using (ITextEdit edit = TextBuffer.CreateEdit())
				{
					edit.Replace(Element.OuterRange.ToSpan(), content);
					edit.Apply();
				}

				SnapshotSpan span = Element.ToSnapshotSpan(TextView.TextBuffer.CurrentSnapshot);// new SnapshotSpan(TextView.TextBuffer.CurrentSnapshot, start, length);

				TextView.Selection.Select(span, false);
				WebEssentialsPackage.ExecuteCommand("Edit.FormatSelection");
				TextView.Caret.MoveTo(new SnapshotPoint(TextView.TextBuffer.CurrentSnapshot, start));
				TextView.Selection.Clear();
			}
		}
	}
}

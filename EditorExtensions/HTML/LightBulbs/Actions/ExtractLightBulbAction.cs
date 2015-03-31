using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlExtractLightBulbAction : HtmlSuggestedActionBase
	{
		public HtmlExtractLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element)
			: base(textView, textBuffer, element, "Extract to File...")
		{ }

		public async override void Invoke(CancellationToken cancellationToken)
		{
			string file;
			string root = ProjectHelpers.GetProjectFolder(WebEssentialsPackage.DTE.ActiveDocument.FullName);

			if (CanSaveFile(root, out file))
			{
				await MakeChanges(root, file);
			}
		}

		private bool CanSaveFile(string folder, out string fileName)
		{
			string ext = Element.IsStyleBlock() ? "css" : "js";

			fileName = null;

			using (var dialog = new SaveFileDialog())
			{
				dialog.FileName = "file." + ext;
				dialog.DefaultExt = "." + ext;
				dialog.Filter = ext.ToUpperInvariant() + " files | *." + ext;
				dialog.InitialDirectory = folder;

				if (dialog.ShowDialog() != DialogResult.OK)
					return false;

				fileName = dialog.FileName;
			}

			return true;
		}

		private async Task MakeChanges(string root, string fileName)
		{
			string text = Element.GetText(Element.InnerRange).Trim();
			string reference = GetReference(Element, fileName, root);

			using (WebEssentialsPackage.UndoContext((this.DisplayText)))
			{
				using (ITextEdit edit = TextBuffer.CreateEdit())
				{
					edit.Replace(new Span(Element.Start, Element.Length), reference);
					edit.Apply();
				}

				await FileHelpers.WriteAllTextRetry(fileName, text);
				ProjectHelpers.AddFileToActiveProject(fileName);
				WebEssentialsPackage.DTE.ItemOperations.OpenFile(fileName);

				await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => {

					WebEssentialsPackage.ExecuteCommand("Edit.FormatDocument");

				}), DispatcherPriority.ApplicationIdle, null);				
			}
		}

		private static string GetReference(ElementNode element, string fileName, string root)
		{
			string relative = FileHelpers.RelativePath(root, fileName);
			string reference = "<script src=\"/{0}\"></script>";

			if (element.IsStyleBlock())
				reference = "<link rel=\"stylesheet\" href=\"/{0}\" />";

			return string.Format(CultureInfo.CurrentCulture, reference, HttpUtility.HtmlAttributeEncode(relative));
		}
	}
}

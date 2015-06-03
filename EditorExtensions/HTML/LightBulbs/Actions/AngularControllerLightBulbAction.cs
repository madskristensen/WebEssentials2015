using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlAngularControllerLightBulbAction : HtmlSuggestedActionBase
	{
		private AttributeNode _ngController;

		public HtmlAngularControllerLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element, AttributeNode attribute)
			: base(textView, textBuffer, element, attribute, "Add new Angular Controller")
		{
			_ngController = attribute;
            IconMoniker = KnownMonikers.JSScript;
		}

		public async override void Invoke(CancellationToken cancellationToken)
		{
			string value = _ngController.Value;

			if (string.IsNullOrEmpty(value))
				value = "myController";

			string folder = ProjectHelpers.GetProjectFolder(WebEssentialsPackage.DTE.ActiveDocument.FullName);
			string file;

			using (var dialog = new SaveFileDialog())
			{
				dialog.FileName = value + ".js";
				dialog.DefaultExt = ".js";
				dialog.Filter = "JS files | *.js";
				dialog.InitialDirectory = folder;

				if (dialog.ShowDialog() != DialogResult.OK)
					return;

				file = dialog.FileName;
			}

			using (WebEssentialsPackage.UndoContext((this.DisplayText)))
			{
				string script = GetScript(value);
				await FileHelpers.WriteAllTextRetry(file, script);

				ProjectHelpers.AddFileToActiveProject(file);
				WebEssentialsPackage.DTE.ItemOperations.OpenFile(file);
			}
		}

		private static string GetScript(string value)
		{
			using (Stream stream = typeof(HtmlAngularControllerLightBulbAction).Assembly.GetManifestResourceStream("MadsKristensen.EditorExtensions.Resources.Scripts.AngularController.js"))
			using (StreamReader reader = new StreamReader(stream))
			{
				return string.Format(CultureInfo.CurrentCulture, reader.ReadToEnd(), value);
			}
		}
	}
}

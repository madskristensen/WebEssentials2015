using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Html.Core.Tree.Nodes;
using Microsoft.Html.Editor.SuggestedActions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.Web.Editor.Text;

namespace MadsKristensen.EditorExtensions.Html
{
	internal class HtmlAngularControllerLightBulbAction : HtmlSuggestedActionBase
	{
		private static BitmapFrame _icon = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/Resources/Images/angular.png", UriKind.RelativeOrAbsolute));
		private AttributeNode _ngController;

		public HtmlAngularControllerLightBulbAction(ITextView textView, ITextBuffer textBuffer, ElementNode element, AttributeNode attribute)
			: base(textView, textBuffer, element)
		{
			_ngController = attribute;
		}

		public override string DisplayText
		{
			get { return "Add new Angular Controller"; }
		}

		public override ImageSource IconSource
		{
			get { return _icon; }
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

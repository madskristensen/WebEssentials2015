using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.Markdown
{
	[Export(typeof(IVsTextViewCreationListener))]
	[ContentType(MarkdownContentTypeDefinition.MarkdownContentType)]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	class MarkdownCreationListener : IVsTextViewCreationListener
	{
		[Import]
		IVsEditorAdaptersFactoryService AdaptersFactory = null;

		public void VsTextViewCreated(IVsTextView textViewAdapter)
		{
			IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);

			view.Options.SetOptionValue<bool>(DefaultOptions.ConvertTabsToSpacesOptionId, true);
		}
	}
}

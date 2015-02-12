using System;
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
			
			view.Properties.GetOrCreateSingletonProperty(() => new CommandFilter(textViewAdapter, view));
		}
	}

	internal sealed class CommandFilter : CommandTargetBase<VSConstants.VSStd2KCmdID>
	{
		public CommandFilter(IVsTextView adapter, IWpfTextView textView)
			: base(adapter, textView, VSConstants.VSStd2KCmdID.RETURN)
		{ }

		protected override bool IsEnabled()
		{
			return true;
		}

		protected override bool Execute(VSConstants.VSStd2KCmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			int position = TextView.Caret.Position.BufferPosition;
			TextView.TextBuffer.Insert(position, Environment.NewLine);
			return true;
		}
	}
}

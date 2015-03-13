using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
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
		[Import]
		IEditorOperationsFactoryService OperationsFactory = null;

		public void VsTextViewCreated(IVsTextView textViewAdapter)
		{
			IWpfTextView view = AdaptersFactory.GetWpfTextView(textViewAdapter);

			view.Options.SetOptionValue<bool>(DefaultOptions.ConvertTabsToSpacesOptionId, true);
			
			view.Properties.GetOrCreateSingletonProperty(() => new CommandFilter(textViewAdapter, view, OperationsFactory.GetEditorOperations(view)));
		}
	}

	internal sealed class CommandFilter : CommandTargetBase<VSConstants.VSStd2KCmdID>
	{
		private IEditorOperations editorOperations;

		public CommandFilter(IVsTextView adapter, IWpfTextView textView, IEditorOperations editorOperations)
			: base(adapter, textView, VSConstants.VSStd2KCmdID.RETURN)
		{
			this.editorOperations = editorOperations;
		}
		

		protected override bool IsEnabled()
		{
			return true;
		}

		protected override bool Execute(VSConstants.VSStd2KCmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			editorOperations.InsertNewLine();
			return true;
		}
	}
}

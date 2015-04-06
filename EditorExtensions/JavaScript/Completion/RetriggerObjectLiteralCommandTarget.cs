using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    internal class RetriggerObjectLiteralCommandTarget : CommandTargetBase<VSConstants.VSStd2KCmdID>
    {
        private ITextView _textView;
        private ICompletionBroker _broker;

        public RetriggerObjectLiteralCommandTarget(IVsTextView adapter, IWpfTextView textView, ICompletionBroker broker)
            : base(adapter, textView, VSConstants.VSStd2KCmdID.TYPECHAR)
        {
            _textView = textView;
            _broker = broker;
        }

        private void Retrigger()
        {
            if (_broker.IsCompletionActive(_textView))
                return;

            WebEssentialsPackage.ExecuteCommand("Edit.ListMembers");
        }

        protected override bool IsEnabled()
        {
            return true;
        }

        protected override bool Execute(VSConstants.VSStd2KCmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

            if (typedChar == ' ' || char.IsLetter(typedChar))
            {
                Retrigger();
                System.Threading.Tasks.Task.Delay(20);
            }

            return false;
        }
    }
}
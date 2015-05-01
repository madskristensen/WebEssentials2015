using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.Html.Editor.Completion;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace MadsKristensen.EditorExtensions.Html
{
    internal class RetriggerTarget : IOleCommandTarget
    {
        private ITextView _textView;
        private IOleCommandTarget _nextCommandTarget;
        private ICompletionBroker _broker;

        public RetriggerTarget(IVsTextView adapter, ITextView textView, ICompletionBroker broker)
        {
            _textView = textView;
            _broker = broker;
            ErrorHandler.ThrowOnFailure(adapter.AddCommandFilter(this, out _nextCommandTarget));
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

                switch (typedChar)
                {
                    case '.':
                        Retrigger();
                        break;
                }
            }

            return _nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        private void Retrigger()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                WebEssentialsPackage.ExecuteCommand("Edit.ListMembers");

            }), DispatcherPriority.Background, null);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return _nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
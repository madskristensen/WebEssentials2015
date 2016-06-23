using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MadsKristensen.EditorExtensions.Html
{
    /// <summary>
    /// Base class for suggested action implementations.  Actions can derive from this type and specialize as necessary for their appropriate
    /// language context (e.g. HTML, JSON, etc).
    /// </summary>
    public abstract class SuggestedActionBase : ISuggestedAction
    {
        public SuggestedActionBase(ITextBuffer buffer, ITextView view, string displayText)
        {
            TextBuffer = buffer;
            TextView = view;
            DisplayText = displayText;
        }

        public ITextBuffer TextBuffer
        {
            get;
            private set;
        }

        public ITextView TextView
        {
            get;
            private set;
        }

        #region ISuggestedAction members

        /// <summary>
        /// By default, nested actions are not supported.
        /// </summary>
        public virtual bool HasActionSets
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// By default, nested actions are not supported.
        /// </summary>
        public virtual Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public string DisplayText
        {
            get;
            private set;
        }

        public string IconAutomationText
        {
            get;
            protected set;
        }

        public ImageMoniker IconMoniker
        {
            get;
            protected set;
        }

        public string InputGestureText
        {
            get;
            protected set;
        }

        /// <summary>
        /// By default, Preview is not supported.
        /// </summary>
        public virtual bool HasPreview
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// By default, Preview is not supported.
        /// </summary>
        public virtual Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        public abstract void Invoke(CancellationToken cancellationToken);

        /// <summary>
        /// By default, telemetry is not supported.
        /// </summary>
        public virtual bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #endregion
    }
}


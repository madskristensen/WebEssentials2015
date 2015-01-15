using System.Collections.Generic;
using System.ComponentModel.Composition;
using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio.Utilities;
using Microsoft.CSS.Editor.Completion;
using Microsoft.Web.Editor.Completion;

namespace MadsKristensen.EditorExtensions.Css
{
    [Export(typeof(ICssCompletionListFilter))]
    [Name("Inherit/Initial Filter")]
    internal class HideInheritInitialCompletionListFilter : ICssCompletionListFilter
    {
        public void FilterCompletionList(IList<CssCompletionEntry> completions, CssCompletionContext context)
        {
            if (context.ContextType != CssCompletionContextType.PropertyValue || WESettings.Instance.Css.ShowInitialInherit)
                return;

            foreach (CssCompletionEntry entry in completions)
            {
                if (entry.DisplayText == "initial" || entry.DisplayText == "inherit")
                {
                    entry.FilterType = CompletionEntryFilterTypes.NeverVisible;
                }
            }
        }
    }
}
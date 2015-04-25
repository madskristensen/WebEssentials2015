using System.Collections.Generic;
using System.ComponentModel.Composition;
using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio.Utilities;
using Microsoft.CSS.Editor.Completion;
using Microsoft.Web.Editor.Completion;
using Microsoft.CSS.Core.TreeItems;

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

            // Only show inherit/initial/unset on the "all" property
            Declaration dec = context.ContextItem.FindType<Declaration>();

            if (dec != null && dec.PropertyNameText == "all")
                return;

            foreach (CssCompletionEntry entry in completions)
            {
                if (entry.DisplayText == "initial" || entry.DisplayText == "inherit" || entry.DisplayText == "unset")
                {
                    entry.FilterType = CompletionEntryFilterTypes.NeverVisible;
                }
            }
        }
    }
}
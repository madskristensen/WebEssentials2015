using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Core.ContentTypes;

namespace MadsKristensen.EditorExtensions.Html
{
    [Export(typeof(IHtmlCompletionListFilter))]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class MSApplicationCompletionFilter : IHtmlCompletionListFilter
    {
        private static BitmapSource _icon = ImageHelper.GetImage(KnownMonikers.Windows);
        public void FilterCompletionList(IList<HtmlCompletion> completions, HtmlCompletionContext context)
        {
            foreach (var completion in completions)
            {
                if (completion.DisplayText.StartsWith("msapplication-", StringComparison.OrdinalIgnoreCase)||
                    completion.DisplayText.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase))
                {
                    completion.IconSource = _icon;
                }
            }
        }
    }
}

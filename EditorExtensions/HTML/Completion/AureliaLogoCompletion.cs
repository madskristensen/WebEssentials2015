using MadsKristensen.EditorExtensions.Html;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Core.ContentTypes;
using Microsoft.Web.Editor;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MadsKristensen.EditorExtensions.Html
{
    [HtmlCompletionProvider(CompletionType.Attributes, "*", "*")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class AureliaLogo2Completion : IHtmlCompletionListProvider
    {
        private static BitmapFrame _icon = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/Resources/Images/aurelia.png", UriKind.RelativeOrAbsolute));

        public CompletionType CompletionType
        {
            get { return CompletionType.Attributes; }
        }

        public IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            return new List<HtmlCompletion>
            {
                 new SimpleHtmlCompletion("aurelia-app", context.Session)
                 {
                     IconSource = _icon
                 },
                 new SimpleHtmlCompletion("aurelia-main", context.Session)
                 {
                     IconSource = _icon
                 },
            };
        }
    }
}

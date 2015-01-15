using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.Web.Core.ContentTypes;

namespace MadsKristensen.EditorExtensions.Html
{
    [HtmlCompletionProvider(Microsoft.Html.Editor.Completion.Def.CompletionType.Values, "meta", "content")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class AppleMetaCompletion : StaticListCompletion
    {
        protected override string KeyProperty { get { return "name"; } }
        public AppleMetaCompletion()
            : base(new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "apple-mobile-web-app-capable",           Values("yes", "no") },
                { "format-detection",                       Values("telephone=yes", "telephone=no") },
                { "apple-mobile-web-app-status-bar-style",  Values("default", "black", "black-translucent") }
            }) { }
    }
}

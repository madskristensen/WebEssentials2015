using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.Web.Core.ContentTypes;

namespace MadsKristensen.EditorExtensions.Html
{
    [HtmlCompletionProvider(Microsoft.Html.Editor.Completion.Def.CompletionType.Values, "link", "sizes")]
    [ContentType(HtmlContentTypeDefinition.HtmlContentType)]
    public class AppleLinkCompletion : StaticListCompletion
    {
        protected override string KeyProperty { get { return "rel"; } }
        public AppleLinkCompletion()
            : base(new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "apple-touch-icon", Values("16x16", "32x32", "57x57", "60x60", "72x72", "76x76","96x96", "114x114", "120x120", "144x144", "152x152", "180x180", "192x192") }
            }) { }
    }
}

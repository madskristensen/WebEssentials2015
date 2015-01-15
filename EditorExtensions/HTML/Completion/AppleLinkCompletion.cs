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
                { "apple-touch-icon", Values("72x72", "114x114", "144x144") }
            }) { }
    }
}

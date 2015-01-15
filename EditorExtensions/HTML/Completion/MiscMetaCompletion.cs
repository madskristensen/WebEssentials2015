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
    public class MiscMetaCompletion : StaticListCompletion
    {
        protected override string KeyProperty { get { return "name"; } }
        public MiscMetaCompletion()
            : base(new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "generator",  Values("Visual Studio") },
                { "robots",     Values("index", "noindex", "follow", "nofollow", "noindex, nofollow", "noindex, follow", "index, nofollow") }
          }) { }
    }
}

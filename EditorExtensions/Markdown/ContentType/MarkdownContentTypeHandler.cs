using System.ComponentModel.Composition;
using Microsoft.Html.Core.Artifacts;
using Microsoft.Html.Editor.ContentType.Def;
using Microsoft.Html.Editor.ContentType.Handlers;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor.Formatting;

namespace MadsKristensen.EditorExtensions.Markdown
{
    [Export(typeof(IContentTypeHandlerProvider))]
    [ContentType(MarkdownContentTypeDefinition.MarkdownContentType)]
    public class MarkdownContentTypeHandlerProvider : IContentTypeHandlerProvider
    {
        public IContentTypeHandler GetContentTypeHandler()
        {
            return new MarkdownContentTypeHandler();
        }
    }

    public class MarkdownContentTypeHandler : HtmlContentTypeHandler
    {
        public override ArtifactCollection CreateArtifactCollection()
        {
            return new MarkdownCodeArtifactCollection(new MarkdownCodeArtifactProcessor());
        }
    }

    // The HTML formatter doesn't work properly with Artifacts
    // unless you implement a whole bunch of internal features
    // for Razor (providing ArtifactGroups).  Plus, it doesn't
    // work properly with Artifacts in other ways (it swallows
    // separators).  I disable it entirely to avoid trouble.
    [Export(typeof(IEditorFormatterProvider))]
    [ContentType(MarkdownContentTypeDefinition.MarkdownContentType)]
    public class MarkdownNonFormatterProvider : IEditorFormatterProvider
    {
        public IEditorFormatter CreateFormatter() { return null; }
        public IEditorRangeFormatter CreateRangeFormatter() { return null; }
    }
}
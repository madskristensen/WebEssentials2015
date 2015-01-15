using System.Windows.Media;
using Microsoft.Web.Editor;
using vs = Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Html;
using Microsoft.Web.Editor.Imaging;

namespace MadsKristensen.EditorExtensions.Html
{
    public class SimpleHtmlCompletion : HtmlCompletion
    {
        private static ImageSource _glyph = GlyphService.GetGlyph(vs.StandardGlyphGroup.GlyphGroupVariable, vs.StandardGlyphItem.GlyphItemPublic);

        public SimpleHtmlCompletion(string displayText, vs.ICompletionSession session)
            : base(displayText, displayText, string.Empty, _glyph, HtmlIconAutomationText.AttributeIconText, session)
        { }

        public SimpleHtmlCompletion(string displayText, string description, vs.ICompletionSession session)
            : base(displayText, displayText, description, _glyph, HtmlIconAutomationText.AttributeIconText, session)
        { }
    }
}

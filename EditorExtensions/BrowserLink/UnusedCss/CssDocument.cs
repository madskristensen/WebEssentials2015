using Microsoft.CSS.Core;
using Microsoft.CSS.Core.Parser;

namespace MadsKristensen.EditorExtensions.BrowserLink.UnusedCss
{
    public class CssDocument : DocumentBase
    {
        private CssDocument(string file)
            : base(file)
        {
        }

        protected override ICssParser CreateParser()
        {
            return new CssParser();
        }

        internal static IDocument For(string fullPath, bool createIfRequired = false)
        {
            return For(fullPath, createIfRequired, f => new CssDocument(f));
        }
    }
}
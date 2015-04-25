using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.CSS.Editor.Completion;

namespace MadsKristensen.EditorExtensions.CSS
{
    class BaseRegexCompletionEntryGlyphProvider : ICssCompletionEntryGlyphProvider
    {
        protected ImageSource Icon { get; set; }
        protected Regex RegExp { get; set; }

        public BaseRegexCompletionEntryGlyphProvider(ImageSource icon, Regex regex)
        {
            Icon = icon;
            RegExp = regex;
        }

        public ImageSource GetCompletionGlyph(string entryName, Uri sourceUri, CssNameType nameType)
        {
            if (sourceUri == null)
            {
                return null;
            }

            string filename = Path.GetFileName(sourceUri.ToString()).Trim();

            if (RegExp.IsMatch(filename))
            {
                return Icon;
            }

            return null;
        }
    }
}

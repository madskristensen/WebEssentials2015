using Microsoft.CSS.Editor;
using Microsoft.CSS.Editor.Completion;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MadsKristensen.EditorExtensions.CSS
{
    [Export(typeof(ICssCompletionEntryGlyphProvider))]
    class SkeletonCompletionEntryGlyphProvider : ICssCompletionEntryGlyphProvider
    {
        private static BitmapFrame _icon = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/Resources/Images/skeleton.png", UriKind.RelativeOrAbsolute));
        private static Regex _regex = new Regex(@"^skeleton(-.*)?(\.min)?\.css$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ImageSource GetCompletionGlyph(string entryName, Uri sourceUri, CssNameType nameType)
        {
            if (sourceUri == null)
            {
                return null;
            }

            string filename = Path.GetFileName(sourceUri.ToString()).Trim();

            if (_regex.IsMatch(filename))
            {
                return _icon;
            }

            return null;
        }
    }
}

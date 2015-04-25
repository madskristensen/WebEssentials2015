using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Microsoft.CSS.Editor.Completion;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.CSS
{
    [Export(typeof(ICssCompletionEntryGlyphProvider))]
    [Name("Web Essentials Skeleton")]
    class SkeletonCompletionEntryGlyphProvider : BaseRegexCompletionEntryGlyphProvider
    {
        private static BitmapFrame _icon = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/Resources/Images/skeleton.png", UriKind.RelativeOrAbsolute));
        private static Regex _regex = new Regex(@"^skeleton(-.*)?(\.min)?\.css$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SkeletonCompletionEntryGlyphProvider()
            : base(_icon, _regex)
        { }
    }
}

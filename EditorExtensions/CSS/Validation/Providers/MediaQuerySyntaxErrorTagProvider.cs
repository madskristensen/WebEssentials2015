using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.CSS.Core;
using Microsoft.VisualStudio.Utilities;
using Microsoft.CSS.Core.Checker;
using Microsoft.CSS.Core.Parser;
using Microsoft.CSS.Core.TreeItems.AtDirectives;

namespace MadsKristensen.EditorExtensions.Css
{
    [Export(typeof(ICssItemChecker))]
    [Name("MediaQuerySyntaxErrorTagProvider")]
    [Order(After = "Default Declaration")]
    internal class MediaQuerySyntaxErrorTagProvider : ICssItemChecker
    {
        private const string _orInvalidMessage = "Validation: CSS media queries uses ',' as a logical or operation.";
        public ItemCheckResult CheckItem(ParseItem item, ICssCheckerContext context)
        {
            if (item.IsValid)
                return ItemCheckResult.Continue;

            if (item.Text.Contains(" or "))
            {
                ICssError tag = new SimpleErrorTag(item, _orInvalidMessage);
                context.AddError(tag);
                return ItemCheckResult.CancelCurrentItem;
            }

            return ItemCheckResult.Continue;
        }

        public IEnumerable<Type> ItemTypes
        {
            get { return new[] { typeof(MediaQuery) }; }
        }
    }
}
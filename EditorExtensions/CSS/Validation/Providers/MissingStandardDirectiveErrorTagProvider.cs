using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.CSS.Core;
using Microsoft.CSS.Editor.Intellisense;
using Microsoft.VisualStudio.Utilities;
using MadsKristensen.EditorExtensions.Settings;

namespace MadsKristensen.EditorExtensions.Css
{
    [Export(typeof(ICssItemChecker))]
    [Name("MissingStandardDirectiveErrorTagProvider")]
    [Order(After = "Default Declaration")]
    internal class MissingStandardDirectiveErrorTagProvider : ICssItemChecker
    {
        public ItemCheckResult CheckItem(ParseItem item, ICssCheckerContext context)
        {
            if (!WESettings.Instance.Css.ValidateVendorSpecifics)
                return ItemCheckResult.Continue;

            AtDirective directive = (AtDirective)item;

            if (context == null || !directive.IsValid || !directive.IsVendorSpecific())
                return ItemCheckResult.Continue;

            ICssCompletionListEntry entry = VendorHelpers.GetMatchingStandardEntry(directive, context);

            if (entry != null)
            {
                var visitor = new CssItemCollector<AtDirective>();
                directive.Parent.Accept(visitor);

                if (!visitor.Items.Any(a => a != null && a.Keyword != null && "@" + a.Keyword.Text == entry.DisplayText))
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Resources.BestPracticeAddMissingStandardDirective, entry.DisplayText);
                    context.AddError(new SimpleErrorTag(directive.Keyword, message));
                    return ItemCheckResult.CancelCurrentItem;
                }
            }

            return ItemCheckResult.Continue;
        }

        public IEnumerable<Type> ItemTypes
        {
            get { return new[] { typeof(AtDirective) }; }
        }
    }
}

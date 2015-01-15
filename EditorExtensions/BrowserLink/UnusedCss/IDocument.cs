using System;
using System.Collections.Generic;
using Microsoft.CSS.Core;
using Microsoft.CSS.Core.TreeItems;

namespace MadsKristensen.EditorExtensions.BrowserLink.UnusedCss
{
    public interface IDocument : IDisposable
    {
        IEnumerable<IStylingRule> Rules { get; }
        bool IsProcessingUnusedCssRules { get; set; }
        object ParseSync { get; }
        string FileName { get; }

        void Reparse();
        void Reparse(string text);
        string GetSelectorName(RuleSet ruleSet);
    }
}

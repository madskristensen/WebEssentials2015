using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.Web.ProjectSystem.ProjectTree.NestingRules;

namespace MadsKristensen.EditorExtensions.Misc
{
    [Export(typeof(IProjectItemNestingRule))]
    [AppliesTo("projectk")]
    [OrderPrecedence(100)]
    class BowerRcNestingRule : IProjectItemNestingRule
    {
        public string GetNestingParent(string potentialChildFilePath, string potentialChildDirectory, IList<string> potentialParentProjectItems)
        {
            string fileName = Path.GetFileName(potentialChildFilePath);
            if (fileName != ".bowerrc") return null;

            return potentialParentProjectItems.FirstOrDefault(x => x == "bower.json");
        }
    }
}

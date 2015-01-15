using System.Collections.Generic;
using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;

namespace MadsKristensen.EditorExtensions.JSONLD
{
    interface IVocabulary
    {
        Dictionary<string, IEnumerable<Entry>> Cache { get; }

        string DisplayName { get; }

        bool AppliesToContext(JSONMember contextNode);
    }
}

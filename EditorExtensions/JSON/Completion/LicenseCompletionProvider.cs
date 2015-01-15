using Microsoft.JSON.Core.Parser;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace MadsKristensen.EditorExtensions.JSON
{
    /// <summary>
    /// Provides Intellisense for the "license" property of package.json and bower.json
    /// </summary>
    [Export(typeof(IJSONCompletionListProvider))]
    [Name("LicenseCompletionProvider")]
    internal class LicenseCompletionProvider : IJSONCompletionListProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        private static List<string> _files = new List<string> { "package.json", "bower.json" };
        private static List<string> _props = new List<string>
        {
            "Apache-2.0",
            "GPL-2.0",
            "GPL-3.0",
            "LGPL-2.1",
            "LGPL-3.0",
            "MIT",
            "MS-PL",
            "MS-RL",
         };

        public JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.PropertyValue; }
        }

        public IEnumerable<JSONCompletionEntry> GetListEntries(JSONCompletionContext context)
        {
            ITextDocument document;
            if (TextDocumentFactoryService.TryGetTextDocument(context.Snapshot.TextBuffer, out document))
            {
                string fileName = Path.GetFileName(document.FilePath).ToLowerInvariant();

                if (string.IsNullOrEmpty(fileName) || !_files.Contains(fileName))
                    yield break;
            }
            else {
                yield break;
            }

            JSONMember member = context.ContextItem as JSONMember;

            if (member == null || member.Name == null || member.UnquotedNameText != "license")
                yield break;

            foreach (string prop in _props)
            {
                yield return new SimpleCompletionEntry(prop, context.Session);
            }
        }
    }
}
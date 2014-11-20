using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Html.Editor;
using Microsoft.Html.Editor.Projection;
using Microsoft.JSON.Core.Format;
using Microsoft.JSON.Editor;
using Microsoft.JSON.Editor.Format;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor;

namespace MadsKristensen.EditorExtensions.Markdown
{
    ///<summary>Preprocesses embedded code Markdown blocks before creating projection buffers.</summary>
    ///<remarks>
    /// Implement this interface to initialize language services for
    /// your language, or to add custom wrapping text around blocks.
    /// Implementations should be state-less; only one instance will
    /// be created.
    ///</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Embedder")]
    public interface ICodeLanguageEmbedder
    {
        ///<summary>Gets a string to insert at the top of the generated ProjectionBuffr for this language.</summary>
        ///<remarks>Each Markdown file will have exactly one copy of this string in its code buffer.</remarks>
        string GlobalPrefix { get; }
        ///<summary>Gets a string to insert at the bottom of the generated ProjectionBuffr for this language.</summary>
        ///<remarks>Each Markdown file will have exactly one copy of this string in its code buffer.</remarks>
        string GlobalSuffix { get; }

        ///<summary>Gets text to insert around each embedded code block for this language.</summary>
        ///<param name="code">The lines of code in the block.  Enumerating this may be expensive.</param>
        ///<returns>
        /// One of the following:
        ///  - Null or an empty sequence to surround with \r\n.
        ///  - A single string to put on both ends of the code.
        ///  - Two strings; one for each end of the code block.
        /// The buffer generator will always add newlines.
        ///</returns>
        ///<remarks>
        /// These strings will be wrapped around every embedded
        /// code block separately.
        ///</remarks>
        IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code);

        ///<summary>Called when a block of this type is first created within a document.</summary>
        void OnBlockCreated(ITextBuffer editorBuffer, LanguageProjectionBuffer projectionBuffer);
    }

    [Export(typeof(ICodeLanguageEmbedder))]
    [ContentType("CSS")]
    public class CssEmbedder : ICodeLanguageEmbedder
    {

        public IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code)
        {
            // If the code doesn't have any braces, surround it in a ruleset so that properties are valid.
            if (code.All(t => t.IndexOfAny(new[] { '{', '}' }) == -1))
                return new[] { ".GeneratedClass-" + Guid.NewGuid() + " {", "}" };
            return null;
        }

        public void OnBlockCreated(ITextBuffer editorBuffer, LanguageProjectionBuffer projectionBuffer) { }
        public string GlobalPrefix { get { return ""; } }
        public string GlobalSuffix { get { return ""; } }
    }
    [Export(typeof(ICodeLanguageEmbedder))]
    [ContentType("Javascript")]
    [ContentType("node.js")]
    public class JavaScriptEmbedder : ICodeLanguageEmbedder
    {
        // Statements like return or arguments can only appear inside a function.
        // There are no statements that cannot appear in a function.
        // TODO: IntelliSense for Node.js vs. HTML.
        static readonly IReadOnlyCollection<string> wrapper = new[] { "function() {", "}" };
        public IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code) { return wrapper; }
        public void OnBlockCreated(ITextBuffer editorBuffer, LanguageProjectionBuffer projectionBuffer) { }
        public string GlobalPrefix { get { return ""; } }
        public string GlobalSuffix { get { return ""; } }
    }

    public abstract class RoslynEmbedder : ICodeLanguageEmbedder
    {
        public abstract IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code);
        static readonly IReadOnlyCollection<string> DefaultReferences = new[] {
            typeof(object),
            typeof(Uri),
            typeof(Enumerable),
            typeof(System.Net.Http.HttpClient),
            typeof(System.Xml.Linq.XElement),
            typeof(System.Web.HttpContextBase),
            typeof(System.Windows.Forms.Form),
            typeof(System.Windows.Window),
            typeof(System.Data.DataSet)
        }.Select(t => t.Assembly.GetName().Name).ToList();

        // Copied from VSEmbed.Roslyn.EditorWorkspace
        // This contains all of the ugly hacks needed
        // to make the Roslyn editor fully functional
        // on a custom Workspace
        class MarkdownWorkspace : Workspace
        {
            static readonly Type IWorkCoordinatorRegistrationService = Type.GetType("Microsoft.CodeAnalysis.SolutionCrawler.IWorkCoordinatorRegistrationService, Microsoft.CodeAnalysis.Features");

            readonly Dictionary<DocumentId, ITextBuffer> documentBuffers = new Dictionary<DocumentId, ITextBuffer>();
            public MarkdownWorkspace(HostServices host) : base(host, WorkspaceKind.Host)
            {
                var wcrService = typeof(HostWorkspaceServices)
                    .GetMethod("GetService")
                    .MakeGenericMethod(IWorkCoordinatorRegistrationService)
                    .Invoke(Services, null);

                IWorkCoordinatorRegistrationService.GetMethod("Register").Invoke(wcrService, new[] { this });
            }
            public Project AddProject(string name, string language)
            {
                ProjectInfo projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(null), VersionStamp.Create(), name, name, language);
                OnProjectAdded(projectInfo);
                return CurrentSolution.GetProject(projectInfo.Id);
            }

            static readonly string referenceAssemblyPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5"
            );


            //static readonly Type xmlDocProvider = typeof(MSBuildWorkspace).Assembly.GetType("Microsoft.CodeAnalysis.FileBasedXmlDocumentationProvider");
            public MetadataReference CreateFrameworkReference(string assemblyName)
            {
                return MetadataReference.CreateFromFile(
                    Path.Combine(referenceAssemblyPath, assemblyName + ".dll"),
                    MetadataReferenceProperties.Assembly
                    //(DocumentationProvider)Activator.CreateInstance(xmlDocProvider, Path.Combine(referenceAssemblyPath, assemblyName + ".xml"))
                );
            }


            ///<summary>Creates a new document linked to an existing text buffer.</summary>
            public Document CreateDocument(ProjectId projectId, ITextBuffer buffer)
            {
                var id = DocumentId.CreateNewId(projectId);
                documentBuffers.Add(id, buffer);

                var docInfo = DocumentInfo.Create(id, "Sample Document",
                    loader: TextLoader.From(buffer.AsTextContainer(), VersionStamp.Create()),
                    sourceCodeKind: SourceCodeKind.Script
                );
                OnDocumentAdded(docInfo);
                OnDocumentOpened(id, buffer.AsTextContainer());
                buffer.Changed += delegate { OnDocumentContextUpdated(id); };
                return CurrentSolution.GetDocument(id);
            }

            protected override void AddMetadataReference(ProjectId projectId, MetadataReference metadataReference)
            {
                OnMetadataReferenceAdded(projectId, metadataReference);
            }
            protected override void ChangedDocumentText(DocumentId id, SourceText text)
            {
                OnDocumentTextChanged(id, text, PreservationMode.PreserveValue);
                UpdateText(text, documentBuffers[id], EditOptions.DefaultMinimalChange);
            }

            // Stolen from Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem.DocumentProvider.StandardTextDocument
            private static void UpdateText(SourceText newText, ITextBuffer buffer, EditOptions options)
            {
                using (ITextEdit textEdit = buffer.CreateEdit(options, null, null))
                {
                    SourceText oldText = buffer.CurrentSnapshot.AsText();
                    foreach (TextChange current in newText.GetTextChanges(oldText))
                    {
                        textEdit.Replace(current.Span.Start, current.Span.Length, current.NewText);
                    }
                    textEdit.Apply();
                }
            }

            public override bool CanApplyChange(ApplyChangesKind feature)
            {
                switch (feature)
                {
                    case ApplyChangesKind.AddMetadataReference:
                    case ApplyChangesKind.RemoveMetadataReference:
                    case ApplyChangesKind.ChangeDocument:
                        return true;
                    case ApplyChangesKind.AddProject:
                    case ApplyChangesKind.RemoveProject:
                    case ApplyChangesKind.AddProjectReference:
                    case ApplyChangesKind.RemoveProjectReference:
                    case ApplyChangesKind.AddDocument:
                    case ApplyChangesKind.RemoveDocument:
                    default:
                        return false;
                }
            }

        }

        static readonly Dictionary<string, string> contentTypeLanguages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "CSharp", LanguageNames.CSharp },
            { "Basic", LanguageNames.VisualBasic }
        };
        [Import]
        public SVsServiceProvider ServiceProvider { get; set; }
        public void OnBlockCreated(ITextBuffer editorBuffer, LanguageProjectionBuffer projectionBuffer)
        {
            var componentModel = (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));

            var workspace = editorBuffer.Properties.GetOrCreateSingletonProperty(() =>
                new MarkdownWorkspace(MefV1HostServices.Create(componentModel.DefaultExportProvider))
            );

            var contentType = projectionBuffer.IProjectionBuffer.ContentType.DisplayName;
            var project = editorBuffer.Properties.GetOrCreateSingletonProperty(contentType, () =>
            {
                var newProject = workspace.AddProject(
                    "Sample " + contentType + " Project",
                    contentTypeLanguages[contentType]
                );
                workspace.TryApplyChanges(workspace.CurrentSolution.AddMetadataReferences(
                    newProject.Id,
                    DefaultReferences.Select(workspace.CreateFrameworkReference)
                ));
                return newProject;
            });
            workspace.CreateDocument(project.Id, projectionBuffer.IProjectionBuffer);
        }

        public virtual string GlobalPrefix { get { return ""; } }
        public virtual string GlobalSuffix { get { return ""; } }
    }

    [Export(typeof(ICodeLanguageEmbedder))]
    [ContentType("CSharp")]
    public class CSharpEmbedder : RoslynEmbedder
    {
        public override string GlobalPrefix
        {
            get
            {
                return @"using System;
                         using System.Collections.Generic;
                         using System.Data;
                         using System.IO;
                         using System.Linq;
                         using System.Net;
                         using System.Net.Http;
                         using System.Net.Http.Formatting;
                         using System.Reflection;
                         using System.Text;
                         using System.Threading;
                         using System.Threading.Tasks;
                         using System.Xml;
                         using System.Xml.Linq;";
            }
        }
        public override IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code)
        {
            return new[] { @"partial class Entry
                            {
                                  async Task<object> SampleMethod" + Guid.NewGuid().ToString("n") + @"() {", @"
                                return await Task.FromResult(new object());
                            }
                            }" };
        }
    }

    [Export(typeof(ICodeLanguageEmbedder))]
    [ContentType("Basic")]
    public class VBEmbedder : RoslynEmbedder
    {
        public override string GlobalPrefix
        {
            get
            {
                return @"Imports System
                        Imports System.Collections.Generic
                        Imports System.Data
                        Imports System.IO
                        Imports System.Linq
                        Imports System.Net
                        Imports System.Net.Http
                        Imports System.Net.Http.Formatting
                        Imports System.Reflection
                        Imports System.Text
                        Imports System.Threading
                        Imports System.Threading.Tasks
                        Imports System.Xml
                        Imports System.Xml.Linq";
            }
        }
        public override IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code)
        {
            return new[] { @"
                            Partial Class Entry
                            Async Function SampleMethod" + Guid.NewGuid().ToString("n") + @"() As Task(Of Object)", @"
                                Return Await Task.FromResult(New Object())
                            End Function
                            End Class" };
        }
    }


    // Ugly hacks because JSONIndenter uses textView.TextBuffer and
    // tries to operate with the outer Markdown TextBuffer, instead
    // of the inner JSON ProjectionBuffer.  To fix this, we need to
    // make sure that it uses the JSONEditorDocument from the inner
    // buffer, and that it successfully finds IJSONFormatterFactory
    // for Markdown.
    [Export(typeof(ICodeLanguageEmbedder))]
    [ContentType("JSON")]
    public class JSONEmbedder : ICodeLanguageEmbedder
    {
        public string GlobalPrefix { get { return ""; } }
        public string GlobalSuffix { get { return ""; } }

        public IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code)
        {
            return null;
        }

        public void OnBlockCreated(ITextBuffer editorBuffer, LanguageProjectionBuffer projectionBuffer)
        {
            WindowHelpers.WaitFor(delegate
            {
                var textView = TextViewConnectionListener.GetFirstViewForBuffer(editorBuffer);
                if (textView == null)
                    return false;
                // Add the inner buffer's EditorDocument to the outer buffer before
                // broken editor code tries to create a new EditorDocument from the
                // outer buffer.
                var editorDocument = JSONEditorDocument.FromTextBuffer(projectionBuffer.IProjectionBuffer);
                ServiceManager.AddService(editorDocument, textView.TextBuffer);
                editorDocument.Closing += delegate { ServiceManager.RemoveService<JSONEditorDocument>(textView.TextBuffer); };

                // JSONIndenter uses TextView.TextBuffer, and therefore operates on the
                // entire Markdown buffer, breaking everything.  I manually force it to
                // use the inner projection buffer instead. Beware that this breaks its
                // ViewCaret property, and I can't fix that unless I mock its TextView.
                var indenter = ServiceManager.GetService<ISmartIndent>(textView);
                indenter.GetType().GetField("_textBuffer", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(indenter, projectionBuffer.IProjectionBuffer);
                return true;
            });
        }
    }

    [Export(typeof(IJSONFormatterFactory))]
    [ContentType("HTMLXProjection")]
    [Name("Hack")]
    public class JSONFormatterPassThroughFactoryHack : IJSONFormatterFactory
    {
        public IJSONFormatter CreateFormatter()
        {
            return JSONFormatterLocator.FindComponent(ContentTypeManager.GetContentType("JSON")).CreateFormatter();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Html.Editor;
using Microsoft.Html.Editor.Projection;
using Microsoft.JSON.Core.Format;
using Microsoft.JSON.Editor;
using Microsoft.JSON.Editor.Format;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Web.Editor;
using Microsoft.Web.Editor.Controller;
using Microsoft.JSON.Editor.Document;
using Microsoft.Web.Editor.Services;

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
		public string GlobalPrefix { get { return "["; } }
		public string GlobalSuffix { get { return "{}]"; } }

		public IReadOnlyCollection<string> GetBlockWrapper(IEnumerable<string> code)
		{
			return new[] { "", "," };
		}

		public void OnBlockCreated(ITextBuffer editorBuffer, LanguageProjectionBuffer projectionBuffer)
		{
			WindowHelpers.WaitFor(delegate
			{
				var textView = TextViewConnectionListener.GetFirstViewForBuffer(editorBuffer);
				if (textView == null)
					return false;
				// Attach the inner buffer's Document to the outer 
				// buffer so that it can be found from the TextView
				var editorDocument = JSONEditorDocument.FromTextBuffer(projectionBuffer.IProjectionBuffer)
								  ?? JSONEditorDocument.Attach(projectionBuffer.IProjectionBuffer);
				ServiceManager.AddService(editorDocument, editorBuffer);
				editorDocument.Closing += delegate { ServiceManager.RemoveService<JSONEditorDocument>(textView.TextBuffer); };
				return true;
			});
		}
	}
	[Export(typeof(ITextViewCreationListener))]
	[ContentType("JSON")]
	class JsonBufferListener : ITextViewCreationListener
	{
		public void OnTextViewCreated(ITextView textView, ITextBuffer textBuffer)
		{
			var jsonBuffer = textView.BufferGraph.GetTextBuffers(tb => tb.ContentType.IsOfType("JSON")).FirstOrDefault();
			if (jsonBuffer == null) return;
			// Attach the inner buffer's Document to the outer 
			// buffer so that it can be found from the TextView
			var editorDocument = JSONEditorDocument.FromTextBuffer(jsonBuffer)
							  ?? JSONEditorDocument.Attach(jsonBuffer);
			ServiceManager.AddService(editorDocument, textView.TextDataModel.DocumentBuffer);
			editorDocument.Closing += delegate { ServiceManager.RemoveService<JSONEditorDocument>(textView.TextBuffer); };
		}
	}
}
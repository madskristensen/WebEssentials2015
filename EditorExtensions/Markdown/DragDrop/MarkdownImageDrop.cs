using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Web;
using MadsKristensen.EditorExtensions.Css;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.DragDrop;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.Markdown
{
    [Export(typeof(IDropHandlerProvider))]
    [DropFormat("FileDrop")]
    [DropFormat("CF_VSSTGPROJECTITEMS")]
    [Name("MarkdownImageDropHandler")]
    [ContentType("markdown")]
    [Order(Before = "DefaultFileDropHandler")]
    public class HtmlImageDropHandlerMarkdownImageDropHandlerProvider : IDropHandlerProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public IDropHandler GetAssociatedDropHandler(IWpfTextView wpfTextView)
        {
            return wpfTextView.Properties.GetOrCreateSingletonProperty(() => new MarkdownImageDropHandler(TextDocumentFactoryService, wpfTextView));
        }
    }

    public class MarkdownImageDropHandler : IDropHandler
    {
        const string MarkdownTemplate = "![{0}]({1})";
        static readonly HashSet<string> _imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".bmp", ".png", ".gif", ".svg", ".tif", ".tiff" };

        readonly ITextDocumentFactoryService _documentFactory;
        readonly IWpfTextView _view;

        string _imageFilename;

        public MarkdownImageDropHandler(ITextDocumentFactoryService documentFactory, IWpfTextView view)
        {
            _documentFactory = documentFactory;
            _view = view;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "img")]
        public DragDropPointerEffects HandleDataDropped(DragDropInfo dragDropInfo)
        {
            string reference = FileHelpers.RelativePath(WebEssentialsPackage.DTE.ActiveDocument.FullName, _imageFilename);

            if (reference.Contains("://"))
            {
                int index = reference.IndexOf('/', 12);
                if (index > -1)
                    reference = reference.Substring(index);
            }
            reference = HttpUtility.UrlPathEncode(reference);

            ITextDocument document;
            if (!_documentFactory.TryGetTextDocument(_view.TextDataModel.DocumentBuffer, out document))
                return DragDropPointerEffects.None;

            _view.TextBuffer.Insert(dragDropInfo.VirtualBufferPosition.Position.Position, string.Format(CultureInfo.CurrentCulture, MarkdownTemplate, Path.GetFileName(reference), reference, HttpUtility.HtmlAttributeEncode(reference)));

            return DragDropPointerEffects.Link;
        }

        public void HandleDragCanceled() { }
        public DragDropPointerEffects HandleDragStarted(DragDropInfo dragDropInfo) { return DragDropPointerEffects.Link; }
        public DragDropPointerEffects HandleDraggingOver(DragDropInfo dragDropInfo) { return DragDropPointerEffects.Link; }

        public bool IsDropEnabled(DragDropInfo dragDropInfo)
        {
            _imageFilename = FontDropHandler.GetImageFilename(dragDropInfo);

            if (string.IsNullOrEmpty(_imageFilename))
                return false;

            if (_imageExtensions.Contains(Path.GetExtension(_imageFilename)))
                return true;

            return false;
        }
    }
}

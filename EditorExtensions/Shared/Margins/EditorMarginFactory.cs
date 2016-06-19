using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using MadsKristensen.EditorExtensions.Svg;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.Margin
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name("MarginFactory")]
    [Order(After = PredefinedMarginNames.RightControl)]
    [MarginContainer(PredefinedMarginNames.Right)]
    [ContentType(SvgContentTypeDefinition.SvgContentType)]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]
    public sealed class MarginFactory : IWpfTextViewMarginProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        static readonly Dictionary<string, Func<ITextDocument, IWpfTextView, IWpfTextViewMargin>> marginFactories = new Dictionary<string, Func<ITextDocument, IWpfTextView, IWpfTextViewMargin>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Svg",               (document, sourceView) => new SvgMargin(document) },                                                             //? null : new TextViewMargin("JavaScript", document, sourceView) }
        };

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            Func<ITextDocument, IWpfTextView, IWpfTextViewMargin> creator;
            if (!marginFactories.TryGetValue(wpfTextViewHost.TextView.TextDataModel.DocumentBuffer.ContentType.TypeName, out creator))
                return null;

            ITextDocument document;

            if (!TextDocumentFactoryService.TryGetTextDocument(wpfTextViewHost.TextView.TextDataModel.DocumentBuffer, out document))
                return null;

            return creator(document, wpfTextViewHost.TextView);
        }
    }
}
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions.JSON
{
    class LogoLayer
    {
        public const string LayerName = "JSON Logo";

        [Export(typeof(AdornmentLayerDefinition))]
        [Name(LayerName)]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;
    }
}

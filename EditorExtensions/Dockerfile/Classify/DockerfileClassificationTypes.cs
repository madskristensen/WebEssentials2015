using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions
{
    public static class DockerfileClassificationTypes
    {
        public const string Keyword = "dockerfile_keyword";
        public const string Comment = "dockerfile_comment";

        [Export, Name(DockerfileClassificationTypes.Keyword)]
        public static ClassificationTypeDefinition DockerfileClassificationBold { get; set; }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = DockerfileClassificationTypes.Keyword)]
    [Name(DockerfileClassificationTypes.Keyword)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    internal sealed class DockerfileBoldFormatDefinition : ClassificationFormatDefinition
    {
        public DockerfileBoldFormatDefinition()
        {
            IsBold = true;
            DisplayName = "Dockerfile Keyword";
        }
    }
}

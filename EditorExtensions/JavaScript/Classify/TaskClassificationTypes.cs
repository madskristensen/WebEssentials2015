using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace MadsKristensen.EditorExtensions.JavaScript
{
    public static class TaskClassificationTypes
    {
        public const string Name = "task_keyword";

        [Export, Name(TaskClassificationTypes.Name)]
        public static ClassificationTypeDefinition TaskClassificationBold { get; set; }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TaskClassificationTypes.Name)]
    [Name(TaskClassificationTypes.Name)]
    [BaseDefinition(PredefinedClassificationTypeNames.String)]
    [Order(After = Priority.High)]
    [UserVisible(true)]
    internal sealed class TaskBoldFormatDefinition : ClassificationFormatDefinition
    {
        public TaskBoldFormatDefinition()
        {
            IsBold = true;
            //ForegroundBrush = Brushes.Orange;
            DisplayName = "Task Name";
        }
    }
}

using System.Linq;
using System.Windows;

namespace LSLCCEditor.Utility.Validation
{
    public static class ValidationTool
    {
        public static bool IsValid(this DependencyObject obj)
        {
            return !System.Windows.Controls.Validation.GetHasError(obj) &&
            LogicalTreeHelper.GetChildren(obj)
            .OfType<DependencyObject>()
            .All(IsValid);
        }
    }
}

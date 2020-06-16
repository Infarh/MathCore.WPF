using System.Windows;
using System.Windows.Media;
using MathCore.Trees;

namespace MathCore.WPF.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static TreeNode<DependencyObject> AsTreeNodeLogical(this DependencyObject obj) => 
            obj.AsTreeNode(VisualTreeHelperExtensions.GetLogicalChilds, LogicalTreeHelper.GetParent);

        public static TreeNode<DependencyObject> AsTreeNodeVisual(this DependencyObject obj) =>
            obj.AsTreeNode(VisualTreeHelperExtensions.GetVisualChilds, VisualTreeHelper.GetParent);
    }
}

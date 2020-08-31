using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Prototype
{
    public class AttachedBehaviors
    {
        public static readonly DependencyProperty EnableContextMenuProperty =
         DependencyProperty.RegisterAttached("EnableContextMenu", typeof(Boolean),
             typeof(DragDropBehavior), new FrameworkPropertyMetadata(OnEnableContextMenuChanged));

        public static void SetEnableContextMenu(DependencyObject element, Boolean value)
        {
            element.SetValue(EnableContextMenuProperty, value);
        }

        public static Boolean GetEnableContextMenu(DependencyObject element)
        {
            return (Boolean)element.GetValue(EnableContextMenuProperty);
        }

        public static void OnEnableContextMenuChanged
            (DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue == true)
            {
                var fe = (FrameworkElement)obj;
                fe.ContextMenuOpening += Fe_ContextMenuOpening;
            }
        }

        private static void Fe_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            if (fe == null) return;
            fe.ContextMenu = new ContextMenu();
            MenuItem mi;

            mi = new MenuItem();
            mi.Header = "Clear";
            mi.Command = new RelayCommand(x => ClearCommand(fe));
            fe.ContextMenu.Items.Add(mi);
        }

        private static void ClearCommand(FrameworkElement sender )
        {
            var node = sender.DataContext as Behaviortree.Node;
            if (node == null) return;
            node.Clear();
            var sv= sender.FindParent<ScrollViewer>();
            sv.InvalidateVisual();
        }
    }

    public class TreeViewHelper
    {
        private static Dictionary<DependencyObject, TreeViewSelectedItemBehavior> behaviors = new Dictionary<DependencyObject, TreeViewSelectedItemBehavior>();

        public static object GetSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewHelper), new UIPropertyMetadata(null, SelectedItemChanged));

        private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is TreeView))
                return;

            if (!behaviors.ContainsKey(obj))
                behaviors.Add(obj, new TreeViewSelectedItemBehavior(obj as TreeView));

            TreeViewSelectedItemBehavior view = behaviors[obj];
            view.ChangeSelectedItem(e.NewValue);
        }

        private class TreeViewSelectedItemBehavior
        {
            TreeView view;
            public TreeViewSelectedItemBehavior(TreeView view)
            {
                this.view = view;
                view.SelectedItemChanged += (sender, e) => SetSelectedItem(view, e.NewValue);
            }

            internal void ChangeSelectedItem(object p)
            {
                TreeViewItem item = (TreeViewItem)view.ItemContainerGenerator.ContainerFromItem(p);
                item.IsSelected = true;
            }
        }
    }
}

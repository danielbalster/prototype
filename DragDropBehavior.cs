using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Prototype
{
    static public class DragDropBehavior
    {
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached
        (
            "IsDragging",
            typeof(bool),
            typeof(DragDropBehavior),
            new UIPropertyMetadata(false)
        );

        public static bool GetIsDragging(DependencyObject source)
        {
            return (bool)source.GetValue(IsDraggingProperty);
        }

        public static void SetIsDragging(DependencyObject target, bool value)
        {
            target.SetValue(IsDraggingProperty, value);
        }

        public static TParent FindParent<TParent>(this DependencyObject child) where TParent : DependencyObject
        {
            DependencyObject current = child;
            while (current != null && !(current is TParent))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as TParent;
        }

        public static void SetParentValue<TParent>(this DependencyObject child, DependencyProperty property, object value) where TParent : DependencyObject
        {
            TParent parent = child.FindParent<TParent>();
            if (parent != null)
            {
                parent.SetValue(property, value);
            }
        }

        #region DragEnabled 
        public static readonly DependencyProperty IsDragSourceProperty =
        DependencyProperty.RegisterAttached("IsDragSource", typeof(Boolean),
            typeof(DragDropBehavior), new FrameworkPropertyMetadata(OnIsDragSourceChanged));

        public static void SetIsDragSource(DependencyObject element, Boolean value)
        {
            element.SetValue(IsDragSourceProperty, value);
        }

        public static Boolean GetIsDragSource(DependencyObject element)
        {
            return (Boolean)element.GetValue(IsDragSourceProperty);
        }

        public static void OnIsDragSourceChanged
            (DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue == true)
            {
                var fe = (FrameworkElement)obj;
                fe.PreviewMouseLeftButtonDown += Fe_PreviewMouseLeftButtonDown;
                fe.DragOver += Fe_DragOver;
                fe.DragEnter += Fe_DragEnter;
                fe.DragLeave += Fe_DragLeave;
                fe.PreviewMouseMove += Fe_PreviewMouseMove;
            }
        }

        private static void Fe_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var fe = sender as FrameworkElement;

            if (fe == null) return;
            var layer = AdornerLayer.GetAdornerLayer(fe);
            var adorner = new DropCursorAdorner(fe);
            layer.Add(adorner);

            DragDrop.DoDragDrop(fe,new object(), DragDropEffects.Copy);


            //layer.Remove(adorner);

        }
        #endregion
        #region DropTarget 
        public static readonly DependencyProperty DropTargetProperty =
        DependencyProperty.RegisterAttached("DropTarget", typeof(Boolean),
            typeof(DragDropBehavior), new FrameworkPropertyMetadata(OnDropTargetChanged));

        public static void SetDropTarget(DependencyObject element, Boolean value)
        {
            element.SetValue(DropTargetProperty, value);
        }

        public static Boolean GetDropTarget(DependencyObject element)
        {
            return (Boolean)element.GetValue(DropTargetProperty);
        }

        public static void OnDropTargetChanged
            (DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue == true)
            {
                var fe = (FrameworkElement)obj;
                fe.AllowDrop = true;
                fe.Drop += Fe_Drop;
            }
        }

        private static void Fe_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            e.Action = DragAction.Continue;
            e.Handled = true;
        }

        //PreviewDragEnter, PreviewDragOver and PreviewDragLeave

        private static void Fe_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private static void Fe_DragLeave(object sender, DragEventArgs e)
        {
        }

        private static void Fe_DragEnter(object sender, DragEventArgs e)
        {
        }

        private static void Fe_DragOver(object sender, DragEventArgs e)
        {
        }

        private static void Fe_Drop(object sender, DragEventArgs e)
        {
            DependencyObject source = e.OriginalSource as DependencyObject;
            if (source != null)
            {
                source.SetValue(DragDropBehavior.IsDraggingProperty, false);
            }
        }
        #endregion
    }
    }

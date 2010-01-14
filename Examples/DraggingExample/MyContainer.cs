using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using JSmith.Dragging;

namespace DraggingExample
{
    public class MyContainer : StackPanel, IDropTarget
    {
        #region IDropTarget Members

        public void OnDragStarted(UIElement dragSource)
        {
            Opacity = .6;
        }

        public void OnDragStopped(UIElement dragSource)
        {
            Opacity = 1;
        }

        public void OnDropTargetEnter(UIElement dragSource)
        {
            dragSource.Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 25 };
        }

        public void OnDropTargetLeave(UIElement dragSource)
        {
            dragSource.Effect = null;
        }

        public void OnDragSourceDropped(UIElement dragSource)
        {
            FrameworkElement element = (FrameworkElement)dragSource;

            Opacity = 1;
            Panel parent = (Panel)element.Parent;
            parent.Children.Remove(dragSource);

            element.Margin = new Thickness(5);
            element.RenderTransform = new TransformGroup();

            Children.Add(dragSource);
        }

        #endregion

    }
}

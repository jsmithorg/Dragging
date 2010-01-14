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

namespace JSmith.Dragging
{
    public interface IDropTarget
    {
        void OnDragStarted(UIElement dragSource);
        void OnDragStopped(UIElement dragSource);
        void OnDropTargetEnter(UIElement dragSource);
        void OnDropTargetLeave(UIElement dragSource);
        void OnDragSourceDropped(UIElement dragSource);
    
    }//end interface

}//end namespace
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JSmith.Dragging
{
    public static class Drop
    {
        #region Fields / Properties

        private static List<UIElement> _dropTargets;
        public static ReadOnlyCollection<UIElement> DropTargets { get; set; }

        private static List<UIElement> _notifiedTargets;

        #endregion

        #region Constructor

        static Drop()
        {
            _notifiedTargets = new List<UIElement>();
            _dropTargets = new List<UIElement>();
            DropTargets = new ReadOnlyCollection<UIElement>(_dropTargets);

        }//end constructor

        #endregion

        public static void RegisterDropTarget(UIElement element)
        {
            _dropTargets.Add(element);

        }//end method

        #region Utilities

        internal static void NotifyIntersectingDropTargets(UIElement element, Point intersectingPoint)
        {
            IEnumerable<UIElement> dropTargets = GetIntersectingDropTargets(intersectingPoint);
            List<UIElement> removableTargets = new List<UIElement>();

            foreach (UIElement dropTarget in _notifiedTargets)
            {
                if (!dropTargets.Contains(dropTarget) && dropTarget is IDropTarget)
                {
                    ((IDropTarget)dropTarget).OnDropTargetLeave(element);

                    removableTargets.Add(dropTarget);

                }//end if
            
            }//end foreach

            for (int i = 0; i < removableTargets.Count; i++)
                _notifiedTargets.Remove(removableTargets[i]);

            foreach (UIElement dropTarget in dropTargets)
            {
                if (dropTarget is IDropTarget && !_notifiedTargets.Contains(dropTarget))
                {
                    ((IDropTarget)dropTarget).OnDropTargetEnter(element);

                    _notifiedTargets.Add(dropTarget);

                }//end if

            }//end foreach

        }//end method

        internal static void NotifyIntersectingDropTargets(UIElement element, Point intersectingPoint, bool dropped)
        {
            if (!dropped)
            {
                NotifyIntersectingDropTargets(element, intersectingPoint);
                return;

            }//end if

            IEnumerable<UIElement> dropTargets = GetIntersectingDropTargets(intersectingPoint);

            foreach (UIElement dropTarget in dropTargets)
            {
                if (dropTarget is IDropTarget)
                {
                    ((IDropTarget)dropTarget).OnDragSourceDropped(element);

                    _notifiedTargets.Remove(dropTarget);

                }//end if

            }//end foreach

        }//end method

        internal static IEnumerable<UIElement> GetIntersectingDropTargets(Point intersectingPoint)
        {
            List<UIElement> targets = new List<UIElement>();

            for (int i = 0; i < _dropTargets.Count; i++)
            {
                UIElement element = _dropTargets[i];
                IEnumerable<UIElement> intersectingTargets = VisualTreeHelper.FindElementsInHostCoordinates(intersectingPoint, element);
                targets.AddRange(intersectingTargets);

            }//end for

            return targets;

        }//end method

        #endregion

    }//end class

}//end namespace
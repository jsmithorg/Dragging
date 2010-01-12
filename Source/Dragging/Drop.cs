﻿using System;
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
        private static List<UIElement> _dropTargets;
        public static ReadOnlyCollection<UIElement> DropTargets { get; set; }

        private static IEnumerable<UIElement> _lastDropTargets;

        static Drop()
        {
            _lastDropTargets = new List<UIElement>();
            _dropTargets = new List<UIElement>();
            DropTargets = new ReadOnlyCollection<UIElement>(_dropTargets);

        }//end constructor

        public static void RegisterDropTarget(UIElement element)
        {
            _dropTargets.Add(element);

        }//end method

        internal static void NotifyIntersectingDropTargets(UIElement element, Point intersectingPoint)
        {
            IEnumerable<UIElement> dropTargets = GetIntersectingDropTargets(intersectingPoint);

            foreach (UIElement dropTarget in _lastDropTargets)
                if (!dropTargets.Contains(dropTarget) && dropTarget is IDropTarget)
                    ((IDropTarget)dropTarget).OnDropTargetLeave(element);

            foreach (UIElement dropTarget in dropTargets)
                if (dropTarget is IDropTarget)
                    ((IDropTarget)dropTarget).OnDropTargetEnter(element);

            _lastDropTargets = dropTargets;

        }//end method

        internal static void NotifyIntersectingDropTargets(UIElement element, Point intersectingPoint, bool dropped)
        {
            IEnumerable<UIElement> dropTargets = GetIntersectingDropTargets(intersectingPoint);

            foreach (UIElement dropTarget in dropTargets)
                if (dropTarget is IDropTarget)
                    ((IDropTarget)dropTarget).OnDragSourceDropped(element);

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

    }//end class

}//end namespace
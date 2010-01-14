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
using System.Linq;
using System.Collections.ObjectModel;

namespace JSmith.Dragging
{
    public static class Drag
    {
        #region Fields / Properties

        public static UIElement Element { get; set; }
        public static Point? Offset { get; set; }
        
        private static Point? _lastMousePoint;
        private static bool _isDragging;

        #endregion

        #region Constructor

        static Drag()
        {
            _lastMousePoint = new Point();
            Offset = new Point();

        }//end constructor

        #endregion

        #region Start / Stop

        public static void Start(UIElement element)
        {
            if (Element != null)
                throw new Exception("Another element is being dragged.  Only one element at a time may be dragged.");

            Element = element;
            Element.CaptureMouse();
            Element.MouseMove += new MouseEventHandler(UIElement_MouseMove);

            TransformGroup tg = Element.RenderTransform as TransformGroup;
            if (tg == null)
            {
                tg = CreateTransformGroup();
                Element.RenderTransform = tg;

            }//end if

            TranslateTransform tt = tg.Children.Where(t => t is TranslateTransform).SingleOrDefault() as TranslateTransform;
            if (tt == null)
                tt = new TranslateTransform();

            tt.X = 0;
            tt.Y = 0;

            OnDragStarted(Element);

        }//end method

        public static void Start(UIElement element, Point elementOffset, Point mouseOffset)
        {
            Start(element);

            TransformGroup tg = (TransformGroup)Element.RenderTransform;
            TranslateTransform tt = (TranslateTransform)tg.Children.Where(t => t is TranslateTransform).Single();
            
            GeneralTransform gt = Application.Current.RootVisual.TransformToVisual(Element);
            Point offset = gt.Transform(new Point());

            //add our element offset to our mouse offset to get our final offset
            Offset = new Point(offset.X - mouseOffset.X, offset.Y - mouseOffset.Y);

            SetPosition(elementOffset);

            _isDragging = true;
            
        }//end method

        public static void Start(UIElement element, Point elementOffset)
        {
            Start(element, elementOffset, new Point());

        }//end method

        public static void Stop()
        {
            Element.MouseMove -= new MouseEventHandler(UIElement_MouseMove);

            Element.ReleaseMouseCapture();

            Drop.NotifyIntersectingDropTargets(Element, _lastMousePoint.Value, true);

            OnDragStopped(Element);

            Element = null;

        }//end method

        #endregion

        #region Utilities

        public static void SetPosition(Point point)
        {
            TransformGroup tg = (TransformGroup)Element.RenderTransform;
            TranslateTransform tt = (TranslateTransform)tg.Children.Where(t => t is TranslateTransform).Single();
            Point newPosition = new Point(point.X + Offset.Value.X, point.Y + Offset.Value.Y);

            tt.X = newPosition.X;
            tt.Y = newPosition.Y;

            Drop.NotifyIntersectingDropTargets(Element, point);

            _lastMousePoint = point;

        }//end method

        private static TransformGroup CreateTransformGroup()
        {
            return new TransformGroup
            {
                Children = new TransformCollection
                {
                    new TranslateTransform(),
                    new ScaleTransform(),
                    new SkewTransform()
                }
            };

        }//end method

        private static void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            SetPosition(e.GetPosition(null));

        }//end method

        internal static void OnDragStarted(UIElement element)
        {
            foreach (UIElement dropTarget in Drop.DropTargets)
                if (dropTarget is IDropTarget)
                    ((IDropTarget)dropTarget).OnDragStarted(element);

        }//end method

        internal static void OnDragStopped(UIElement element)
        {
            foreach (UIElement dropTarget in Drop.DropTargets)
                if (dropTarget is IDropTarget)
                    ((IDropTarget)dropTarget).OnDragStopped(element);

        }//end method

        #endregion

    }//end class

}//end namespace
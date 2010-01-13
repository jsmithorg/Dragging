using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using JSmith.Dragging;

namespace DraggingExample
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainPage mp = new MainPage();
            RootVisual = mp;

            mp.MyRect.MouseLeftButtonDown += new MouseButtonEventHandler(MyEllipse_MouseLeftButtonDown);

            mp.MyRect2.MouseLeftButtonDown += (s, ee) => Drag.Start((UIElement)s, ee.GetPosition(null), ee.GetPosition(mp.MyRect2));
            mp.MyRect2.MouseLeftButtonUp += (s, ee) => Drag.Stop();

            Drop.RegisterDropTarget(mp.DropTarget);
        }

        void MyEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TranslateTransform tt = new TranslateTransform();

            Rectangle el = new Rectangle
            {
                Width = 80,
                Height = 60,
                Opacity = 0,
                Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new ScaleTransform(),
                        tt,
                        new SkewTransform()
                    }
                }
            };
            el.MouseLeftButtonUp += new MouseButtonEventHandler(MyEllipse_MouseLeftButtonUp);
            el.Dispatcher.BeginInvoke(() =>
            {
                el.Opacity = .5;
                MainPage mp2 = (MainPage)RootVisual; 
                //GeneralTransform gt2 = mp2.MyRect.TransformToVisual(el);
                //Point p2 = gt2.Transform(new Point());
                
                //tt.X = e.GetPosition(mp2.MyRect).X + p2.X;
                //tt.Y = e.GetPosition(mp2.MyRect).Y + p2.Y;

                Point mouseOffset = e.GetPosition(mp2.MyRect);
                Point dragOffset = new Point((mouseOffset.X / 100) * 80, (mouseOffset.Y / 80) * 60);

                //GeneralTransform gt = mp2.MyRect.TransformToVisual(el);
                //Point p = gt.Transform(mouseOffset);

                Drag.Start(el, e.GetPosition(null), new Point { X = dragOffset.X, Y = dragOffset.Y });
            });

            MainPage mp = (MainPage)RootVisual;
            mp.LayoutRoot.Children.Add(el);

            //Point mouseOffset = e.GetPosition(mp.MyRect);
            //Point dragOffset = new Point((mouseOffset.X / 100) * 80, (mouseOffset.Y / 80) * 60);
            //Drag.Start(el, e.GetPosition(null), new Point { X = dragOffset.X, Y = dragOffset.Y });
        }

        void MyEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = Drag.Element;
            Drag.Stop();

            if (((TransformGroup)element.RenderTransform).Children.Count < 1)
                return;

            TranslateTransform tt = (TranslateTransform)((TransformGroup)element.RenderTransform).Children.Where(t => t is TranslateTransform).Single();

            //the difference between the root (0,0) and the host object
            //GeneralTransform gt = RootVisual.TransformToVisual(((MainPage)RootVisual).MyRect);
            //Point p = gt.Transform(new Point());

            //the difference between the host object and the dragging object
            GeneralTransform gt = ((MainPage)RootVisual).MyRect.TransformToVisual(element);
            Point p = gt.Transform(new Point());

            DoubleAnimation da = new DoubleAnimation
            {
                To = tt.X + p.X,
                Duration = TimeSpan.FromSeconds(.6),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut, Power = 4 }
            };

            Storyboard.SetTarget(da, tt);
            Storyboard.SetTargetProperty(da, new PropertyPath("X"));

            DoubleAnimation da2 = new DoubleAnimation
            {
                To = tt.Y + p.Y,
                Duration = TimeSpan.FromSeconds(.6),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut, Power = 4 }
            };

            Storyboard.SetTarget(da2, tt);
            Storyboard.SetTargetProperty(da2, new PropertyPath("Y"));

            Storyboard sb = new Storyboard();
            sb.Children.Add(da);
            sb.Children.Add(da2);
            sb.Completed += (s, ee) => ((MainPage)RootVisual).LayoutRoot.Children.Remove(element);
            sb.Begin();

        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}

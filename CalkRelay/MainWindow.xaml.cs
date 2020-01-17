using CalkRelay.BaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CalkRelay
{

    public partial class MainWindow
    {
        //The boolean that signifys when an output is being linked
        private bool _linkingStarted = false;
        //The temporary line that shows when linking an output
        private LineGeometry _tempLink;
        //The output that is being linked to
        private ConnectionPoint _tempOutput;

        public MainWindow()
        {
            this.InitializeComponent();

            CircuitCanvas.Drop += CircuitCanvas_Drop;
            CircuitCanvas.MouseDown += CircuitCanvas_MouseDown;
            CircuitCanvas.MouseMove += CircuitCanvas_MouseMove;
            CircuitCanvas.MouseUp += CircuitCanvas_MouseUp;
        }

        private void ObjectSelector_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ObjectSelector.SelectedItem == null)
                return;

            DragDrop.DoDragDrop(ObjectSelector, ObjectSelector.SelectedItem, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void CircuitCanvas_Drop(object sender, DragEventArgs e)
        {
            String[] allFormats = e.Data.GetFormats();
            if (allFormats.Length == 0)
                return;

            string ItemType = allFormats[0];

            CircuitObject instance = (CircuitObject)Assembly.GetExecutingAssembly().CreateInstance(ItemType);

            if (instance == null)
                return;

            CircuitCanvas.Children.Add(instance);

            Point p = e.GetPosition(CircuitCanvas);

            Canvas.SetLeft(instance, p.X - 15);
            Canvas.SetTop(instance, p.Y - 15);
        }
      
        private void CircuitCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point MousePosition = e.GetPosition(CircuitCanvas);

            HitTestResult result = VisualTreeHelper.HitTest(CircuitCanvas, MousePosition);

            if (result == null || result.VisualHit == null)
                return;

            if (result.VisualHit is Border)
            {
                Border border = (Border)result.VisualHit;
                var IO = border.Parent;

                if (IO is ConnectionPoint)
                {
                    ConnectionPoint IOOutput = (ConnectionPoint)IO;

                    //Get the center of the output relative to the canvas
                    Point position = IOOutput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOOutput.ActualWidth / 2, IOOutput.ActualHeight / 2));

                    //Creates a new line
                    _linkingStarted = true;
                    _tempLink = new LineGeometry(position, position);

                    //Assign it to the list of connections to be displayed
                    Connections.Children.Add(_tempLink);

                    //Assign the temporary output to the current output
                    _tempOutput = (ConnectionPoint)IO;

                    e.Handled = true;
                }
            }
        }

        private void CircuitCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_linkingStarted)
            {
                _tempLink.EndPoint = e.GetPosition(CircuitCanvas);
                e.Handled = true;
            }
        }

        private void CircuitCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_linkingStarted)
            {
                bool linked = false;

                var BaseType = e.Source.GetType().BaseType;

                if (BaseType == typeof(CircuitObject))
                {
                    CircuitObject obj = (CircuitObject)e.Source;
                    Point MousePosition = e.GetPosition(obj);
                    HitTestResult result = VisualTreeHelper.HitTest(obj, MousePosition);

                    if (result == null || result.VisualHit == null)
                    {
                        Connections.Children.Remove(_tempLink);
                        _tempLink = null;
                        _linkingStarted = false;
                        return;
                    }

                    if (result.VisualHit is Border)
                    {
                        Border border = (Border)result.VisualHit;
                        var IO = border.Parent;

                        if (IO is ConnectionPoint)
                        {
                            ConnectionPoint IOInput = (ConnectionPoint)IO;
                            Point inputPoint = IOInput.TransformToAncestor(CircuitCanvas).Transform(new Point(IOInput.ActualWidth / 2, IOInput.ActualHeight / 2));
                            _tempLink.EndPoint = inputPoint;

                            //Adding to the input and output lines
                            obj.AttachInputLine(_tempLink);
                            ((CircuitObject)((Grid)_tempOutput.Parent).Parent).AttachOutputLine(_tempLink);

                            linked = true;
                        }
                    }
                }

                //If it isn't linked remove the temporary link
                if (!linked)
                {
                    Connections.Children.Remove(_tempLink);
                    _tempLink = null;
                }
                
                _linkingStarted = false;
                e.Handled = true;
            }
        }

    }
}

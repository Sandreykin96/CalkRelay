using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CalkRelay.BaseObjects
{
    public class CircuitObject : UserControl
    {
        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register("CanMove", typeof(bool), typeof(CircuitObject), new PropertyMetadata(true));

        public bool CanMove
        {
            get { return (bool)GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        //The anchor point of the object when being moved
        private Point _anchorPoint;

        //The current location of the point
        private Point _currentPoint;

        //The transformer that will change the position of the object
        private TranslateTransform _transform = new TranslateTransform();

        //Boolean to check if the object is being dragged
        private bool _isInDrag = false;

        //The lines being connected to the input
        private List<LineGeometry> _attachedInputLines;

        //The lines being connected to the output
        private List<LineGeometry> _attachedOutputLines;

        public CircuitObject()
        {
            //Set the events for the object
            this.MouseLeftButtonDown += DragObject_MouseLeftButtonDown;
            this.MouseMove += DragObject_MouseMove;
            this.MouseLeftButtonUp += DragObject_MouseLeftButtonUp;

            _attachedInputLines = new List<LineGeometry>();
            _attachedOutputLines = new List<LineGeometry>();
        }

        private void DragObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanMove == false)
                return;

            var x = Mouse.DirectlyOver;

            if (x is Border)
                return;

            var element = sender as FrameworkElement;

            _anchorPoint = e.GetPosition(null);
            _isInDrag = true;

            element.CaptureMouse();
            e.Handled = true;
        }

        private void DragObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isInDrag)
            {
                _isInDrag = false;
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void DragObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isInDrag)
            {
                var element = sender as FrameworkElement;
                _currentPoint = e.GetPosition(null);

                //Transform the element based off the last position
                _transform.X += _currentPoint.X - _anchorPoint.X;
                _transform.Y += _currentPoint.Y - _anchorPoint.Y;

                //Transform connected lines
                foreach (LineGeometry attachedLine in _attachedInputLines)
                {
                    attachedLine.EndPoint = MoveLine(attachedLine.EndPoint,
                                                     (_currentPoint.X - _anchorPoint.X),
                                                     (_currentPoint.Y - _anchorPoint.Y));
                }

                foreach (LineGeometry attachedLine in _attachedOutputLines)
                {
                    attachedLine.StartPoint = MoveLine(attachedLine.StartPoint,
                                                     (_currentPoint.X - _anchorPoint.X),
                                                     (_currentPoint.Y - _anchorPoint.Y));
                }

                //Transform the elements location
                this.RenderTransform = _transform;
                //Update the anchor point
                _anchorPoint = _currentPoint;
            }
        }

        private Point MoveLine(Point PointToMove, double AmountToMoveX, double AmountToMoveY)
        {
            Point transformedPoint = new Point();
            transformedPoint.X = PointToMove.X + AmountToMoveX;
            transformedPoint.Y = PointToMove.Y + AmountToMoveY;
            return transformedPoint;
        }

        public void AttachInputLine(LineGeometry line)
        {
            _attachedInputLines.Add(line);
        }
        public void AttachOutputLine(LineGeometry line)
        {
            _attachedOutputLines.Add(line);
        }

    }
}

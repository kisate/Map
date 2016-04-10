using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMap.ViewModels
{
    sealed class MapPanZoomer
    {
        private const double ZOOMSTEP = 0.6;        

        private double _zoomFactor;
        private Point _offset;

        /// <summary>
        /// Keeps pixelsize of map drawing area
        /// </summary>
        private Size _mapSize;

        public Size ImageSize { get; set; }

        /// <summary>
        /// sets/gets the initial point for panning
        /// </summary>
        public Point PanOrigin { get; set; }

        public MapPanZoomer()
        {
            Reset();
        }

        public Matrix GetMatrix()
        {
            var matrix = new Matrix(_zoomFactor, 0, 0, _zoomFactor,
                _offset.X, _offset.Y);
            return matrix;
        }

        private void Reset()
        {
            _zoomFactor = 1;            
            _offset = new Point();
        }

        /// <summary>
        /// Resizes map screen size
        /// </summary>
        /// <param name="mapScreenSize"></param>
        public void ResizeMap(Size mapScreenSize)
        {
            _mapSize = mapScreenSize;
            Reset();
        }

        /// <summary>
        /// shift offset
        /// </summary>
        /// <param name="cursorPoint"></param>
        public void Pan(Point cursorPoint)
        {
            var x = _offset.X - PanOrigin.X + cursorPoint.X;
            var y = _offset.Y - PanOrigin.Y + cursorPoint.Y;
            ValidateOffset(x, y);
        }

        /// <summary>
        /// Zooms in or out at the current mouse cursor point. 
        /// Transformation matrix is rebuilt by increasing or decreasing _zoomFactor
        /// </summary>
        /// <param name="zoomIn"></param>
        /// <param name="cursorPoint"></param>
        public void Zoom(bool zoomIn, Point cursorPoint)
        {
            // if wheel scrolled in positive direction, zoomFactor is increased
            // otherwise it is decreased, but not smaller than zoomFactor == 1
            var delta = zoomIn ?
                ZOOMSTEP : (_zoomFactor - 1.0) > ZOOMSTEP / 2 ?
                    -ZOOMSTEP : 0;

            // calculating matrix components
            if (delta != 0)
            {
                _zoomFactor += delta;

                // if zoomfactor is 1.0, no offset (so the image is stretched into control bounds
                if (Math.Abs(_zoomFactor - 1.0) < ZOOMSTEP / 2)
                {
                    _offset = new Point();
                }
                else
                {
                    // new offset is shifted by 
                    // (new_virtual_radius_vector - old_virtual_radius_vector) = delta*physical_radius_vector
                    // this is needed so sooming is going under cursor, 
                    // i.e. old real coords are equal to new real coords
                    // as well as old virtual coords are equal to new virtual coords
                    double
                        x = _offset.X - delta * cursorPoint.X,
                        y = _offset.Y - delta * cursorPoint.Y;

                    ValidateOffset(x, y);
                }
            }
        }

        /// <summary>
        /// Translate image coords to screen coords
        /// </summary>
        /// <param name="imageCoords"></param>
        /// <returns></returns>
        public Point TranslatePoint(Point imageCoords)
        {
            // translate to picture size
            var x = imageCoords.X / ImageSize.Width * _mapSize.Width;
            var y = imageCoords.Y / ImageSize.Height * _mapSize.Height;
            // transform by matrix
            var x1 = _zoomFactor * x + _offset.X;
            var y1 = _zoomFactor * y + _offset.Y;

            return new Point(x1, y1);
        }

        /// <summary>
        /// Translate screen coords to image coords
        /// </summary>
        /// <param name="imageCoords"></param>
        /// <returns></returns>
        public Point TranslatePointBack(Point imageCoords)
        {
            // transform by matrix
            //var r1 = (imageCoords - _offset) / _zoomFactor;
            var r1 = imageCoords;

            // translate to image size
            var x = r1.X * ImageSize.Width / _mapSize.Width;
            var y = r1.Y * ImageSize.Height / _mapSize.Height;

            return new Point(x, y);
        }
        

        /// <summary>
        /// check that edge points fit into the screen
        /// offset can't be positive - that means TopLeft is inside screen
        /// if offset is negative - check that BR is not inside PictureBox.
        /// Updates offset
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ValidateOffset(double x, double y)
        {
            if (x >= 0) { x = 0; }
            else
            {
                var minX = _mapSize.Width * (1.0D - _zoomFactor);
                if (x < minX)
                {
                    x = minX;
                }
            }

            if (y >= 0) { y = 0; }
            else
            {
                var minY = _mapSize.Height * (1.0D - _zoomFactor);
                if (y < minY)
                {
                    y = minY;
                }
            }

            _offset = new Point(x, y);
        }

        /// <summary>
        /// Returns X as relative to a width of a map
        /// </summary>
        /// <param name="xScreen"></param>
        /// <returns></returns>
        public double GetRelativeX(double xScreen)
        {
            return _mapSize.Width != 0 ? xScreen / _mapSize.Width : 0;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Prototype
{
    static class Helper
    {
        public static object FindResource(string key)
        {
            foreach(Window window in App.Current.Windows)
            {
                var res = window.TryFindResource(key);
                if (res != null)
                    return res;
            }
            return null;
        }

        public const double DegreesToRadians = 0.01745329252;

        static public Matrix3D GetProjectionMatrix(double near, double far, double fov, double aspectRatio)
        {
            double hFoV = DegreesToRadians * fov;
            double xScale = 1 / Math.Tan(hFoV / 2);
            double yScale = aspectRatio * xScale;
            double m33 = (far == double.PositiveInfinity) ? -1 : (far / (near - far));
            double m43 = near * m33;
            return new Matrix3D(
                xScale, 0, 0, 0,
                0, yScale, 0, 0,
                0, 0, m33, -1,
                0, 0, m43, 0
            );
        }

        // is any of the four ``bounds´´ corners in the circle around crosshair with radius?
        static public bool TouchesCircle(Rect bounds, Point crosshair, double radius)
        {
            return (((bounds.BottomLeft - crosshair).Length < radius) ||
                ((bounds.BottomRight - crosshair).Length < radius) ||
                ((bounds.TopLeft - crosshair).Length < radius) ||
                ((bounds.TopRight - crosshair).Length < radius));
        }

        static public bool InsideCircle(Rect bounds, Point crosshair, double radius)
        {
            return (((bounds.BottomLeft - crosshair).Length < radius) &&
                ((bounds.BottomRight - crosshair).Length < radius) &&
                ((bounds.TopLeft - crosshair).Length < radius) &&
                ((bounds.TopRight - crosshair).Length < radius));
        }
    }
}

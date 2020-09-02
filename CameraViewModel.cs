using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Prototype
{
    public class CameraViewModel : ViewModelBase<MatrixCamera>
    {
        double zoom = 20;
        double pitch = -60;
        double yaw = 0;

        Vector position = new Vector(0, 0);

        double near = 0.01;
        double far = 5000;
        double fov = 90;
        double aspectRatio = 1;

        #region Zoom
        public double Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                if (zoom != value)
                {
                    zoom = value;
                    RaisePropertyChanged("Zoom");
                }
            }
        }
        #endregion
        #region Pitch
        public double Pitch
        {
            get
            {
                return pitch;
            }
            set
            {
                if (pitch != value)
                {
                    pitch = value;
                    RaisePropertyChanged("Pitch");
                }
            }
        }
        #endregion
        #region Yaw
        public double Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                if (yaw != value)
                {
                    yaw = value;
                    RaisePropertyChanged("Yaw");
                }
            }
        }
        #endregion
        #region Position
        public Vector Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position != value)
                {
                    position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }
        #endregion
        #region Near
        public double Near
        {
            get
            {
                return near;
            }
            set
            {
                if (near != value)
                {
                    near = value;
                    updateProjection();
                    RaisePropertyChanged("Near");
                }
            }
        }
        #endregion
        #region Far
        public double Far
        {
            get
            {
                return far;
            }
            set
            {
                if (far != value)
                {
                    far = value;
                    updateProjection();
                    RaisePropertyChanged("Far");
                }
            }
        }
        #endregion
        #region FOV
        public double FOV
        {
            get
            {
                return fov;
            }
            set
            {
                if (fov != value)
                {
                    fov = value;
                    updateProjection();
                    RaisePropertyChanged("FOV");
                }
            }
        }
        #endregion
        #region AspectRatio
        public double AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                if (aspectRatio != value)
                {
                    aspectRatio = value;
                    updateProjection();
                    RaisePropertyChanged("AspectRatio");
                }
            }
        }
        #endregion

        private void updateProjection()
        {
            Model.ProjectionMatrix = Helper.GetProjectionMatrix(near, far, fov, aspectRatio);
        }

        public void Update(State state)
        {
            var ry = -(state.Gamepad.RightThumbX / 32768.0);
            var rx = (state.Gamepad.RightThumbY / 32768.0);
            var ly = (state.Gamepad.LeftThumbY / 32768.0) * 5.0;
            var lx = (state.Gamepad.LeftThumbX / 32768.0) * 3.0;

            // remove noise
            if (ly > -0.5 && ly < +0.5) ly = 0.0;
            if (lx > -0.5 && lx < +0.5) lx = 0.0;
            if (ry > -0.5 && ry < +0.5) ry = 0.0;
            if (rx > -0.5 && rx < +0.5) rx = 0.0;

            // apply
            var z = zoom - ly;
            if (z < 5) z = 5;
            else if (z > 1000) z = 1000;
            Zoom = z;

            var y = yaw + lx;
            if (y < 0.0) y += 360.0;
            if (y > 360.0) y -= 360.0;
            Yaw = y;

            // increase move speed with distance
            rx *= Math.Log10(Zoom);
            ry *= Math.Log10(Zoom);

            // transform
            var rad = yaw * Helper.DegreesToRadians;
            var sin = Math.Sin(rad);
            var cos = Math.Cos(rad);
            var pitch_sin = Zoom * Math.Sin(Helper.DegreesToRadians * -pitch);
            var pitch_cos = Zoom * Math.Cos(Helper.DegreesToRadians * -pitch);

            // this is the position of the camera. it's moved oriented to the viewing direction
            Position -= new Vector(rx * sin + ry * cos, rx * cos - ry * sin);

            // where is the camera
            var m1 = new TranslateTransform3D(position.X, 0, position.Y);

            // rotation is applied around Y-axis
            var m2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), yaw));

            // we zoom back and forth along the viewing direction
            var m3 = new TranslateTransform3D(new Vector3D(0, pitch_sin, pitch_cos));
            var m4 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), pitch));

            var v = m4.Value * m3.Value * m2.Value * m1.Value;
            v.Invert();
            Model.ViewMatrix = v;
        }

    }
}

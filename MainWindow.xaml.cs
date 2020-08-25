using System;
using System.Collections.Generic;
using System.Linq;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Windows.Media.Media3D;
using SharpDX.XInput;


namespace Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            InitializeComponent();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Controller = new ControllerViewModel();
            World = new WorldViewModel(world);
            UnitsEditor = new UnitsEditorViewModel(world);
            BehaviortreesEditor = new BehaviortreesEditorViewModel(world);

            world.Init();
            DataContext = this;

            SetupRender();

            CompositionTarget.Rendering += OnRender;
        }

        World world = new World();

        public WorldViewModel World
        {
            get;set;
        }

        public UnitsEditorViewModel UnitsEditor
        {
            get; private set;
        }

        public BehaviortreesEditorViewModel BehaviortreesEditor
        {
            get; private set;
        }

        public ControllerViewModel Controller
        {
            get;
            set;
        }

        State lastState;
        void CheckButton(Image image, GamepadButtonFlags bits, GamepadButtonFlags last, GamepadButtonFlags now)
        {
            if ((bits & last) != (bits & now))
            {
                image.Visibility = ((now & bits) == bits) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        void DisplayController()
        {
            /*
            if (!Controller.Model.IsConnected) return;
            // just a test
            var state = Controller.Model.GetState();

            var last = lastState.Gamepad.Buttons;
            var now = state.Gamepad.Buttons;

            lastState = state;
            */
        }
        const double DegreesToRadians = 0.01745329252;

        Matrix3D GetProjectionMatrix(double aspectRatio)
        {
            double hFoV = DegreesToRadians * Camera.FieldOfView;
            double zn = Camera.NearPlaneDistance;
            double zf = Camera.FarPlaneDistance;
            double xScale = 1 / Math.Tan(hFoV / 2);
            double yScale = aspectRatio * xScale;
            double m33 = (zf == double.PositiveInfinity) ? -1 : (zf / (zn - zf));
            double m43 = zn * m33;
            return new Matrix3D(
                xScale, 0, 0, 0,
                     0, yScale, 0, 0,
                     0, 0, m33, -1,
                     0, 0, m43, 0);
        }

        Material selected = new DiffuseMaterial { Brush = Brushes.Orange };
        Dictionary<UnitTypes, Material> materials = new Dictionary<UnitTypes, Material>();

        private void SetupRender()
        {
            materials[UnitTypes.Animal] = new DiffuseMaterial { Brush = Brushes.Gray };
            materials[UnitTypes.Builder] = new DiffuseMaterial { Brush = Brushes.Green };
            materials[UnitTypes.Carrier] = new DiffuseMaterial { Brush = Brushes.Blue };
            materials[UnitTypes.Leader] = new DiffuseMaterial { Brush = Brushes.Red };
            materials[UnitTypes.Military] = new DiffuseMaterial { Brush = Brushes.Yellow };
            materials[UnitTypes.Worker] = new DiffuseMaterial { Brush = Brushes.Magenta };

            var odd = new GeometryModel3D();
            odd.Geometry = MeshFactory.CreateCheckerboard(10, 50, 0);
            odd.Material = new DiffuseMaterial { Brush = Brushes.LightCyan };
            gizmos.Children.Add(odd);
            var even = new GeometryModel3D();
            even.Geometry = MeshFactory.CreateCheckerboard(10, 50, 1);
            even.Material = new DiffuseMaterial { Brush = Brushes.LightBlue };
            gizmos.Children.Add(even);
        }

        private void OnRender(object sender, EventArgs e)
        {
            Controller.Update();
            DisplayController();
            World.Update();
            UpdateCamera();

            wireframe.Children.Clear();
            Models.Children.Clear();

            var crosshair = new Point(wireframe.ActualWidth/2, wireframe.ActualHeight/2);

            foreach (var uvm in UnitsEditor.Units)
            {
                uvm.ModelVisual3D.Transform = new TranslateTransform3D(uvm.X, 0, uvm.Y);
                uvm.GeometryModel3D.Material = uvm.Selected ? selected : materials[uvm.Type];
                Models.Children.Add(uvm.ModelVisual3D);

                GeneralTransform3DTo2D transform = uvm.ModelVisual3D.TransformToAncestor(Viewport);
                if (transform != null)
                {
                    Rect bounds = transform.TransformBounds(VisualTreeHelper.GetDescendantBounds(uvm.ModelVisual3D));
                    if (!bounds.IsEmpty)
                    {
                        if (cb_crosshair.IsChecked.Value)
                        {
                            uvm.Selected = (bounds.Contains(crosshair));
                        }
                        if (cb_selectioncircle.IsChecked.Value)
                        {
                            uvm.Selected = TouchesCircle(bounds, crosshair, sl_size.Value/2);
                        }

                        if(cb_showbounds.IsChecked.Value)
                        {
                            var p = new Rectangle
                            {
                                Stroke = Brushes.Blue,
                                StrokeThickness = 0.25,
                                Width = bounds.Width,
                                Height = bounds.Height
                            };
                            Canvas.SetLeft(p, bounds.Left);
                            Canvas.SetTop(p, bounds.Top);
                            wireframe.Children.Add(p);
                        }
                    }
                }
            }
        }

        // is any of the four ``bounds´´ corners in the circle around crosshair with radius?
        private bool TouchesCircle(Rect bounds, Point crosshair, double radius)
        {
            return (((bounds.BottomLeft - crosshair).Length < radius) ||
                ((bounds.BottomRight - crosshair).Length < radius) ||
                ((bounds.TopLeft - crosshair).Length < radius) ||
                ((bounds.TopRight - crosshair).Length < radius));
        }

        private bool InsideCircle(Rect bounds, Point crosshair, double radius)
        {
            return (((bounds.BottomLeft - crosshair).Length < radius) &&
                ((bounds.BottomRight - crosshair).Length < radius) &&
                ((bounds.TopLeft - crosshair).Length < radius) &&
                ((bounds.TopRight - crosshair).Length < radius));
        }

        void OnTimerTick(object sender, EventArgs e)
        {
        }

        Vector position = new Vector(0,-1);
        public float aboveGround { get; set; } = 20;
        public float pitch { get; set; } = -60;
        float orientation = 0.0f;

        private void UpdateCamera()
        {
            if (!Controller.Model.IsConnected) return;
            var state = Controller.Model.GetState();
            int x = state.Gamepad.RightThumbX / 16384;
            int y = state.Gamepad.RightThumbY / 16384;

            if (state.Gamepad.LeftTrigger > 0)
            {
                orientation += 3.0f;
                if (orientation < 0.0f) orientation += 360.0f;
            }
            if (state.Gamepad.RightTrigger > 0)
            {
                orientation -= 3.0f;
                if (orientation > 360.0f) orientation -= 360.0f;
            }

            double rad = orientation * DegreesToRadians;
            double sin = Math.Sin(rad);
            double cos = Math.Cos(rad);
            float fx = -y;
            float fy = x;

            position -= new Vector(fx * sin + fy * cos, fx * cos - fy * sin);

            Camera.LookDirection = new Vector3D(Math.Sin(rad), pitch* DegreesToRadians , Math.Cos(rad));
            Camera.Position = new Point3D(position.X, aboveGround, position.Y);
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lb = sender as ListBox;
            if (lb == null) return;
            var uvm = (lb.SelectedValue as UnitViewModel);
            if (uvm == null) return;
            var root = new Behaviortree.Decorator { uvm.Model.Behavior.Root };
            var dlg = new Behaviortree.Editor.BehaviortreeEditor { Behavior = root };
            dlg.ShowDialog();
            uvm.Model.Behavior.Root = root[0];
            root.Clear();
            dlg.Behavior = null;
        }
    }
}

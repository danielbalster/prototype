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
    public enum HitTestModes
    {
        Full,
        Center,
        Partial
    }
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
            Camera = new CameraViewModel { Model = camera };
            Camera.AspectRatio = Viewport.ActualWidth / Viewport.ActualHeight;

            world.Init();
            DataContext = this;

            SetupRender();

            Viewport.MouseUp += Viewport_MouseUp;

            CompositionTarget.Rendering += OnRender;
        }

        public HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;
            if (rayResult == null) return HitTestResultBehavior.Continue;
            RayMeshGeometry3DHitTestResult rayMeshResult = rayResult as RayMeshGeometry3DHitTestResult;
            if (rayMeshResult == null) return HitTestResultBehavior.Continue;
            GeometryModel3D hitgeo = rayMeshResult.ModelHit as GeometryModel3D;

            if (!(hitgeo == planeOdd || hitgeo == planeEven)) return HitTestResultBehavior.Continue;

            var unit = new Unit { World = World.Model };
            unit.Position = new Vector(rayResult.PointHit.X, rayResult.PointHit.Z); ;
            unit.Selected = false;
            unit.Type = UnitTypes.Leader;
            unit.Behavior = null;

            World.Model.PendingActions.Add((world) => {
                world.Units.Add(unit);

            });


            return HitTestResultBehavior.Stop;
        }

        private void Viewport_MouseUp(object sender, MouseButtonEventArgs args)
        {
            var viewport = sender as Viewport3D;
            var mouseposition = args.GetPosition(viewport);
            var testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            var testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            var pointparams = new PointHitTestParameters(mouseposition);
            var rayparams = new RayHitTestParameters(testpoint3D, testdirection);
            VisualTreeHelper.HitTest(viewport, null, HTResult, pointparams);
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

        public HitTestModes HitTestMode { get; set; }

        public CameraViewModel Camera { get; set; }

        void CheckButton(Image image, GamepadButtonFlags bits, GamepadButtonFlags last, GamepadButtonFlags now)
        {
            if ((bits & last) != (bits & now))
            {
                image.Visibility = ((now & bits) == bits) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        void DisplayController()
        {

        }
 
        Material selectionmaterial = new DiffuseMaterial { Brush = Brushes.Orange };
        Material highlightmaterial = new DiffuseMaterial { Brush = Brushes.LavenderBlush };
        Dictionary<UnitTypes, Material> materials = new Dictionary<UnitTypes, Material>();

        private void SetupRender()
        {
            materials[UnitTypes.Animal] = new DiffuseMaterial { Brush = Brushes.Gray };
            materials[UnitTypes.Builder] = new DiffuseMaterial { Brush = Brushes.Green };
            materials[UnitTypes.Carrier] = new DiffuseMaterial { Brush = Brushes.Blue };
            materials[UnitTypes.Leader] = new DiffuseMaterial { Brush = Brushes.Red };
            materials[UnitTypes.Military] = new DiffuseMaterial { Brush = Brushes.Yellow };
            materials[UnitTypes.Worker] = new DiffuseMaterial { Brush = Brushes.Magenta };

            planeOdd = new GeometryModel3D();
            planeOdd.Geometry = MeshFactory.CreateCheckerboard(5, 500, 0);
            planeOdd.Material = new DiffuseMaterial { Brush = Brushes.LightCyan };
            gizmos.Children.Add(planeOdd);
            planeEven = new GeometryModel3D();
            planeEven.Geometry = MeshFactory.CreateCheckerboard(5, 500, 1);
            planeEven.Material = new DiffuseMaterial { Brush = Brushes.LightBlue };
            gizmos.Children.Add(planeEven);

            var cyl = new GeometryModel3D();
            cyl.Geometry = MeshFactory.CreateCylinder(1, 1, 36);
            var blk = new SolidColorBrush(Colors.Black);
            blk.Opacity = 0.2;
            cyl.Material = cyl.BackMaterial = new DiffuseMaterial { Brush = blk };
            selectionCylinder = new ModelVisual3D { Content = cyl };
        }
        ModelVisual3D selectionCylinder;
        GeometryModel3D planeOdd;
        GeometryModel3D planeEven;

        private void OnRender(object sender, EventArgs e)
        {
            Controller.Update();

            if (Controller.IsConnected)
            {
                var state = Controller.Model.GetState();
                Camera.Update(state);
                World.Model.CameraPosition = Camera.Position;
            }

            World.Update();

            wireframe.Children.Clear();
            Models.Children.Clear();
            var crosshair = new Point(wireframe.ActualWidth/2, wireframe.ActualHeight/2);

            World.HighlightedUnits.Clear();
            foreach (var uvm in UnitsEditor.Units)
            {
                uvm.ModelVisual3D.Transform = new TranslateTransform3D(uvm.X, 0, uvm.Y);
                Models.Children.Add(uvm.ModelVisual3D);

                var selected = false;
                GeneralTransform3DTo2D transform = uvm.ModelVisual3D.TransformToAncestor(Viewport);
                if (transform != null)
                {
                    Rect bounds = transform.TransformBounds(VisualTreeHelper.GetDescendantBounds(uvm.ModelVisual3D));
                    if (!bounds.IsEmpty)
                    {
                        if (cb_crosshair.IsChecked.Value)
                        {
                            selected = (bounds.Contains(crosshair));
                        }
                        if (cb_selectioncircle.IsChecked.Value)
                        {
                            switch(HitTestMode)
                            {
                                case HitTestModes.Full:
                                    selected = Helper.InsideCircle(bounds, crosshair, sl_size.Value / 2);
                                    break;
                                case HitTestModes.Center:
                                    var center = new Point(bounds.Left + bounds.Width / 2, bounds.Top+bounds.Height / 2);
                                    selected = (center - crosshair).Length < (sl_size.Value / 2);
                                    break;
                                case HitTestModes.Partial:
                                    selected = Helper.TouchesCircle(bounds, crosshair, sl_size.Value / 2);
                                    break;
                            }
                        }
                        if (cb_wscircle.IsChecked.Value)
                        {
                            var agentRadius = 1;
                            var extend = 0;
                            switch (HitTestMode)
                            {
                                case HitTestModes.Full:
                                    extend = +agentRadius;
                                    break;
                                case HitTestModes.Center:
                                    break;
                                case HitTestModes.Partial:
                                    extend = -agentRadius;
                                    break;
                            }

                            selected = ((uvm.Position - Camera.Position).Length+extend) < (sl_size.Value/20);
                        }

                        if (cb_showbounds.IsChecked.Value)
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

                uvm.GeometryModel3D.Material = uvm.Selected ? selectionmaterial : materials[uvm.Type];
                if (selected)
                {
                    uvm.GeometryModel3D.Material = highlightmaterial;
                    World.HighlightedUnits.Add(uvm);
                }

            }

            // the 1/20 scale of the sl_size value is purely cosmetic, no reason behind it.
            if (cb_wscircle.IsChecked.Value)
            {
                var trans = new TranslateTransform3D(Camera.Position.X, 0, Camera.Position.Y);
                var scale = new ScaleTransform3D(new Vector3D(sl_size.Value / 20, 1, sl_size.Value / 20));
                selectionCylinder.Transform = new Transform3DGroup { Children = { scale, trans } };
                Models.Children.Add(selectionCylinder);
            }
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

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Camera != null)
                Camera.AspectRatio = Viewport.ActualWidth / Viewport.ActualHeight;
        }
    }
}

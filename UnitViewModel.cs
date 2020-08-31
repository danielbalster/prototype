using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Input;

namespace Prototype
{
    public class UnitViewModel : ViewModelBase<Unit>
    {
        public UnitViewModel(Unit model)
        {
            Model = model;

            modelVisual3D.Model = geometryModel3D;
            modelVisual3D.MouseDown += OnMouseDown;
            modelVisual3D.MouseUp += OnMouseUp;
            geometryModel3D.Geometry = Helper.FindResource("cubeMesh") as Geometry3D;
            geometryModel3D.Material = Helper.FindResource("normalMaterial") as Material;
            behaviortree = new BehaviortreeViewModel(Model.Behavior);
            blackboard = new BlackboardViewModel(Model.Blackboard);
        }

        BlackboardViewModel blackboard;
        public BlackboardViewModel Blackboard
        {
            get => blackboard;
            set
            {
                if (value != blackboard)
                {
                    blackboard = value;
                    RaisePropertyChanged("Blackboard");
                }
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Model.World.PendingActions.Add((world) => {
                    world.Units.Remove(Model);
                });
            }
        }

        #region ModelVisual3D
        public ModelUIElement3D ModelVisual3D
        {
            get
            {
                return modelVisual3D;
            }
        }
        ModelUIElement3D modelVisual3D = new ModelUIElement3D();
        #endregion

        #region GeometryModel3D
        public GeometryModel3D GeometryModel3D
        {
            get
            {
                return geometryModel3D;
            }
        }
        private GeometryModel3D geometryModel3D = new GeometryModel3D();
        #endregion

        public UnitTypes Type
        {
            get
            {
                return Model.Type;
            }
            set
            {
                if (Model.Type != value)
                {
                    Model.Type = value;
                    RaisePropertyChanged("Type");
                }
            }
        }

        public bool Selected
        {
            get
            {
                return Model.Selected;
            }
            set
            {
                if (Model.Selected != value)
                {
                    Model.Selected = value;
                    RaisePropertyChanged("Selected");
                }
            }
        }

        public Guid Id
        {
            get => Model.Id;
        }

        BehaviortreeViewModel behaviortree;
        public BehaviortreeViewModel Behaviortree
        {
            get
            {
                return behaviortree;
            }
            set
            {
                if (!Model.Behavior.Equals(behaviortree.Model))
                {
                    behaviortree = value;
                    Model.Behavior = behaviortree.Model;
                    RaisePropertyChanged("Behaviortree");
                }
            }
        }

        #region X
        public double X
        {
            get
            {
                return Model.Position.X;
            }
            set
            {
                if (Model.Position.X != value)
                {
                    Model.Position.X = value;
                    RaisePropertyChanged("X");
                }
            }
        }
        #endregion
        #region Y
        public double Y
        {
            get
            {
                return Model.Position.Y;
            }
            set
            {
                if (Model.Position.Y != value)
                {
                    Model.Position.Y = value;
                    RaisePropertyChanged("Y");
                }
            }
        }
        #endregion
        #region Position
        public Vector Position
        {
            get
            {
                return Model.Position;
            }
            set
            {
                if (Model.Position != value)
                {
                    Model.Position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }
        #endregion
    }
}

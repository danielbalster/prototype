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
            Model.PropertyChanged += Model_PropertyChanged;

            modelVisual3D.Model = geometryModel3D;
            modelVisual3D.MouseDown += OnMouseDown;
            modelVisual3D.MouseUp += OnMouseUp;
            geometryModel3D.Geometry = Helper.FindResource("cubeMesh") as Geometry3D;
            if (Model.Behavior!=null)
                behaviortree = new BehaviortreeViewModel(Model.Behavior);
            blackboard = new BlackboardViewModel(Model.Blackboard);
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }

        #region Blackboard
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
        #endregion

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.LeftShift))
        }

        internal void Sync()
        {
            if (Behaviortree == null || (Behaviortree.Model!=null && !Behaviortree.Model.Equals(Model.Behavior)))
            {
                Behaviortree = new BehaviortreeViewModel(Model.Behavior);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Model.World.PendingActions.Add((world) => {
                    world.Units.Remove(Model);
                    Model.Dispose();
                });
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                foreach (var unit in Model.World.Units)
                {
                    unit.Selected = unit.GroupId == GroupId;
                }
            }
            else
            {
                Selected = !Selected;
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

        #region Type
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
        #endregion

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

        public Guid GroupId
        {
            get
            {
                return Model.GroupId;
            }
            set
            {
                if (Model.GroupId != value)
                {
                    Model.GroupId = value;
                    RaisePropertyChanged("GroupId");
                }
            }
        }

        public Guid Id
        {
            get => Model.Id;
        }

        #region Behaviortree
        BehaviortreeViewModel behaviortree;
        public BehaviortreeViewModel Behaviortree
        {
            get
            {
                return behaviortree;
            }
            set
            {
                if (Model.Behavior == null || behaviortree==null || !Model.Behavior.Equals(behaviortree.Model))
                {
                    behaviortree = value;
                    Model.Behavior = behaviortree.Model;
                    RaisePropertyChanged("Behaviortree");
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

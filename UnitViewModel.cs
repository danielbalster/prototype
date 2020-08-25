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

            modelVisual3D = new ModelVisual3D();
            var geometryModel = new GeometryModel3D();
            modelVisual3D.Content = geometryModel;
            geometryModel.Geometry = Helper.FindResource("cubeMesh") as Geometry3D;
            geometryModel.Material = Helper.FindResource("normalMaterial") as Material;
            //geometryModel.Transform = new TranslateTransform3D(model.Position.X, 0, model.Position.Y);

            behaviortree = new BehaviortreeViewModel(Model.Behavior);

        }

        public GeometryModel3D GeometryModel3D
        {
            get
            {
                return modelVisual3D.Content as GeometryModel3D;
            }
        }

        #region Position
        private ModelVisual3D modelVisual3D;
        public ModelVisual3D ModelVisual3D
        {
            get
            {
                return modelVisual3D;
            }
        }
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



        // horrible hack: since WPF's Media3D supports no binding, whole MVVM is not usable.
        // instead, the viewmodel will hold the 3D object and modifies it directly.
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Behaviortree;

namespace Prototype
{
    public class BehaviortreeViewModel : ViewModelBase<Behaviortree.Behaviortree>
    {
        public BehaviortreeViewModel(Behaviortree.Behaviortree model)
        {
            Model = model;
        }

        #region Root
        public Node Root
        {
            get
            {
                return Model.Root;
            }
            set
            {
                if (Model.Root != value)
                {
                    Model.Root = value;
                    RaisePropertyChanged("Root");
                }
            }
        }
        #endregion

        #region Name
        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        #endregion
    }
 
}

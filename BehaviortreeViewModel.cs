using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Behaviortree;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace Prototype
{
    public class BehaviortreeViewModel : ViewModelBase<Behaviortree.Behaviortree>
    {
        public BehaviortreeViewModel(Behaviortree.Behaviortree model)
        {
            Model = model;
            if (model != null)
            TreeRoot = (INodeViewModel) ViewModelFactory.Create(model.Root);
        }
        public INodeViewModel TreeRoot { get; set; } = null;

        public INodeViewModel FindNode(Guid guid)
        {
            return TreeRoot.FindNode(guid);
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

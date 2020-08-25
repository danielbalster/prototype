using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Prototype
{
    public class BehaviortreesEditorViewModel : ViewModelBase<World>
    {
        public BehaviortreesEditorViewModel(World model)
        {
            Model = model;

            Behaviortrees = new ObservableCollection<BehaviortreeViewModel>();
            Model.Behaviortrees.CollectionChanged += Behaviortrees_CollectionChanged;
        }

        public ObservableCollection<BehaviortreeViewModel> Behaviortrees { get; set; }

        private void Behaviortrees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Behaviortree.Behaviortree bt in e.NewItems)
                    {
                        Behaviortrees.Insert(e.NewStartingIndex, new BehaviortreeViewModel(bt));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Behaviortree.Behaviortree bt in e.OldItems)
                    {
                        Behaviortrees.Remove(Behaviortrees.Where(x => x.Model == bt).Single());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Behaviortrees.Clear();
                    break;
            }
        }
    }
}

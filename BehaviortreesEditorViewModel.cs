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
using System.Reflection;
using Prototype.Behaviortree;
using System.Windows.Threading;

namespace Prototype
{
    public class BehaviortreesEditorViewModel : ViewModelBase<World>
    {
        public BehaviortreesEditorViewModel(World model)
        {
            Model = model;

            Behaviortrees = new ObservableCollection<BehaviortreeViewModel>();
            Model.Behaviortrees.CollectionChanged += Behaviortrees_CollectionChanged;

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var na = type.GetCustomAttribute<NodeAttribute>();
                if (na != null)
                {
                    na.Type = type;
                    DraggableNodes.Add(na);
                }
            }

            timer.Interval = new TimeSpan(0,0,0,0,100);
            timer.Tick += OnTimerTick;
            timer.Start();

        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (selectedUnit == null) return;
            if (selectedUnit.Blackboard == null) return;
            selectedUnit.Blackboard.Sync();
            // only if the edited behaviortree is used by the currently selected unit
            if (selectedUnit.Behaviortree!=null && Current!=null && selectedUnit.Behaviortree.Model == Current.Model)
            {
                foreach (var de in selectedUnit.Blackboard.Model.store)
                {
                    if (de.Key.Item2 != "!") continue;
                    var nvm = Current.FindNode(de.Key.Item1) as INodeViewModel;
                    if (nvm == null) continue;
                    nvm.Status = (Status)de.Value;
                }
            }
        }

        DispatcherTimer timer = new DispatcherTimer();

        UnitViewModel selectedUnit;
        public UnitViewModel SelectedUnit
        {
            get => selectedUnit;
            set
            {
                if (selectedUnit != value)
                {
                    selectedUnit = value;
                    RaisePropertyChanged("SelectedUnit");
                }
            }
        }

        BehaviortreeViewModel current;
        public BehaviortreeViewModel Current
        {
            get => current;
            set
            {
                if (current != value)
                {
                    current = value;
                    RaisePropertyChanged("Current");
                }
            }
        }

        public List<NodeAttribute> DraggableNodes { get; set; } = new List<NodeAttribute>();

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

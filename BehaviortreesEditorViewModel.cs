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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;

namespace Prototype
{
    public class BehaviortreesEditorViewModel : ViewModelBase<World>
    {
        public BehaviortreesEditorViewModel(World model)
        {
            Model = model;

            Behaviortrees = new ObservableCollection<BehaviortreeViewModel>();
            Model.Behaviortrees.CollectionChanged += Behaviortrees_CollectionChanged;


            Blackboard.Instances.CollectionChanged += Blackboards_CollectionChanged;
            foreach( var bb in Blackboard.Instances)
            {
                Blackboards.Add(new BlackboardViewModel(bb));
            }
            

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

            AddBehaviortree = new RelayCommand(arg => {
                Model.Behaviortrees.Add(new Behaviortree.Behaviortree { Name = "Untitled", Root = new Behaviortree.Decorator() });
            });
            RemoveBehaviortree = new RelayCommand(arg => {
                if (Current != null)
                    Model.Behaviortrees.Remove(Current.Model);
            });

            ofDialog.DefaultExt = ".behaviors";
            ofDialog.FileName = "untitled";
            ofDialog.Filter = "Behaviors (*.behaviors)|All(*.*)";
            sfDialog.DefaultExt = ".behaviors";
            sfDialog.FileName = "untitled";
            sfDialog.Filter = "Behaviors (*.behaviors)|All(*.*)";

            Load = new RelayCommand(arg => {
                if (ofDialog.ShowDialog().Value)
                {
                    load(ofDialog.FileName);
                }
            });
            Save = new RelayCommand(arg => {
                if (sfDialog.ShowDialog().Value)
                {
                    save(sfDialog.FileName);
                }
            });
        }
        OpenFileDialog ofDialog = new OpenFileDialog();
        SaveFileDialog sfDialog = new SaveFileDialog();

        #region silly serializer

        private void load(string filename)
        {
            try
            {
                Model.Behaviortrees.Clear();
                var doc = new XmlDocument();
                doc.Load(filename);
                foreach (XmlElement n in doc.SelectNodes("//Behaviortree"))
                {
                    var behaviortree = new Behaviortree.Behaviortree();
                    behaviortree.Name = n.GetAttribute("name");
                    behaviortree.Root = loadBehavior(n.ChildNodes[0] as XmlElement);
                    if (behaviortree.Root == null) behaviortree.Root = new Decorator();
                    Model.Behaviortrees.Add(behaviortree);
                }

                Model.Units.Clear();
                foreach (XmlElement n in doc.SelectNodes("//Unit"))
                {
                    var unit = new Unit();
                    unit.Behavior = Model.FindBehaviortreeByName(n.GetAttribute("behavior"));
                    unit.Position.X = double.Parse(n.GetAttribute("x"));
                    unit.Position.Y = double.Parse(n.GetAttribute("y"));
                    unit.Type = (UnitTypes) Enum.Parse(typeof(UnitTypes), n.GetAttribute("type"));
                    Model.Units.Add(unit);
                }

            }
            catch (Exception ex)
            {

            }
        }

        private INode loadBehavior(XmlElement e)
        {
            if (e == null) return null;
            var node = NodeFactory.Create(e.Name);
            if (node == null) return null;

            var guid = e.GetAttribute("guid");
            node.Id = Guid.Parse(guid);

            foreach (var type in node.GetType().GetProperties())
            {
                if (type.GetCustomAttribute<PropertyAttribute>() == null) continue;
                try
                {
                    var value = e.GetAttribute(type.Name);
                    var targetvalue = Convert.ChangeType(value, type.PropertyType);
                    type.SetMethod.Invoke(node, new[] { targetvalue });
                }
                catch(Exception)
                {

                }
            }

            foreach ( XmlElement child in e.ChildNodes )
            {
                var c = loadBehavior(child);
                if (c != null) node.Add(c);
            }
            return node;
        }

        private void save(string filename)
        {
            try
            {
                var doc = new XmlDocument();
                var world = doc.CreateElement("World");
                var behaviors = doc.CreateElement("Behaviors");
                var units = doc.CreateElement("Units");

                doc.AppendChild(world);
                world.AppendChild(behaviors);
                world.AppendChild(units);
                foreach (var bt in Model.Behaviortrees)
                {
                    var behavior = doc.CreateElement("Behaviortree");
                    behaviors.AppendChild(behavior);
                    behavior.SetAttribute("name", bt.Name);
                    saveBehavior(behavior, bt.Root);
                }

                foreach( var unit in Model.Units )
                {
                    var un = doc.CreateElement("Unit");
                    units.AppendChild(un);
                    un.SetAttribute("guid", unit.Id.ToString());
                    un.SetAttribute("type", unit.Type.ToString());
                    un.SetAttribute("x", unit.Position.X.ToString());
                    un.SetAttribute("y", unit.Position.Y.ToString());
                    un.SetAttribute("behavior", unit.Behavior.Name);
                }

                doc.Save(filename);
            }
            catch(Exception)
            {

            }
        }

        private void saveBehavior(XmlElement parent, INode node)
        {
            var elem = parent.OwnerDocument.CreateElement(node.Name);
            parent.AppendChild(elem);

            elem.SetAttribute("guid", node.Id.ToString());

            foreach( var type in node.GetType().GetProperties() )
            {
                if (type.GetCustomAttribute<PropertyAttribute>() == null) continue;
                var result = type.GetMethod.Invoke(node, null);
                elem.SetAttribute(type.Name, result.ToString());
            }

            foreach( var child in node ) saveBehavior(elem, child);
        }
        #endregion


        public ICommand AddBehaviortree { get; private set; }
        public ICommand RemoveBehaviortree { get; private set; }
        public ICommand Load { get; private set; }
        public ICommand Save { get; private set; }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (selectedBlackboard == null) return;
            selectedBlackboard.Sync();
            if (selectedBlackboard != null && Current!=null)
            {
                foreach (var de in selectedBlackboard.Model.store)
                {
                    // '!' is the status of the node, we're only interested in that
                    if (de.Key.Item2 != "!") continue;
                    var nvm = Current.FindNode(de.Key.Item1) as INodeViewModel;
                    if (nvm == null) continue;
                    nvm.Status = (Status)de.Value;
                }
            }
        }

        DispatcherTimer timer = new DispatcherTimer();

        BlackboardViewModel selectedBlackboard;
        public BlackboardViewModel SelectedBlackboard
        {
            get => selectedBlackboard;
            set
            {
                if (selectedBlackboard != value)
                {
                    selectedBlackboard = value;
                    RaisePropertyChanged("SelectedBlackboard");
                }
            }
        }

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

        public ObservableCollection<BlackboardViewModel> Blackboards { get; set; } = new ObservableCollection<BlackboardViewModel>();

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
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        Behaviortrees.RemoveAt(e.OldStartingIndex + i);
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

        private void Blackboards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Behaviortree.Blackboard bb in e.NewItems)
                    {
                        Blackboards.Insert(e.NewStartingIndex, new BlackboardViewModel(bb));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        Blackboards.RemoveAt(e.OldStartingIndex + i);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Blackboards.Clear();
                    break;
            }
        }
    }
}

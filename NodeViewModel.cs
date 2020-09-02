using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;

namespace Prototype.Behaviortree
{
    public interface INodeViewModel : INotifyPropertyChanged
    {
        INodeViewModel FindNode(Guid guid);
        Status Status { get; set; }
        Guid Id { get; }
        string Name { get; }
        AmountType AmountChildren { get; }
        INodeViewModel Parent { get; set; }
        ObservableCollection<INodeViewModel> Children { get; set; }
        void Remove(INodeViewModel child);
        INode DataModel { get; }
    }

    public class NodeViewModelBase<T> : ViewModelBase<T> , INodeViewModel
        where T : Node
    {
        public ObservableCollection<INodeViewModel> Children { get; set; } = new ObservableCollection<INodeViewModel>();

        class CreateArgs
        {
            public Type Type { get; set; }
            public INodeViewModel Node { get; set; }
        }

        static Dictionary<string, NodeAttribute> NodeTypes = new Dictionary<string, NodeAttribute>();
        static NodeViewModelBase()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var na = type.GetCustomAttribute<NodeAttribute>();
                if (na != null)
                {
                    na.Type = type;
                    NodeTypes.Add(type.Name, na);
                }
            }
        }

        public NodeViewModelBase(T model)
        {
            Model = model;

            Model.CollectionChanged += Model_CollectionChanged;
            foreach (var item in Model)
            {
                var vm = (INodeViewModel)ViewModelFactory.Create(item);
                if (vm != null)
                {
                    vm.Parent = this;
                    Children.Add(vm);
                }
            }

            Delete = new RelayCommand(arg =>
            {
                if (Parent != null)
                    Parent.Remove(this);
            });

            Create = new RelayCommand(arg =>
            {
                ContextMenu.Items.Clear();
                foreach( var de in NodeTypes )
                {
                    var mi = new MenuItem();
                    mi.Header = de.Key;
                    mi.Command = new RelayCommand((arg2) =>
                    {
                        var ca = arg2 as CreateArgs;

                        var node = Activator.CreateInstance(ca.Type) as Prototype.Behaviortree.Node;
                        ca.Node.DataModel.Add(node);
                    });
                    mi.CommandParameter = new CreateArgs { Type = de.Value.Type, Node = this};

                    ContextMenu.Items.Add(mi);
                }
                ContextMenu.IsOpen = true;


            });

            CheckEmpty();
        }

        public ContextMenu ContextMenu { get; set; } = new ContextMenu();

        public INode DataModel { get => Model; }

        public void Remove(INodeViewModel child)
        {
            Model.Remove(child.DataModel);
        }

        public ICommand Delete { get; private set; }
        public ICommand Create { get; private set; }

        private void Model_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach( var item in e.NewItems )
                    {
                        var vm = (INodeViewModel)ViewModelFactory.Create(item);
                        if (vm != null)
                        {
                            vm.Parent = this;
                            Children.Add(vm);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i=0; i<e.OldItems.Count; ++i)
                    {
                        Children.RemoveAt(e.OldStartingIndex+i);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Children.Clear();
                    break;
            }

            CheckEmpty();
        }

        void CheckEmpty()
        {
            switch (AmountChildren)
            {
                case AmountType.None:
                    IsEmpty = false;
                    break;
                case AmountType.One:
                case AmountType.Many:
                    IsEmpty = Children.Count == 0;
                    break;
            }
        }

        public INodeViewModel FindNode(Guid guid)
        {
            if (Id == guid) return this;
            foreach( var child in Children)
            {
                var nvm = child.FindNode(guid);
                if (nvm != null) return nvm;
            }
            return null;
        }

        // this is actually not in the model, only in the viewmodel
        Status status = Status.Error;
        public Status Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        bool isEmpty = false;
        public bool IsEmpty
        {
            get => isEmpty;
            set
            {
                if (isEmpty != value)
                {
                    isEmpty = value;
                    RaisePropertyChanged("IsEmpty");
                }
            }
        }



        public Guid Id { get => Model.Id; }
        public string Name { get => Model.Name; }
        public AmountType AmountChildren { get => Model.AmountChildren; }
        public INodeViewModel Parent { get; set; }
    }

    [Model(Type=typeof(Node))]
    public class NodeViewModel : NodeViewModelBase<Node>
    {
        public NodeViewModel(Node model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Decorator))]
    public class DecoratorViewModel : NodeViewModelBase<Decorator>
    {
        public DecoratorViewModel(Decorator model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Inverter))]
    public class InverterViewModel : NodeViewModelBase<Inverter>
    {
        public InverterViewModel(Inverter model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Selector))]
    public class SelectorViewModel : NodeViewModelBase<Selector>
    {
        public SelectorViewModel(Selector model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Sequence))]
    public class SequenceViewModel : NodeViewModelBase<Sequence>
    {
        public SequenceViewModel(Sequence model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Print))]
    public class PrintViewModel : NodeViewModelBase<Print>
    {
        public PrintViewModel(Print model) : base(model)
        {
        }
        public string Text
        {
            get => Model.Text;
            set
            {
                if (Model.Text != value)
                {
                    Model.Text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }
    }

    [Model(Type = typeof(Delay))]
    public class DelayViewModel : NodeViewModelBase<Delay>
    {
        public DelayViewModel(Delay model) : base(model)
        {
        }

        public double Milliseconds
        {
            get => Model.Milliseconds;
            set
            {
                if (Model.Milliseconds != value)
                {
                    Model.Milliseconds = value;
                    RaisePropertyChanged("Milliseconds");
                }
            }
        }
    }

    [Model(Type = typeof(HasTarget))]
    public class HasTargetViewModel : NodeViewModelBase<HasTarget>
    {
        public HasTargetViewModel(HasTarget model) : base(model)
        {
        }
    }

    [Model(Type = typeof(FindTarget))]
    public class FindTargetViewModel : NodeViewModelBase<FindTarget>
    {
        public FindTargetViewModel(FindTarget model) : base(model)
        {
        }
    }

    [Model(Type = typeof(MoveTo))]
    public class MoveToViewModel : NodeViewModelBase<MoveTo>
    {
        public MoveToViewModel(MoveTo model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Succeeder))]
    public class SucceederViewModel : NodeViewModelBase<Succeeder>
    {
        public SucceederViewModel(Succeeder model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Failer))]
    public class FailerViewModel : NodeViewModelBase<Failer>
    {
        public FailerViewModel(Failer model) : base(model)
        {
        }
    }

    [Model(Type = typeof(Repeater))]
    public class RepeaterViewModel : NodeViewModelBase<Repeater>
    {
        public int N
        {
            get => Model.N;
            set
            {
                if (Model.N != value)
                {
                    Model.N = value;
                    RaisePropertyChanged("N");
                }
            }
        }

        public RepeaterViewModel(Repeater model) : base(model)
        {
        }
    }

    [Model(Type = typeof(RepeatUntilFail))]
    public class RepeatUntilFailViewModel : NodeViewModelBase<RepeatUntilFail>
    {
        public RepeatUntilFailViewModel(RepeatUntilFail model) : base(model)
        {
        }
    }


}

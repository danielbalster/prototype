using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Specialized;

namespace Prototype.Behaviortree
{
    public interface INodeViewModel
    {
        INodeViewModel FindNode(Guid guid);
        Status Status { get; set; }
        Guid Id { get; }
        string Name { get; }
        AmountType AmountChildren { get; }
    }

    public class NodeViewModelBase<T> : ViewModelBase<T> , INodeViewModel
        where T : Node
    {
        public List<INodeViewModel> Children { get; set; } = new List<INodeViewModel>();

        public NodeViewModelBase(T model)
        {
            Model = model;

            Model.CollectionChanged += Model_CollectionChanged;
            foreach (var item in Model)
            {
                var vm = (INodeViewModel)ViewModelFactory.Create(item);
                if (vm != null) Children.Add(vm);
            }
        }

        private void Model_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach( var item in e.NewItems )
                    {
                        var vm = (INodeViewModel)ViewModelFactory.Create(item);
                        if (vm != null) Children.Add(vm);
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

        public Guid Id { get => Model.Id; }
        public string Name { get => Model.Name; }
        public AmountType AmountChildren { get => Model.AmountChildren; }
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


}

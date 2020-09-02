using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Prototype.Behaviortree
{
    public enum AmountType
    {
        None,
        One,
        Many,
    }

    public interface INode : ICollection, IList
    {
        Guid Id { get; }
        string Name { get; }
        Status Execute(Blackboard bb);
    }

    public class Node : ObservableCollection<INode>, INode
    {
        public Guid Id { get; } = Guid.NewGuid();

        public virtual AmountType AmountChildren { get => AmountType.None; }

        public string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected virtual void OnOpen(Blackboard bb)
        {
        }

        protected virtual void OnClose(Blackboard bb)
        {
        }

        protected virtual void OnEnter(Blackboard bb)
        {

        }
        protected virtual void OnExit(Blackboard bb)
        {

        }
        protected virtual Status OnExecute(Blackboard bb)
        {
            return Status.Error;
        }

        /*
         * ?!#$ are internal keys, they can be hidden in editors.
         * 
         * ? = is a node currently open or closed?
         * ! = last state of a node
         * # = iterator of sequences/selectors
         * 
         */

        public Status Execute(Blackboard bb)
        {
            OnEnter(bb);
            bb.Get(Id, "?", out object isOpen);
            if (isOpen==null || !(bool)isOpen)
            {
                bb.Set(Id, "?", true);
                OnOpen(bb);
            }
            var status = OnExecute(bb);
            bb.Set(Id, "!", status);
            if (status != Status.Running)
            {
                OnClose(bb);
                bb.Set(Id, "?", false);
            }
            OnExit(bb);
            return status;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}

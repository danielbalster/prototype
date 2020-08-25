using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    public enum AmountType
    {
        None,
        One,
        Many,
    }

    public class Node : List<Node>
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

        protected virtual void OnOpen(object sender)
        {
        }

        protected virtual void OnClose(object sender)
        {
        }

        protected virtual void OnEnter(object sender)
        {

        }
        protected virtual void OnExit(object sender)
        {

        }
        protected virtual Status OnExecute(object sender)
        {
            return Status.Error;
        }

        public Status Execute(object sender)
        {
            OnEnter(sender);
            object isOpen = false;
            Blackboard.Get(sender, Id, "isOpen", out  isOpen);
            if (isOpen==null || !(bool)isOpen)
            {
                Blackboard.Set(sender, Id, "isOpen", true);
                OnOpen(sender);
            }
            var status = OnExecute(sender);
            if (status != Status.Running)
            {
                OnClose(sender);
                Blackboard.Set(sender, Id, "isOpen", false);
            }
            OnExit(sender);
            return status;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node]
    class Delay : Decorator
    {
        [Property]
        public double Milliseconds { get; set; }

        protected override void OnOpen(object sender)
        {
            Blackboard.Set(sender,Id, "delay", DateTime.Now.AddMilliseconds(Milliseconds));
        }

        protected override Status OnExecute(object sender)
        {
            Blackboard.Get(sender,Id, "delay", out object future);
            if ((((DateTime)future).Ticks - DateTime.Now.Ticks) > 0) return Status.Running;
            return this[0].Execute(sender);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node(Path = "Basic/Delay")]
    public class Delay : Decorator
    {
        [Property]
        public double Milliseconds { get; set; }

        protected override void OnOpen(Blackboard bb)
        {
            bb.Set(Id, "alarm", DateTime.Now.AddMilliseconds(Milliseconds));
        }

        protected override Status OnExecute(Blackboard bb)
        {
            if (Count == 0) return Status.Error;
            bb.Get(Id, "alarm", out object future);
            if ((((DateTime)future).Ticks - DateTime.Now.Ticks) > 0) return Status.Running;
            return this[0].Execute(bb);
        }
    }
}

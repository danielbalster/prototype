using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node]
    public class Repeater : Decorator
    {
        [Property]
        public int N { get; set; }

        protected override void OnOpen(Blackboard bb)
        {
            bb.Set(Id, "i", 0);
        }

        protected override void OnClose(Blackboard bb)
        {
            bb.Clear(Id, "i");
        }

        protected override Status OnExecute(Blackboard bb)
        {
            if (Count == 0) return Status.Error;
            bb.Get<int>(Id, "i", out int i);
            if (i < N)
            {
                var status = this[0].Execute(bb);
                if (status == Status.Success)
                {
                    i++;
                    bb.Set(Id, "i", i);
                    return Status.Running;
                }
                return status;
            }
            return Status.Success;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node(Path = "Basic/RepeatUntilFail")]
    public class RepeatUntilFail : Decorator
    {
        protected override Status OnExecute(Blackboard bb)
        {
            if (Count == 0) return Status.Error;
            var status = this[0].Execute(bb);
            if (status == Status.Error)
                return Status.Error;
            if (status == Status.Failure)
                return Status.Success;
            return Status.Running;
        }
    }
}

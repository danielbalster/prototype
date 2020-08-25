using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    // success = all have succeeded

    [Node]
    class Sequence : Composite
    {
        protected override void OnOpen(object sender)
        {
            Blackboard.Set(sender, Id, "i", 0);
        }

        protected override Status OnExecute(object sender)
        {
            int start = 0;
            if (Blackboard.Get(sender, Id, "i", out object iter))
            {
                start = (int)iter;
            }

            for (int i = start; i < Count; ++i)
            {
                var status = this[i].Execute(sender);
                if (status != Status.Success)
                {
                    if (status == Status.Running)
                    {
                        Blackboard.Set(sender, Id, "i", i);
                    }
                    return status;
                }
            }
            return Status.Success;
        }
    }
}

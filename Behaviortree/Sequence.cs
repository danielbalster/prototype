using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    // success = all have succeeded

    [Node(Path = "Sequence")]
    public class Sequence : Composite
    {
        protected override void OnOpen(Blackboard bb)
        {
            bb.Set(Id, "#", 0);
        }

        protected override Status OnExecute(Blackboard bb)
        {
            int start = 0;
            if (bb.Get(Id, "#", out object iter))
            {
                start = (int)iter;
            }

            for (int i = start; i < Count; ++i)
            {
                var status = this[i].Execute(bb);
                if (status != Status.Success)
                {
                    if (status == Status.Running)
                    {
                        bb.Set(Id, "#", i);
                    }
                    return status;
                }
            }
            return Status.Success;
        }
    }
}

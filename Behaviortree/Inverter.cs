using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node]
    sealed class Inverter : Decorator
    {
        protected override Status OnExecute(object sender)
        {
            var status = this[0].Execute(sender);
            switch(status)
            {
                case Status.Success: return Status.Failure;
                case Status.Failure: return Status.Success;
                default:
                    return status;
            }
        }
    }
}

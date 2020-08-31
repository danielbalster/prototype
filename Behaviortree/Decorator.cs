using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    public class Decorator : Node
    {
        public override AmountType AmountChildren { get => AmountType.One; }

        public Node Child
        {
            get
            {
                return this[0];
            }
            set
            {
                Clear();
                Add(value);
            }
        }

        public Decorator()
        {
            //Capacity = 1;
        }

        protected override Status OnExecute(Blackboard bb)
        {
            return this[0].Execute(bb);
        }
    }
}

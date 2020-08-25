using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    class Composite : Node
    {
        public override AmountType AmountChildren { get => AmountType.Many; }
    }
}

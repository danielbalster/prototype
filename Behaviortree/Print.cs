using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node]
    class Print : Node
    {
        public string Text { get; set; } = "empty";

        protected override Status OnExecute(object sender)
        {
            Console.WriteLine(Text);
            return Status.Success;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    [Node(Path = "Basic/Print")]
    public class Print : Node
    {
        [Property]
        public string Text { get; set; } = "empty";

        protected override Status OnExecute(Blackboard bb)
        {
            Console.WriteLine(Text);
            return Status.Success;
        }
    }
}

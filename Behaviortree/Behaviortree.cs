using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    public class Behaviortree
    {
        public Node Root = null;

        public string Name { get; set; } = "Untitled";

        public Status Result { get; set; }

        public Status Execute(Blackboard bb)
        {
            if (Root == null) return Status.Error;
            return Root.Execute(bb);
        }
    }
}

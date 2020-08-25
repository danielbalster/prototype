using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Behaviortree;
using System.Windows;

namespace Prototype
{
    [Node]
    class FindTarget : Node
    {
        Random rng = new Random();
        protected override Status OnExecute(object sender)
        {
            var p = new Vector(rng.Next(-100, +100), rng.Next(-100, +100));
            Blackboard.Set(sender, Guid.Empty, "target", p);
            return Status.Success;
        }
    }

    [Node]
    class HasTarget : Node
    {
        protected override Status OnExecute(object sender)
        {
            if (Blackboard.Get(sender, Guid.Empty, "target", out object value))
            {
                return Status.Success;
            }
            return Status.Failure;
        }
    }

    [Node]
    class MoveTo : Node
    {
        protected override void OnOpen(object sender)
        {
            var unit = sender as Unit;
            if (unit == null) return;

            if (!Blackboard.Get(sender, Guid.Empty, "target", out object target)) return;


            var delta = (Vector)target- unit.Position;
            delta.Normalize();

            Blackboard.Set(sender, Id, "step", delta * 0.1);
        }

        protected override void OnClose(object sender)
        {
            Blackboard.Clear(sender, Id, "step");
            Blackboard.Clear(sender, Guid.Empty, "target");
        }

        protected override Status OnExecute(object sender)
        {
            var unit = sender as Unit;
            if (unit == null) return Status.Error;

            if (!Blackboard.Get(sender, Guid.Empty, "target", out object target)) return Status.Error;

            if (!Blackboard.Get(sender, Id, "step", out object step)) return Status.Error;

            unit.Position += (Vector)step;
            var diff = unit.Position - (Vector)target;
            if (diff.Length > 1.0)
            { 
                return Status.Running;
            }

            unit.Position = (Vector)target;

            return Status.Success;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Behaviortree;
using System.Windows;

/*
 * add new behavior tree nodes here
 * 
 * next, they also need a ViewModel (NodeViewModel.cs)
 * next, they also need a datatemplate (BehaviorTreeEditor.xaml)
 *
 */

namespace Prototype
{


    [Node]
    public class FindTarget : Node
    {
        static Random rng = new Random();
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "unit", out Unit unit)) return Status.Error;
            var r = Helper.DegreesToRadians * rng.Next(0, 360);
            var p = new Vector(Math.Sin(r)*10,Math.Cos(r)*10);
            bb.Set( Guid.Empty, "target", unit.Position + p);
            return Status.Success;
        }
    }

    [Node]
    public class SetTarget : Node
    {
        static Random rng = new Random();
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "world", out World world)) return Status.Error;
            
            var r = Helper.DegreesToRadians * rng.Next(0, 360);
            var p = new Vector(Math.Sin(r) * 10, Math.Cos(r) * 10);
            bb.Set(Guid.Empty, "target", world.TargetPosition + p);
            return Status.Success;
        }
    }



    [Node]
    public class HasTarget : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            if (bb.Get( Guid.Empty, "target", out Vector value))
            {
                return Status.Success;
            }
            return Status.Failure;
        }
    }

    [Node]
    public class MoveTo : Node
    {
        protected override void OnOpen(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "unit", out Unit unit)) return;
            if (!bb.Get( Guid.Empty, "target", out Vector target)) return;

            var delta = target- unit.Position;
            delta.Normalize();

            bb.Set( Id, "step", delta * 0.1);
        }

        protected override void OnClose(Blackboard bb)
        {
            bb.Clear( Id, "step");
            bb.Clear( Guid.Empty, "target");
        }

        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get( Guid.Empty, "unit", out Unit unit)) return Status.Error;
            if (!bb.Get( Guid.Empty, "target", out Vector target)) return Status.Error;

            if (!bb.Get( Id, "step", out Vector step)) return Status.Error;

            unit.Position += step;
            var diff = unit.Position - target;
            if (diff.Length > 1.0)
            { 
                return Status.Running;
            }

            unit.Position = target;

            return Status.Success;
        }
    }

}

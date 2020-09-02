namespace Prototype.Behaviortree
{
    [Node]
    public class Succeeder : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            return Status.Success;
        }
    }
}

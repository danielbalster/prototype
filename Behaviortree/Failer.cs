namespace Prototype.Behaviortree
{
    [Node]
    public class Failer : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            return Status.Failure;
        }
    }
}

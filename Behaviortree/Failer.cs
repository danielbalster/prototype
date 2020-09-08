namespace Prototype.Behaviortree
{
    [Node(Path = "Basic/Failer")]
    public class Failer : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            return Status.Failure;
        }
    }
}

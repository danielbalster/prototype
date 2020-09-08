namespace Prototype.Behaviortree
{
    [Node(Path = "Basic/Succeeder")]
    public class Succeeder : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            return Status.Success;
        }
    }
}

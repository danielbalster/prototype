namespace Prototype.Behaviortree
{
    [Node(Path="Basic/Decorator")]
    public class Decorator : Node
    {
        public override AmountType AmountChildren { get => AmountType.One; }

        public INode Child
        {
            get
            {
                return this[0];
            }
            set
            {
                Clear();
                Add(value);
            }
        }

        protected override Status OnExecute(Blackboard bb)
        {
            if (Count == 0) return Status.Error;
            return this[0].Execute(bb);
        }
    }
}

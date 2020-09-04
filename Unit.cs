using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows;

namespace Prototype
{
    public enum UnitTypes
    {
        Military,
        Leader,
        Carrier,
        Builder,
        Worker,
        Animal,
    }

    public class Unit : IDisposable
    {
        public Vector Position = new Vector();
        public bool Selected = false;
        public UnitTypes Type = UnitTypes.Military;
        public int GroupdId = 0;
        public Guid Id { get; private set; } = Guid.NewGuid();

        private Behaviortree.Blackboard blackboard = new Behaviortree.Blackboard();
        private Behaviortree.Behaviortree behavior = null;
        private World world = null;

        public Behaviortree.Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                if (blackboard != value)
                {
                    blackboard = value;
                }
            }
        }

        public Behaviortree.Behaviortree Behavior
        {
            get => behavior;
            set
            {
                if (behavior != value)
                {
                    behavior = value;
                    blackboard.Set(Guid.Empty, "unit", this);
                }
            }
        }
        public World World
        {
            get => world;
            set
            {
                if (world != value)
                {
                    world = value;
                    blackboard.Set(Guid.Empty, "world", world);
                }
            }
        }

        public void Update()
        {
            if (Behavior != null)
            {
                Behavior.Result = Behavior.Execute(blackboard);
            }
        }

        public void Dispose()
        {
            Behavior = null;
            World = null;
            blackboard.Dispose();
            blackboard = null;
        }
    }
}

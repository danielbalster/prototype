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

    public class Unit
    {
        Behaviortree.Behaviortree behavior;
        public Vector Position = new Vector();
        public bool Selected = false;
        public UnitTypes Type = UnitTypes.Military;

        public Behaviortree.Behaviortree Behavior
        {
            get { return behavior; }
            set { behavior = value; }
        }

        public Unit()
        {
        }

        public void Update()
        {
            if (behavior != null)
            {
                var status = behavior.Execute(this);
            }
        }
    }
}

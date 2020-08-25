using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Prototype
{
    public class World
    {
        public ObservableCollection<Unit> Units { get; } = new ObservableCollection<Unit>();
        public ObservableCollection<Behaviortree.Behaviortree> Behaviortrees { get; } = new ObservableCollection<Behaviortree.Behaviortree>();

        // grid: 100 x 100 cells
        // cell can be occupied or not

        public Behaviortree.Behaviortree FindBehaviortreeByName(string key)
        {
            return Behaviortrees.Single(s => s.Name == key);
        }

        public void Init()
        {
            Behaviortrees.Add(
                new Behaviortree.Behaviortree
                {
                    Name = "Random Mover",
                    Root = new Behaviortree.Selector {
                        new Behaviortree.Sequence {
                            new HasTarget(),
                            new MoveTo()
                        },
                        new Behaviortree.Sequence
                        {
                            new Behaviortree.Inverter{ new HasTarget() },
                            new FindTarget { },
                        }
                    }
                }
            );

            Behaviortrees.Add(
                new Behaviortree.Behaviortree
                {
                    Name = "Hello World Printer",
                    Root =
                    new Behaviortree.Selector {

                        new Behaviortree.Delay {
                            Milliseconds = 2000 ,
                             Child = new Behaviortree.Print {
                                 Text = "Hello, World!"
                             },
                        }
                    }
                }
            );
        }

        public delegate void WorldUpdatedDelegate(object sender);
        public event WorldUpdatedDelegate WorldUpdated;

        public void Update()
        {
            foreach( var unit in Units )
            {
                unit.Update();
            }
            WorldUpdated?.Invoke(this);
        }
    }
}

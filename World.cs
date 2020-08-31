using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
namespace Prototype
{
    public class World
    {
        public ObservableCollection<Unit> Units { get; } = new ObservableCollection<Unit>();
        public ObservableCollection<Behaviortree.Behaviortree> Behaviortrees { get; } = new ObservableCollection<Behaviortree.Behaviortree>();

        public List<Action<World>> PendingActions = new List<Action<World>>();

        // grid: 100 x 100 cells
        // cell can be occupied or not

        public Vector CameraPosition
        {
            get;set;
        }

        public Unit Find(Predicate<Unit> predicate)
        {
            foreach( var unit in Units )
            {
                if (predicate(unit)) return unit;
            }
            return null;
        }

        public Unit FindNearest(Unit self)
        {
            double shortest = double.MaxValue;
            Unit result = null;
            foreach(var unit in Units)
            {
                if (unit.Equals(self)) continue;
                var dist = (unit.Position - self.Position).Length;
                if (dist < shortest)
                {
                    shortest = dist;
                    result = unit;
                }
            }
            return result;
        }

        public Unit FindNearestOfType(Unit self, UnitTypes type)
        {
            double shortest = double.MaxValue;
            Unit result = null;
            foreach (var unit in Units)
            {
                if (unit.Equals(self)) continue;
                if (unit.Type != type) continue;
                var dist = (unit.Position - self.Position).Length;
                if (dist < shortest)
                {
                    shortest = dist;
                    result = unit;
                }
            }
            return result;
        }

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
                    Root = new Behaviortree.Decorator
                    {
                        new Behaviortree.Selector {
                            new Behaviortree.Sequence {
                                new HasTarget(),
                                new MoveTo()
                            },
                            new Behaviortree.Sequence
                            {
                                new Behaviortree.Inverter{ new HasTarget() },
                                new Behaviortree.Delay { Milliseconds = 600, Child = new FindTarget { } },
                            }
                        }
                    }
                }
            );

            Behaviortrees.Add(
                new Behaviortree.Behaviortree
                {
                    Name = "Galeore",
                    Root = new Behaviortree.Decorator
                    {
                        new Behaviortree.Selector {
                        new Behaviortree.Sequence {
                            new HasTarget(),
                            new MoveTo()
                        },
                        new Behaviortree.Selector {
                            new Behaviortree.Sequence {
                                new HasTarget(),
                                new MoveTo()
                            },
                            new Behaviortree.Sequence {
                                new HasTarget(),
                                new MoveTo()
                            },
                            new HasTarget(),
                            new MoveTo()
                        },
                        new Behaviortree.Sequence
                        {
                            new Behaviortree.Inverter{ new HasTarget() },
                            new Behaviortree.Inverter{
                                new Behaviortree.Inverter{
                                    new Behaviortree.Inverter{
                                        new Behaviortree.Inverter{
                                            new Behaviortree.Inverter{
                                                new HasTarget()
                                            },
                                        },
                                    },
                                },
                            },
                            new FindTarget { },
                            new Behaviortree.Sequence {
                            new Behaviortree.Sequence {
                                new HasTarget(),
                                new MoveTo()
                            },
                                new HasTarget(),
                                new MoveTo()
                            },
                            new HasTarget(),
                            new MoveTo()
                        }
                    }
                    }
                }
            );


            Behaviortrees.Add(
                new Behaviortree.Behaviortree
                {
                    Name = "Hello World Printer",
                    Root =
                    new Behaviortree.Decorator {
                        new Behaviortree.Selector {

                            new Behaviortree.Delay {
                                Milliseconds = 2000 ,
                                 Child = new Behaviortree.Print {
                                     Text = "Hello, World!"
                                 },
                            }
                        }
                    }
                }
            );
        }

        public delegate void WorldUpdatedDelegate(object sender);
        public event WorldUpdatedDelegate WorldUpdated;

        public void Update(bool playing)
        {
            if (PendingActions.Count > 0)
            {
                foreach (var action in PendingActions)
                {
                    action(this);
                }
                PendingActions.Clear();
            }

            if (playing)
            {
                // simulate all units
                foreach (var unit in Units)
                {
                    unit.Update();
                }
            }
            WorldUpdated?.Invoke(this);
        }
    }
}

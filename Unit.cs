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
        Red,
        Green,
        Blue,
        Yellow,
    }

    public class Unit : Notifier, IDisposable
    {
        #region Selected
        private UnitTypes type = UnitTypes.Red;
        public UnitTypes Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    RaisePropertyChanged("Type");
                }
            }
        }
        #endregion

        #region Id
        private Guid id = Guid.NewGuid();
        public Guid Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }
        #endregion

        #region Selected
        private bool selected = false;
        public bool Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    RaisePropertyChanged("Selected");
                }
            }
        }
        #endregion

        #region Position
        private Vector position = new Vector();
        public Vector Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }
        #endregion



        private Behaviortree.Blackboard blackboard = new Behaviortree.Blackboard();
        private Behaviortree.Behaviortree behavior = null;
        private World world = null;

        static public bool IsExistingGroup(Guid guid)
        {
            return GroupMapping.ContainsKey(guid);
        }

        static public List<Unit> GetGroupMembers(Guid guid)
        {
            if (GroupMapping.TryGetValue(guid, out List<Unit> members)) return members;
            return null;
        }

        static Dictionary<Guid, List<Unit>> GroupMapping { get; } = new Dictionary<Guid, List<Unit>>();

        private Guid groupId = Guid.Empty;
        public Guid GroupId
        {
            get => groupId;
            set
            {
                if (groupId != value)
                {
                    {
                        if (groupId != Guid.Empty && GroupMapping.TryGetValue(groupId, out List<Unit> members))
                        {
                            members.Remove(this);
                            if (members.Count == 0)
                            {
                                GroupMapping.Remove(groupId);
                            }
                        }
                    }

                    groupId = value;

                    {
                        if (groupId != Guid.Empty)
                        {
                            if (GroupMapping.TryGetValue(groupId, out List<Unit> members))
                            {
                                members.Add(this);
                            }
                            else
                            {
                                var group = new List<Unit>();
                                group.Add(this);
                                GroupMapping[groupId] = group;
                            }
                        }
                    }
                    RaisePropertyChanged("GroupId");
                }
            }
        }

        public Behaviortree.Blackboard Blackboard
        {
            get => blackboard;
            set
            {
                if (blackboard != value)
                {
                    blackboard = value;
                    RaisePropertyChanged("Blackboard");
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
                    RaisePropertyChanged("Behavior");
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
                    RaisePropertyChanged("World");
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
            GroupId = Guid.Empty;
            Behavior = null;
            World = null;
            blackboard.Dispose();
            blackboard = null;
        }
    }
}

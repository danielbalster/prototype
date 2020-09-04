using Prototype.Behaviortree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    static class NodeFactory
    {
        static Dictionary<string, Type> NodeTypes = new Dictionary<string, Type>();
        static NodeFactory()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var na = type.GetCustomAttribute<NodeAttribute>();
                if (na != null)
                {
                    NodeTypes.Add(type.Name, type);
                }
            }
        }

        public static INode Create(string name)
        {
            if (!NodeTypes.ContainsKey(name)) return null;
            return Activator.CreateInstance(NodeTypes[name]) as Prototype.Behaviortree.INode;
        }
    }
}

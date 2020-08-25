using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    static class Blackboard
    {
        static Dictionary<Tuple<int, Guid,string>,object> store = new Dictionary<Tuple<int, Guid, string>, object>();

        public static bool Get(object sender, Guid nodeScope, string key, out object value)
        {
            return store.TryGetValue(new Tuple<int, Guid, string>(sender.GetHashCode(), nodeScope, key), out value);
        }
        public static void Set(object sender, Guid nodeScope, string key, object value)
        {
            store[ new Tuple<int, Guid, string>(sender.GetHashCode(), nodeScope, key) ] = value;
        }
        public static void Clear(object sender, Guid nodeScope, string key)
        {
            store.Remove(new Tuple<int, Guid, string>(sender.GetHashCode(), nodeScope, key));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Prototype.Behaviortree
{
    public class Blackboard : IDisposable
    {
        static public ObservableCollection<Blackboard> Instances { get; private set; } = new ObservableCollection<Blackboard>();
        public Blackboard()
        {
            Instances.Add(this);
        }

        public void Dispose()
        {
            Instances.Remove(this);
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public Dictionary<Tuple<Guid,string>,object> store = new Dictionary<Tuple<Guid, string>, object>();

        public bool Get(Guid nodeScope, string key, out object value)
        {
            return store.TryGetValue(new Tuple<Guid, string>(nodeScope, key), out value);
        }

        public bool Get<T>(Guid nodeScope, string key, out T value)
        {
            if (store.TryGetValue (new Tuple<Guid, string>(nodeScope, key), out object o))
            {
                value = (T) o;
                return true;
            }
            value = default(T);
            return false;
        }

        public void Set(Guid nodeScope, string key, object value)
        {
            store[ new Tuple<Guid, string>(nodeScope, key) ] = value;
        }

        public void Clear(Guid nodeScope, string key)
        {
            store.Remove(new Tuple<Guid, string>( nodeScope, key));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Prototype
{
    public class ModelAttribute : Attribute
    {
        public Type Type { get; set; }
    }

    static public class ViewModelFactory
    {
        static Dictionary<Type, Type> creators = new Dictionary<Type, Type>();

        static ViewModelFactory()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var ma = type.GetCustomAttribute<ModelAttribute>();
                if (ma == null) continue;

                creators[ma.Type] = type;
            }
        }

        public static Notifier Create(object model)
        {
            var type = model.GetType();
            if (!creators.TryGetValue(type, out Type value))
                return null;
            return (Notifier) Activator.CreateInstance(value, model);
        }
    }
}

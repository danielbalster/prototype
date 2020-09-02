﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static ViewModel Create(object model)
        {
            var type = model.GetType();
            if (!creators.TryGetValue(type, out Type value))
                return null;
            return (ViewModel) Activator.CreateInstance(value, model);
        }
    }
}
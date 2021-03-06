﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    public class NodeAttribute : Attribute
    {
        public string Description { get; set; }
        public string Path { get; set; }
        public Type Type { get; set; }
    }

    public class PropertyAttribute : Attribute
    {
        public string Description { get; set; }
    }
}

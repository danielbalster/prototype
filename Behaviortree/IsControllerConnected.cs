﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Behaviortree
{
    class IsControllerConnected : Node
    {
        protected override Status OnExecute(object sender)
        {
            return //Player.Instance.Controller.IsConnected ? Status.Success :
            Status.Failure;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace Prototype.Behaviortree
{
    class IsButtonPressed : Node
    {
        //public SharpDX.XInput.GamepadButtonFlags Buttons;

        protected override Status OnExecute(Blackboard bb)
        {
            /*
            if (!Player.Instance.Controller.GetState(out State state))
            {
                return Status.Error;
            }

            if ((state.Gamepad.Buttons & Buttons) == Buttons)
            {
                return Status.Success;
            }
            */
            return Status.Failure;
        }
    }
}

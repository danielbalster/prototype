using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace Prototype
{
    public class ControllerViewModel : ViewModelBase<Controller>
    {
        public ControllerViewModel(Controller model)
        {
            Model = model;

            isConnected = Model.IsConnected;
        }

        bool isConnected;
        public bool IsConnected
        {
            get
            {
                return Model.IsConnected;
            }
            private set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    if (isConnected)
                    {
                        current = Model.GetState();
                        changedButtons = GamepadButtonFlags.None;
                    }
                    RaisePropertyChanged("IsConnected");
                }
            }
        }

        State current;
        GamepadButtonFlags changedButtons;

        internal void Update()
        {
            IsConnected = Model.IsConnected;
            if (!IsConnected) return;
            var next = Model.GetState();
            changedButtons = current.Gamepad.Buttons ^ next.Gamepad.Buttons;
            current = next;
        }

        public bool IsHeld(GamepadButtonFlags flags)
        {
            return IsConnected && (current.Gamepad.Buttons & flags) == flags;
        }

        public bool HasChanged(GamepadButtonFlags flags)
        {
            return IsConnected && (changedButtons & flags) == flags;
        }
    }
}

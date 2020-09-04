﻿using Prototype.Behaviortree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace Prototype
{
    [Node]
    public class ChangeVariable : Node
    {
        [Property]
        public string Key { set; get; }
        [Property]
        public string Value { set; get; }
        protected override Status OnExecute(Blackboard bb)
        {
            bb.Set(Guid.Empty, Key, Value);
            return Status.Success;
        }
    }
    [Model(Type = typeof(ChangeVariable))]
    public class ChangeVariableViewModel : NodeViewModelBase<ChangeVariable>
    {
        #region Key
        public string Key
        {
            get => Model.Key;
            set
            {
                if (Model.Key != value)
                {
                    Model.Key = value;
                    RaisePropertyChanged("Key");
                }
            }
        }
        #endregion
        #region Value
        public string Value
        {
            get => Model.Value;
            set
            {
                if (Model.Value != value)
                {
                    Model.Value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }
        #endregion
        public ChangeVariableViewModel(ChangeVariable model) : base(model)
        {
        }
    }

    [Node]
    public class CompareVariable : Node
    {
        [Property]
        public string Key { set; get; }
        [Property]
        public string Value { set; get; }
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get<string>(Guid.Empty, Key, out string value)) return Status.Error;
            if (value == Value) return Status.Success;
            return Status.Failure;
        }
    }

    [Model(Type = typeof(CompareVariable))]
    public class CompareVariableViewModel : NodeViewModelBase<CompareVariable>
    {
        #region Key
        public string Key
        {
            get => Model.Key;
            set
            {
                if (Model.Key != value)
                {
                    Model.Key = value;
                    RaisePropertyChanged("Key");
                }
            }
        }
        #endregion
        #region Value
        public string Value
        {
            get => Model.Value;
            set
            {
                if (Model.Value != value)
                {
                    Model.Value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }
        #endregion
        public CompareVariableViewModel(CompareVariable model) : base(model)
        {
        }
    }


    [Node]
    public class ButtonPressed : Node
    {
        [Property]
        public int Buttons { get; set; }

        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "controller", out ControllerViewModel cvm)) return Status.Error;
            if (!cvm.IsConnected) return Status.Error;
            var s = cvm.Model.GetState();
            if ((((int)s.Gamepad.Buttons) & Buttons) == Buttons)
                return Status.Success;


            return Status.Failure;
        }
    }
    [Model(Type = typeof(ButtonPressed))]
    public class ButtonPressedViewModel : NodeViewModelBase<ButtonPressed>
    {
        public string Buttons
        {
            get
            {
                var bits = (GamepadButtonFlags) Model.Buttons;
                var sb = new StringBuilder();
                if (bits.HasFlag(GamepadButtonFlags.A)) sb.Append("A ");
                if (bits.HasFlag(GamepadButtonFlags.B)) sb.Append("B ");
                if (bits.HasFlag(GamepadButtonFlags.X)) sb.Append("X ");
                if (bits.HasFlag(GamepadButtonFlags.Y)) sb.Append("Y ");
                if (bits.HasFlag(GamepadButtonFlags.LeftShoulder)) sb.Append("LS ");
                if (bits.HasFlag(GamepadButtonFlags.RightShoulder)) sb.Append("RS ");
                if (bits.HasFlag(GamepadButtonFlags.LeftThumb)) sb.Append("LT ");
                if (bits.HasFlag(GamepadButtonFlags.RightThumb)) sb.Append("RT ");
                if (bits.HasFlag(GamepadButtonFlags.DPadUp)) sb.Append("UP ");
                if (bits.HasFlag(GamepadButtonFlags.DPadDown)) sb.Append("DOWN ");
                if (bits.HasFlag(GamepadButtonFlags.DPadLeft)) sb.Append("LEFT ");
                if (bits.HasFlag(GamepadButtonFlags.DPadRight)) sb.Append("RIGHT ");
                if (bits.HasFlag(GamepadButtonFlags.Start)) sb.Append("START ");
                if (bits.HasFlag( GamepadButtonFlags.Back)) sb.Append("BACK ");
                return sb.ToString();
            }
            set
            {
                GamepadButtonFlags bits = 0;
                var s = value.ToUpperInvariant();
                int i;
                i = s.IndexOf("START"); if (i != -1) { s = s.Remove(i, 5); bits |= GamepadButtonFlags.Start; }
                i = s.IndexOf("BACK"); if (i != -1) { s = s.Remove(i, 4); bits |= GamepadButtonFlags.Back; }
                i = s.IndexOf("RIGHT"); if (i != -1) { s = s.Remove(i, 5); bits |= GamepadButtonFlags.DPadRight; }
                i = s.IndexOf("LEFT"); if (i != -1) { s = s.Remove(i, 4); bits |= GamepadButtonFlags.DPadLeft; }
                i = s.IndexOf("UP"); if (i != -1) { s = s.Remove(i, 2); bits |= GamepadButtonFlags.DPadUp; }
                i = s.IndexOf("DOWN"); if (i != -1) { s = s.Remove(i, 4); bits |= GamepadButtonFlags.DPadDown; }
                i = s.IndexOf("RT"); if (i != -1) { s = s.Remove(i, 2); bits |= GamepadButtonFlags.RightThumb; }
                i = s.IndexOf("LT"); if (i != -1) { s = s.Remove(i, 2); bits |= GamepadButtonFlags.LeftThumb; }
                i = s.IndexOf("RS"); if (i != -1) { s = s.Remove(i, 2); bits |= GamepadButtonFlags.RightShoulder; }
                i = s.IndexOf("LS"); if (i != -1) { s = s.Remove(i, 2); bits |= GamepadButtonFlags.LeftShoulder; }
                i = s.IndexOf("A"); if (i != -1) { s = s.Remove(i, 1); bits |= GamepadButtonFlags.A; }
                i = s.IndexOf("B"); if (i != -1) { s = s.Remove(i, 1); bits |= GamepadButtonFlags.B; }
                i = s.IndexOf("X"); if (i != -1) { s = s.Remove(i, 1); bits |= GamepadButtonFlags.X; }
                i = s.IndexOf("Y"); if (i != -1) { s = s.Remove(i, 1); bits |= GamepadButtonFlags.Y; }
                if (Model.Buttons != (int)bits)
                {
                    Model.Buttons = (int)bits;
                    RaisePropertyChanged("Buttons");
                }
            }
        }

        public ButtonPressedViewModel(ButtonPressed model) : base(model)
        {
        }
    }

    [Node]
    public class HasSelection : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "world", out WorldViewModel world)) return Status.Error;

            foreach (var unit in world.Units)
            {
                if (unit.Selected) return Status.Success;
            }

            return Status.Failure;
        }
    }

    [Node]
    public class SelectUnits : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "world", out WorldViewModel world)) return Status.Error;

            foreach( var unit in world.HighlightedUnits )
            {
                unit.Selected = true;
            }

            return Status.Success;
        }
    }
    [Model(Type = typeof(SelectUnits))]
    public class SelectUnitsViewModel : NodeViewModelBase<SelectUnits>
    {
        public SelectUnitsViewModel(SelectUnits model) : base(model)
        {
        }
    }

    [Node]
    public class UnselectUnits : Node
    {
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "world", out WorldViewModel world)) return Status.Error;

            foreach (var unit in world.HighlightedUnits)
            {
                unit.Selected = false;
            }
            return Status.Success;
        }
    }

    [Model(Type = typeof(UnselectUnits))]
    public class UnselectUnitsViewModel : NodeViewModelBase<UnselectUnits>
    {
        public UnselectUnitsViewModel(UnselectUnits model) : base(model)
        {
        }
    }

    [Node]
    class RemoveSelected : Node
    {

    }



    [Node]
    public class TestSelectionMode : Node
    {
        [Property]
        public SelectionModes SelectionMode { set; get; }
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "world", out WorldViewModel world)) return Status.Error;

            if (world.SelectionMode == SelectionMode)
                return Status.Success;

            return Status.Failure;
        }
    }
    [Model(Type = typeof(TestSelectionMode))]
    public class TestSelectionModeViewModel : NodeViewModelBase<TestSelectionMode>
    {
        #region SelectionMode
        public SelectionModes SelectionMode
        {
            get => Model.SelectionMode;
            set
            {
                if (Model.SelectionMode != value)
                {
                    Model.SelectionMode = value;
                    RaisePropertyChanged("SelectionMode");
                }
            }
        }
        #endregion
        public TestSelectionModeViewModel(TestSelectionMode model) : base(model)
        {
        }
    }

    [Node]
    public class SetSelectionMode : Node
    {
        [Property]
        public SelectionModes SelectionMode { set; get; }
        protected override Status OnExecute(Blackboard bb)
        {
            if (!bb.Get(Guid.Empty, "world", out WorldViewModel world)) return Status.Error;

            world.SelectionMode = SelectionMode;
            return Status.Success;
        }
    }
    [Model(Type = typeof(SetSelectionMode))]
    public class SetSelectionModeViewModel : NodeViewModelBase<SetSelectionMode>
    {
        #region SelectionMode
        public SelectionModes SelectionMode
        {
            get => Model.SelectionMode;
            set
            {
                if (Model.SelectionMode != value)
                {
                    Model.SelectionMode = value;
                    RaisePropertyChanged("SelectionMode");
                }
            }
        }
        #endregion
        public SetSelectionModeViewModel(SetSelectionMode model) : base(model)
        {
        }
    }


}
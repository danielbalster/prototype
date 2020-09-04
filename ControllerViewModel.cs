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
                    RaisePropertyChanged("IsConnected");
                }
            }
        }

        internal void Update()
        {
            IsConnected = Model.IsConnected;
        }
    }
}

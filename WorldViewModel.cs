using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Prototype
{
    public class WorldViewModel : ViewModelBase<World>
    {
        public WorldViewModel(World model)
        {
            Model = model;

        }
        internal void Update()
        {
            Model.Update();
        }
    }
}

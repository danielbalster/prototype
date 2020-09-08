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
using System.Windows.Threading;
using System.Windows;
using Prototype.Behaviortree;

namespace Prototype
{
    public class UnitsEditorViewModel : WorldViewModel
    {
        public UnitsEditorViewModel(World model) : base(model)
        {
            Model = model;

            AddRandom = new RelayCommand(param => addRandom());
            AddUniform = new RelayCommand(param => addUniform());
            ClearUnits = new RelayCommand(param => clearUnits());
            SelectAll = new RelayCommand(param => selectAll());
            SelectNone = new RelayCommand(param => selectNone());
            RemoveSelected = new RelayCommand(param => removeSelected());
            AddSingle = new RelayCommand(param => addSingle());

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += OnTimerTick;
            timer.Start();

        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            foreach( var uvm in Units )
            {
                uvm.Sync();
            }
        }

        DispatcherTimer timer = new DispatcherTimer();

        public ICommand ClearUnits { private set; get; }
        public ICommand AddRandom { private set; get; }
        public ICommand AddSingle { private set; get; }
        public ICommand SelectAll { private set; get; }
        public ICommand SelectNone { private set; get; }
        public ICommand AddUniform { private set; get; }
        public ICommand RemoveSelected { private set; get; }

        public UnitTypes UnitType { get; set; }

        public BehaviortreeViewModel SelectedBehaviortree { get; set; }

        System.Random random = new System.Random();
        private void addRandom()
        {
            //for (int i = 0; i < 100; ++i)
            {
                var unit = new Unit { World = Model };
                unit.Position = new Vector(random.Next(-100, +100), random.Next(-100, +100));
                unit.Selected = true;
                unit.Type = UnitType;
                unit.Behavior = Model.FindBehaviortreeByName("Random Mover");
                Model.Units.Add(unit);
            }
        }
        private void addUniform()
        {
            for (int x=-10; x<10; x++)
                for (int y = -10; y < 10; y++)
                {
                    var unit = new Unit { World = Model };
                    unit.Position = new Vector(x * 10, y * 10);
                    unit.Selected = true;
                    unit.Type = UnitType;
                    unit.Behavior = null; //
                Model.Units.Add(unit);
            }
        }
        private void addSingle()
        {
            var unit = new Unit { World = Model };
            unit.Position = Model.CameraPosition;
            unit.Selected = false;
            unit.Type = UnitType;
            unit.Behavior = SelectedBehaviortree?.Model;
            Model.Units.Add(unit);
        }
        private void removeSelected()
        {
            foreach (var unit in Model.Units.Where(a => a.Selected).ToList())
            {
                Model.Units.Remove(unit);
                unit.Dispose();
            }
        }
        private void clearUnits()
        {
            Model.Units.Clear();
        }
        private void selectNone()
        {
            foreach (var unit in Model.Units) unit.Selected = false;
        }
        private void selectAll()
        {
            foreach (var unit in Model.Units) unit.Selected = true;
        }

    }
}

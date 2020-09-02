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
    public class UnitsEditorViewModel : ViewModelBase<World>
    {
        public UnitsEditorViewModel(World model)
        {
            Model = model;

            Units = new ObservableCollection<UnitViewModel>();
            Model.Units.CollectionChanged += Units_CollectionChanged;

            AddRandom = new RelayCommand(param => addRandom());
            AddUniform = new RelayCommand(param => addUniform());
            ClearUnits = new RelayCommand(param => clearUnits());
            SelectAll = new RelayCommand(param => selectAll());
            SelectNone = new RelayCommand(param => selectNone());
            RemoveSelected = new RelayCommand(param => removeSelected());
            AddSingle = new RelayCommand(param => addSingle());

        }

        public ICommand ClearUnits { private set; get; }
        public ICommand AddRandom { private set; get; }
        public ICommand AddSingle { private set; get; }
        public ICommand SelectAll { private set; get; }
        public ICommand SelectNone { private set; get; }
        public ICommand AddUniform { private set; get; }
        public ICommand RemoveSelected { private set; get; }

        public ObservableCollection<UnitViewModel> Units { get; set; }

        #region ViewModel Collection
        private void Units_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Unit unit in e.NewItems)
                    {
                        Units.Insert(e.NewStartingIndex, new UnitViewModel(unit));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Unit unit in e.OldItems)
                    {
                        Units.Remove(Units.Where(x => x.Model == unit).Single());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Units.Clear();
                    break;
            }
        }
        #endregion

        public UnitTypes UnitType { get; set; }

        System.Random random = new System.Random();
        private void addRandom()
        {
            //for (int i = 0; i < 100; ++i)
            {
                var unit = new Unit { World = Model };
                unit.Position.X = random.Next(-100, +100);
                unit.Position.Y = random.Next(-100, +100);
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
                    unit.Position.X = x*10;
                unit.Position.Y = y*10;
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
            unit.Behavior = Model.FindBehaviortreeByName("Hello World Printer");
            Model.Units.Add(unit);
        }
        private void removeSelected()
        {
            foreach (var unit in Model.Units.Where(a => a.Selected).ToList())
            {
                Model.Units.Remove(unit);
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

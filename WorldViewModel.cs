using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Prototype
{
    public class WorldViewModel : ViewModelBase<World>
    {
        public List<UnitViewModel> HighlightedUnits { get; set; } = new List<UnitViewModel>();
        public ObservableCollection<UnitViewModel> Units { get; set; } = new ObservableCollection<UnitViewModel>();

        public WorldViewModel(World model)
        {
            Model = model;

            Model.Units.CollectionChanged += Units_CollectionChanged;
            foreach (var item in Model.Units)
            {
                Units.Add(new UnitViewModel(item as Unit));
            }
        }

        private void Units_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        Units.Add(new UnitViewModel(item as Unit));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        Units.RemoveAt(e.OldStartingIndex + i);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Units.Clear();
                    break;
            }
        }

        internal void Update()
        {
            Model.Update(Playing);
        }

        private bool playing = false;
        public bool Playing
        {
            get => playing;
            set
            {
                if (playing != value)
                {
                    playing = value;
                    RaisePropertyChanged("Playing");
                }
            }
        }
    }
}

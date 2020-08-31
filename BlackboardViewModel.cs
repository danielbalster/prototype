using Prototype.Behaviortree;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Prototype
{
    public class BlackboardEntry : ViewModel
    {
        private object value;
        public Guid Id { get; set; }
        public string Key { get; set; }
        public object Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }
    }

    public class BlackboardViewModel : ViewModelBase<Blackboard>
    {
        public BlackboardViewModel(Blackboard bb)
        {
            Model = bb;

            //bb.store.CollectionChanged
            Sync();

        }

        public void Sync()
        {
            foreach ( var de in Model.store )
            {
                if ("$#?!".Contains(de.Key.Item2)) continue;
                var found = false;
                foreach( var e in Entries )
                {
                    if (e.Id==de.Key.Item1 && e.Key== de.Key.Item2)
                    {
                        found = true;
                        e.Value = de.Value;
                    }
                }
                if (!found)
                {
                    Entries.Add(new BlackboardEntry { Id = de.Key.Item1, Key = de.Key.Item2, Value = de.Value });
                }
            }
        }

        public ObservableCollection<BlackboardEntry> Entries { get; set; } = new ObservableCollection<BlackboardEntry>();
    }
}

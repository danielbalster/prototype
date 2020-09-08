using System;
using System.ComponentModel;

namespace Prototype
{
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }

    public class ViewModelBase<T> : Notifier
    {
        public T Model { get; set; }
    }

}

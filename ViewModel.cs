using System;
using System.ComponentModel;

namespace Prototype
{
    public class ViewModelBase<T> : INotifyPropertyChanged
    {
        public T Model { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}

using System;
using System.ComponentModel;

namespace Prototype
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class ViewModelBase<T> : ViewModel
    {
        public T Model { get; set; }
    }

}

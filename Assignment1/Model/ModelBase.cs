using System;
using System.ComponentModel;

namespace Assignment1.Model
{
    [Serializable]
    public class ModelBase : INotifyPropertyChanged
    {
        [field: NonSerialized()] // required for this class to be serializable (excluding this property)
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string p)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p));
            }
        }
    }
}

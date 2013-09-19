using System;
using System.ComponentModel;

namespace AssignmentsDAT510.Model
{
    [Serializable]
    public class ModelBase : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()] // required for this class to be serializable (excluding this property)
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

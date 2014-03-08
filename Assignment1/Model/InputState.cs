using System;

namespace Assignment1.Model
{
    /// <summary>
    /// This class represents the current input state of the application
    /// </summary>
    [Serializable]
    public class InputState : ModelBase
    {
        private string _key;
        private string _input;
        private bool _isEncrypting;

        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged("Input");
            }
        }

        public bool IsEncrypting
        {
            get { return _isEncrypting; }
            set
            {
                _isEncrypting = value;
                OnPropertyChanged("IsEncrypting");
                OnPropertyChanged("Action");
            }
        }

        public string Action
        {
            get { return IsEncrypting ? "Encrypt" : "Decrypt"; }
        }

        public string LoggedFormatted
        {
            get
            {
                return string.Format("{0: d. MMM yyyy kl. HH:mm:ss}", Logged);
            }
        }

        public DateTime Logged { get; set; }
    }
}

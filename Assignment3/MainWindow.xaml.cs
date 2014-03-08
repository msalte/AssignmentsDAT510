using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using Assignment3.Interface;
using Assignment3.IO;
using Assignment3.Model;

namespace Assignment3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged, INotifyParametersChanged, INotifyMessageEdited,
                                      INotifyDSAErrorOccurred
    {
        private string _input;

        private bool _shouldShowPerMessageParams;
        private Message _selectedMessage;
        public event PropertyChangedEventHandler PropertyChanged;

        public string P
        {
            get { return IsPerMessageParams ? SelectedMessage.GlobalKeys.P.ToString() : DSA.GlobalKeys.P.ToString(); }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.GlobalKeys.P = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                    else DSA.GlobalKeys.P = BigInteger.Parse(value);
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("P"));
                }
            }
        }

        public string Q
        {
            get { return IsPerMessageParams ? SelectedMessage.GlobalKeys.Q.ToString() : DSA.GlobalKeys.Q.ToString(); }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.GlobalKeys.Q = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                    else DSA.GlobalKeys.Q = BigInteger.Parse(value);
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("Q"));
                }
            }
        }

        public string G
        {
            get { return IsPerMessageParams ? SelectedMessage.GlobalKeys.G.ToString() : DSA.GlobalKeys.G.ToString(); }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.GlobalKeys.G = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                    else DSA.GlobalKeys.G = BigInteger.Parse(value);
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("G"));
                }
            }
        }

        public string X
        {
            get { return IsPerMessageParams ? SelectedMessage.UserPrivateKey.X.ToString() : DSA.UserPrivateKey.X.ToString(); }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.UserPrivateKey.X = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                    else DSA.UserPrivateKey.X = BigInteger.Parse(value);
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("X"));
                }
            }
        }

        public string Y
        {
            get { return IsPerMessageParams ? SelectedMessage.UserPublicKey.Y.ToString() : DSA.UserPublicKey.Y.ToString(); }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.UserPublicKey.Y = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                    else DSA.UserPublicKey.Y = BigInteger.Parse(value);
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("Y"));
                }
            }
        }

        public string K
        {
            get { return IsPerMessageParams ? SelectedMessage.SecretNumber.K.ToString() : DSA.SecretNumber.K.ToString(); }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.SecretNumber.K = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                    else DSA.SecretNumber.K = BigInteger.Parse(value);
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("K"));
                }
            }
        }

        public string R
        {
            get { return IsPerMessageParams ? SelectedMessage.Signature.R.ToString() : "-"; }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.Signature.R = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("R"));
                }
            }
        }

        public string S
        {
            get { return IsPerMessageParams ? SelectedMessage.Signature.S.ToString() : "-"; }
            set
            {
                try
                {
                    if (IsPerMessageParams)
                    {
                        SelectedMessage.Signature.S = BigInteger.Parse(value);
                        OnMessageEdited();
                    }
                }
                catch (FormatException)
                {
                    DSAErrorOccurred(CreateInputParseExceptionError("S"));
                }
            }
        }


        public MainWindow()
        {
            StoredMessages = MessageIO.Read();

            DSA = new DSA(this, this);
            DSA.GenerateParameters();

            InitializeComponent();
        }

        public ObservableCollection<Message> StoredMessages { get; set; }

        public bool IsPerMessageParams
        {
            get { return _shouldShowPerMessageParams && SelectedMessage != null; }
            set
            {
                _shouldShowPerMessageParams = value;
                OnPropertyChanged("IsGenerateParamsButtonEnabled");
            }
        }

        public IEnumerable<string> ParamsComboItems
        {
            get
            {
                return new List<string>
                    {
                        "This session",
                        "Selected message"
                    };
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

        public DSA DSA { get; set; }

        private Message SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                _selectedMessage = value;
                NotifyParamsChanged();
            }
        }

        private void SignAndStoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                return;
            }

            var signedMessage = DSA.SignMessage(Input);

            if (signedMessage == null) return;

            StoredMessages.Insert(0, signedMessage);

            MessageIO.Write(StoredMessages);

            Input = string.Empty;
        }

        private void StoredMessagesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedMessage = (Message) e.AddedItems[0];
        }

        private void EditMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMessage == null)
            {
                return;
            }

            new EditMessageWindow(this, SelectedMessage, this).ShowDialog();
        }

        private void VerifyMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMessage == null)
            {
                return;
            }

            var verified = SelectedMessage.Verify();

            new VerificationWindow(verified, this).ShowDialog();
        }

        private void OnPropertyChanged(string p)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p));
            }
        }

        private void ParamsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsPerMessageParams = !((string) e.AddedItems[0]).Equals("This session");

            NotifyParamsChanged();
        }

        public void OnParametersChanged()
        {
            NotifyParamsChanged();
        }

        public void OnMessageEdited()
        {
            MessageIO.Write(StoredMessages);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NotifyParamsChanged()
        {
            OnPropertyChanged("P");
            OnPropertyChanged("Q");
            OnPropertyChanged("G");
            OnPropertyChanged("X");
            OnPropertyChanged("Y");
            OnPropertyChanged("K");
            OnPropertyChanged("R");
            OnPropertyChanged("S");
        }

        public void DSAErrorOccurred(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error");
        }

        private string CreateInputParseExceptionError(string parameter)
        {
            return "Inputted parameter (" + parameter + ") cannot be parsed to a number!";
        }
    }
}
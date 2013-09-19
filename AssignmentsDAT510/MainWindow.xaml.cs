using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using AssignmentsDAT510.Interface;
using AssignmentsDAT510.Model;
using AssignmentsDAT510.Util;
using Microsoft.Win32;

namespace AssignmentsDAT510
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged, IProgressCallback, ILogSelectionCallback
    {
        private string _output;
        private string _inputValidationMessage;
        private string _inputValidationMessageColor;
        private bool _isLoggingEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public InputState InputState { get; set; }
        public List<string> ActionList { get; set; }
        public ProgressBarState ProgressBarState { get; set; }
        private ObservableCollection<InputState> Log { get; set; }

        public static int KeyInputMaxLength
        {
            get { return FeistelCipher.KeySize/BitUtils.ByteSize; }
        }

        public MainWindow()
        {
            Initialize();
            InitializeComponent();
        }

        public bool IsLoggingEnabled
        {
            get { return _isLoggingEnabled; }
            set
            {
                _isLoggingEnabled = value;
                OnPropertyChanged("IsLoggingEnabled");
            }
        }

        public string InputValidationMessage
        {
            get { return _inputValidationMessage; }
            set
            {
                _inputValidationMessage = value;
                OnPropertyChanged("InputValidationMessage");
            }
        }

        public string InputValidationMessageColor
        {
            get { return _inputValidationMessageColor; }
            set
            {
                _inputValidationMessageColor = value;
                OnPropertyChanged("InputValidationMessageColor");
            }
        }

        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged("Output");
            }
        }

        private void OnPropertyChanged(string p)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p));
            }
        }

        private void Initialize()
        {
            InputState = new InputState();
            ActionList = new List<string> {"Encrypt", "Decrypt"};
            ProgressBarState = new ProgressBarState();
            IsLoggingEnabled = true;
            Log = InputStateIO.Read();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCryptography();
        }

        private async void ExecuteCryptography()
        {
            var feistelCipher = new FeistelCipher(this);

            if (!IsInputValid())
            {
                return;
            }

            await Task.Run(() => feistelCipher.Execute(InputState.Input, InputState.Key, InputState.IsEncrypting));
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            InputState.Input = Output;
            Output = string.Empty;
        }

        public void OnBegin(int maxValue)
        {
            ProgressBarState.Max = maxValue;
            OnPropertyChanged("ProgressBarState");
        }

        public void OnUpdate(int progress)
        {
            ProgressBarState.Current += progress;
            OnPropertyChanged("ProgressBarState");
        }

        public void OnComplete(string output)
        {
            if (IsLoggingEnabled)
            {
                // As this method is called from an async thread, it is required
                // to tell the application to invoke the LogInputState() method
                // from the dispatcher thread as that is required to modify the
                // ObservableCollection.

                Application.Current.Dispatcher.Invoke(LogInputState);
            }

            Output = output;
        }

        /// <summary>
        /// Logs the current input state. Must be called from the Dispatcher thread.
        /// </summary>
        private void LogInputState()
        {
            Log.Insert(0, new InputState
            {
                Input = InputState.Input,
                Key = InputState.Key,
                IsEncrypting = InputState.IsEncrypting,
                Logged = DateTime.Now
            });

            InputStateIO.Write(Log);

            //ClearInputState();
        }

        private void ClearInputState()
        {
            InputState.Key = string.Empty;
            InputState.Input = string.Empty;

            InputValidationMessage = string.Empty;
        }

        public void OnError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "An error occurred", MessageBoxButton.OK, MessageBoxImage.Error);

            ResetProgress();
        }

        private void ActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputState.IsEncrypting = e.AddedItems[0].ToString().Equals("Encrypt");

            ResetProgress();
        }

        private void KeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyTextBox = (TextBox) e.Source;

            InputState.Key = keyTextBox.Text;

            IsInputValid();
            ResetProgress();
        }

        private void ResetProgress()
        {
            ProgressBarState.Max = 100;
            ProgressBarState.Current = 0;

            OnPropertyChanged("ProgressBarState");
        }

        private bool IsInputValid()
        {
            if (string.IsNullOrEmpty(InputState.Key) || string.IsNullOrWhiteSpace(InputState.Input) ||
                InputState.Key.Length != KeyInputMaxLength)
            {
                InputValidationMessage = "Key length must be " + KeyInputMaxLength + " characters (" +
                                         FeistelCipher.KeySize + " bits) and input length must be > 0";
                InputValidationMessageColor = Colors.Red.ToString();

                return false;
            }

            InputValidationMessage = "Input is OK";
            InputValidationMessageColor = Colors.Green.ToString();

            return true;
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var inputTextBox = (TextBox) e.Source;
            InputState.Input = inputTextBox.Text;

            IsInputValid();
            ResetProgress();
        }

        private void OpenLogWindow()
        {
            new LogWindow(Log, this).Show();
        }

        private void OpenLogMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenLogWindow();
        }

        private void OpenLogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenLogWindow();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void OnSelected(InputState selectedInputState)
        {
            if (selectedInputState == null) return;

            InputState.Input = selectedInputState.Input;
            InputState.Key = selectedInputState.Key;
            InputState.IsEncrypting = selectedInputState.IsEncrypting;
        }

        private void ClearInputButton_Click(object sender, RoutedEventArgs e)
        {
            ClearInputState();
        }

        private void ImportFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Import File"
            };

            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                var fileName = openFileDialog.FileName;

                var fileContent = File.ReadAllText(fileName);

                InputState.Input = fileContent;
            }
        }
    }
}
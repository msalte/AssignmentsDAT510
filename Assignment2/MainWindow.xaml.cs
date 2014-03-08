using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Assignment2.Interface;
using Assignment2.Model;
using Assignment2.Utils;

namespace Assignment2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :
        ICryptographyProgressCallback,
        INotifyPropertyChanged,
        IGenerationProgressCallback
    {
        private readonly RSA _rsa;

        private bool _isCryptographyInProgress;
        private bool _isGenerationInProgress;
        private bool _isRSAParametersLockChecked;
        private bool _isParametersSetManually;
        private bool _isInputTypeAutoChecked;
        private bool _isInputTypeBinaryChecked;
        private bool _isInputTypeTextChecked;
        private bool _isInputTooLong;

        private string _input;
        private string _output;
        private string _generationTimeElapsedMessage;

        private int _primeLength;

        public string MessageBackgroundColor
        {
            get
            {
                if (!IsParametersGenerated && !IsParametersSetManually || IsInputTooLong)
                {
                    return Colors.LightCoral.ToString();
                }

                if (!IsValidInput)
                {
                    return Colors.WhiteSmoke.ToString();
                }

                return Colors.LightGreen.ToString();
            }
        }

        public bool IsInputBinary
        {
            get
            {
                if (IsInputTypeBinaryChecked) return true;
                if (IsInputTypeTextChecked) return false;

                return Input != null && !Input.Any(bit => bit != '1' && bit != '0');
            }
        }

        public string RSAParametersColor
        {
            get { return IsRSAParametersLockChecked ? Colors.Gray.ToString() : Colors.Black.ToString(); }
        }

        public bool IsRSAParametersLockChecked
        {
            get { return _isRSAParametersLockChecked; }
            set
            {
                _isRSAParametersLockChecked = value;
                OnPropertyChanged("IsRSAParametersLockChecked");
                OnPropertyChanged("RSAParametersColor");
            }
        }

        public bool IsInputTypeAutoChecked
        {
            get { return _isInputTypeAutoChecked; }
            set
            {
                _isInputTypeAutoChecked = value;

                OnPropertyChanged("IsInputTypeAutoChecked");
            }
        }

        public bool IsInputTypeBinaryChecked
        {
            get { return _isInputTypeBinaryChecked; }
            set
            {
                _isInputTypeBinaryChecked = value;

                OnPropertyChanged("IsInputTypeBinaryChecked");
            }
        }

        public bool IsInputTypeTextChecked
        {
            get { return _isInputTypeTextChecked; }
            set
            {
                _isInputTypeTextChecked = value;

                OnPropertyChanged("IsInputTypeTextChecked");
            }
        }

        public bool IsEncrypting { get; set; }
        public bool IsGenerationComplete { get; set; }

        public string GenerationTimeElapsedMessage
        {
            get { return _generationTimeElapsedMessage; }
            set
            {
                _generationTimeElapsedMessage = value;
                OnPropertyChanged("GenerationTimeElapsedMessage");
            }
        }

        public string Message
        {
            get
            {
                if (!IsParametersGenerated && !IsParametersSetManually)
                {
                    return "You need to generate or input RSA parameters to begin.";
                }

                if (!IsValidInput)
                {
                    return "Enter your input message to encrypt/decrypt.";
                }

                return IsValidInput && !IsInputTooLong
                    ? "The inputted message is valid, press OK to execute action."
                    : "The inputted message size exceeds the maximum for the current n parameter.";
            }
        }

        private async void ComputeInputLength()
        {
            if (Input == null || RSAParameters == null || RSAParameters.N == null)
            {
                IsInputTooLong = false;

                return;
            }
            
            var inputSize = await Task.Run(() => IsInputBinary
                ? BigNumberUtils.FromBinaryString(Input)
                : BigNumberUtils.FromPlainText(Input));

            IsInputTooLong = await Task.Run(() => inputSize.GreaterThan(RSAParameters.N));
        }

        public bool IsInputTooLong
        {
            get { return _isInputTooLong; }
            set
            {
                _isInputTooLong = value;
                OnPropertyChanged("Message");
                OnPropertyChanged("MessageBackgroundColor");
                OnPropertyChanged("CanClickOK");
            }
        }

        public int PrimeLength
        {
            get { return _primeLength == 0 ? 15 : _primeLength; } // return 15 as default length
            set
            {
                _primeLength = value;
                OnPropertyChanged("PrimeLength");
            }
        }

        public RSAParameters RSAParameters { get; set; }

        public bool IsParametersGenerated { get; set; }

        public bool IsParametersSetManually
        {
            get { return _isParametersSetManually; }
            set
            {
                _isParametersSetManually = value;
                OnPropertyChanged("CanClickOK");
                OnPropertyChanged("Message");
                OnPropertyChanged("MessageBackgroundColor");
            }
        }

        public IEnumerable<string> ActionList
        {
            get { return new[] {"Encrypt", "Decrypt"}; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;

                ComputeInputLength();

                OnPropertyChanged("Input");
                OnPropertyChanged("CanClickOK");
                OnPropertyChanged("Message");
                OnPropertyChanged("MessageBackgroundColor");
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

        public bool IsGenerationInProgress
        {
            get { return _isGenerationInProgress; }
            set
            {
                _isGenerationInProgress = value;
                OnPropertyChanged("IsGenerationInProgress");
                OnPropertyChanged("CanClickOK");
                OnPropertyChanged("CanClickGenerate");
            }
        }

        public bool CanClickOK
        {
            get
            {
                return !IsCryptographyInProgress && !IsInputTooLong && !IsGenerationInProgress && IsValidInput &&
                       (IsGenerationComplete || IsParametersSetManually);
            }
        }

        public bool CanClickGenerate
        {
            get { return !IsCryptographyInProgress && !IsGenerationInProgress; }
        }

        public bool IsValidInput
        {
            get { return !string.IsNullOrEmpty(Input); }
        }

        public bool IsCryptographyInProgress
        {
            get { return _isCryptographyInProgress; }
            set
            {
                _isCryptographyInProgress = value;
                OnPropertyChanged("IsCryptographyInProgress");
                OnPropertyChanged("CanClickOK");
                OnPropertyChanged("CanClickGenerate");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            _rsa = new RSA(this, this);

            RSAParameters = new RSAParameters();

            IsEncrypting = true;
            IsRSAParametersLockChecked = true;
            IsInputTypeAutoChecked = true;
        }

        public void OnCryptographyBegin()
        {
            IsCryptographyInProgress = true;
        }

        public void OnCryptographyError(string errorMessage)
        {
            Output = errorMessage;

            IsCryptographyInProgress = false;
        }

        public void OnCryptographyComplete(string result)
        {
            IsCryptographyInProgress = false;

            Output = result;
        }

        private void OnPropertyChanged(string p)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p));
            }
        }

        private void MoveUpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Input = Output;

            Output = string.Empty;
        }

        private void MoveDownButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Output = Input;

            Input = string.Empty;
        }

        private void InpuTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var inputTextBox = (TextBox) e.Source;

            Input = inputTextBox.Text;
        }

        private void GenerateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BeginGenerateRSAParameters();
        }

        private async void BeginGenerateRSAParameters()
        {
            await Task.Run(() => _rsa.GeneratePrimes(PrimeLength));
        }

        public void OnGenerationBegin()
        {
            GenerationTimeElapsedMessage = "Generating...";
            IsGenerationInProgress = true;
            IsGenerationComplete = false;

            OnPropertyChanged("CanClickOK");
        }

        public void OnGenerationComplete(Stopwatch timeElapsed, RSAParameters generatedParameters)
        {
            IsParametersGenerated = true;
            IsGenerationInProgress = false;
            IsGenerationComplete = true;

            RSAParameters = generatedParameters;

            SetTimeElapsed(timeElapsed);

            OnPropertyChanged("RSAParameters");

            ComputeInputLength();
        }

        private void SetTimeElapsed(Stopwatch timeElapsed)
        {
            var ms = timeElapsed.ElapsedMilliseconds;

            var msg = "Generated in ";

            var seconds = (ms/1000)%60;
            var minutes = ((ms/1000)/60)%60;
            var hours = ((ms/1000)/60)/60;

            if (hours > 0)
            {
                msg += hours + " hour(s), ";
            }

            if (minutes > 0)
            {
                msg += minutes + " minute(s), ";
            }

            if (seconds > 0)
            {
                msg += seconds + " second(s)";
            }

            if (seconds <= 0)
            {
                msg += ms + " millisecond(s)";
            }

            GenerationTimeElapsedMessage = msg;
        }

        public void OnGenerationError(string errorMessage)
        {
            IsGenerationInProgress = false;
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BeginExecuteCryptography();
        }

        private async void BeginExecuteCryptography()
        {
            await Task.Run(() => _rsa.ExecuteCryptography(RSAParameters, IsEncrypting, Input, IsInputBinary));
        }

        private void ActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsEncrypting = e.AddedItems[0].ToString().Equals("Encrypt");
        }

        private void RsaNTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsRSAParametersLockChecked)
            {
                var manualN = ((TextBox) e.Source).Text;

                RSAParameters.N = new BigNumber(manualN);

                ComputeInputLength();

                IsParametersSetManually = true;
                return;
            }

            IsParametersSetManually = false;
        }

        private void RsaDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsRSAParametersLockChecked)
            {
                var manualD = ((TextBox) e.Source).Text;

                RSAParameters.D = new BigNumber(manualD);

                IsParametersSetManually = true;
                return;
            }

            IsParametersSetManually = false;
        }

        private void RsaETextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsRSAParametersLockChecked)
            {
                var manualE = ((TextBox) e.Source).Text;

                RSAParameters.E = new BigNumber(manualE);

                IsParametersSetManually = true;
                return;
            }

            IsParametersSetManually = false;
        }

        private void InputTypeAutoRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ComputeInputLength();
        }

        private void InputTypeBinaryRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ComputeInputLength();
        }

        private void InputTypeTextRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ComputeInputLength();
        }
    }
}
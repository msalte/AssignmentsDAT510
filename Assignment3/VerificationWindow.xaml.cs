using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Assignment3
{
    /// <summary>
    /// Interaction logic for VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow
    {
        public VerificationWindow(bool isVerified, Window parent)
        {
            Left = parent.Left + parent.Left / 2;
            Top = parent.Top + ((parent.Height - ActualHeight) / 2);

            IsVerified = isVerified;

            InitializeComponent();
        }

        public bool IsVerified { get; set; }

        public string VerificationMessage
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("The selected message ");

                sb.Append(IsVerified ? "passed" : "did not pass");

                sb.Append(" the verification test!");


                return sb.ToString();
            }
        }

        public string VerificationMessageColor
        {
            get
            {
                return IsVerified ? Colors.LightGreen.ToString() : Colors.LightCoral.ToString();
            }
        }

        public string VerificationIcon
        {
            get
            {
                return IsVerified ? "/Images/Pass.png" : "/Images/Fail.png";
            }
        }
    }
}

using System.Windows;
using Assignment3.Interface;
using Assignment3.Model;

namespace Assignment3
{
    /// <summary>
    /// Interaction logic for EditMessageWindow.xaml
    /// </summary>
    public partial class EditMessageWindow
    {
        private readonly INotifyMessageEdited _callback;

        public EditMessageWindow(INotifyMessageEdited callback, Message message, Window parent)
        {
            _callback = callback;

            Left = parent.Left + parent.Left/2;
            Top = parent.Top + ((parent.Height - ActualHeight)/2);

            Message = message;

            InitializeComponent();
        }

        public Message Message { get; set; }

        private void SaveEditButton_Click(object sender, RoutedEventArgs e)
        {
            _callback.OnMessageEdited();

            Close();
        }
    }
}
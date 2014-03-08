using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Assignment1.Interface;
using Assignment1.Model;
using Assignment1.Util;

namespace AssignmentsDAT510
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow
    {
        private readonly ILogSelectionCallback _logSelectionCallback;
        public ObservableCollection<InputState> Log { get; set; }
        private InputState SelectedInputState { get; set; }

        public LogWindow(ObservableCollection<InputState> log, ILogSelectionCallback callback)
        {
            _logSelectionCallback = callback;

            Log = log;
            InitializeComponent();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            InputStateIO.Write(Log);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            SetSelectedInputState();
        }

        private void LogListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedInputState = (InputState) e.AddedItems[0];
        }

        private void LogListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Close();
            SetSelectedInputState();
        }

        private void SetSelectedInputState()
        {
            _logSelectionCallback.OnSelected(SelectedInputState);
        }
    }
}

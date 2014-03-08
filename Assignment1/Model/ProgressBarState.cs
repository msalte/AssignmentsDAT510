namespace Assignment1.Model
{
    /// <summary>
    /// This class represents the current progress bar's state
    /// </summary>
    public class ProgressBarState
    {
        public int Max { get; set; }
        public int Current { get; set; }

        public ProgressBarState()
        {
            Max = 100;
            Current = 0;
        }
    }
}
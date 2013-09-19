namespace AssignmentsDAT510.Model
{
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
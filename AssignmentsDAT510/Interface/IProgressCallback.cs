namespace AssignmentsDAT510.Interface
{
    /// <summary>
    /// An interface allowing the async thread executing the encryption task to communicate with the main thread (GUI)
    /// </summary>
    public interface IProgressCallback
    {
        /// <summary>
        /// Should be called when the encryption task begins
        /// </summary>
        /// <param name="maxValue">The max value for the progress bar</param>
        void OnBegin(int maxValue);
        /// <summary>
        /// Should be called when the encryption task wants to publish a progress update
        /// </summary>
        /// <param name="progress">The progress value to increment the progress bar</param>
        void OnUpdate(int progress);
        /// <summary>
        /// Should be called when the encryption task is complete
        /// </summary>
        /// <param name="output">The output string resulting from the encryption task</param>
        void OnComplete(string output);
        /// <summary>
        /// Should be called if an error occurrs during the encryption task
        /// </summary>
        /// <param name="errorMessage">The corresponding error message associated with the specific error</param>
        void OnError(string errorMessage);
    }
}

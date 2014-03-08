namespace Assignment2.Interface
{
    public interface ICryptographyProgressCallback
    {
        void OnCryptographyBegin();
        void OnCryptographyError(string errorMessage);
        void OnCryptographyComplete(string result);
    }
}
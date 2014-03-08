using System.Diagnostics;
using Assignment2.Model;

namespace Assignment2.Interface
{
    public interface IGenerationProgressCallback
    {
        void OnGenerationBegin();
        void OnGenerationComplete(Stopwatch timeElapsed, RSAParameters generatedParameters);
        void OnGenerationError(string errorMessage);
    }
}
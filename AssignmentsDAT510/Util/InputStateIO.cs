using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AssignmentsDAT510.Model;

namespace AssignmentsDAT510.Util
{
    /// <summary>
    /// This class is heavily influenced by the official documentation regarding C# object serialization.
    /// Source: http://msdn.microsoft.com/en-us/library/4abbf6k0.aspx
    /// </summary>
    public static class InputStateIO
    {
        private const string FilePath = "InputStateLog.log";

        public static void Write(ObservableCollection<InputState> toWrite)
        {
            var formatter = new BinaryFormatter();

            using (var stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, toWrite);
            }
        }

        public static  ObservableCollection<InputState> Read()
        {
            var formatter = new BinaryFormatter();

            try
            {
                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return (ObservableCollection<InputState>)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                // Any exception should simply return an empty collection
                return new ObservableCollection<InputState>();
            }
            
        }
    }
}

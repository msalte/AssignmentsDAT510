using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assignment3.Model;

namespace Assignment3.IO
{
    /// <summary>
    /// This class is used to provide capability of storing of messages.
    /// 
    /// This class is heavily influenced by the official documentation regarding C# object serialization.
    /// Source: http://msdn.microsoft.com/en-us/library/4abbf6k0.aspx
    /// </summary>
    public static class MessageIO
    {
        private const string FilePath = "StoredMessages.log";

        public static void Write(ObservableCollection<Message> toWrite)
        {
            var formatter = new BinaryFormatter();

            using (var stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, toWrite);
            }
        }

        public static ObservableCollection<Message> Read()
        {
            var formatter = new BinaryFormatter();

            try
            {
                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return (ObservableCollection<Message>)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                // Any exception should simply return an empty collection
                return new ObservableCollection<Message>();
            }

        }
    }
}

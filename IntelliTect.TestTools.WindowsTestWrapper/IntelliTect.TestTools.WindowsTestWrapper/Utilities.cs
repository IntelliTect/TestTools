using System;
using System.IO;
using System.Threading;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    public static class Utilities
    {
        //TODO: Think about moving or renaming this. Handle exception cases better than below.
        /// <summary>
        /// Parse a text file and return a string of its contents
        /// </summary>
        /// <param name="path">The full path of the location and file name</param>
        /// <returns>Returns the text of the chosen file</returns>
        public static string ParseTextFile(string path)
        {
            string text = null;
            int retryAttempts = 0;
            do
            {
                text = File.ReadAllText(path);
                Thread.Sleep( 100 );
                retryAttempts++;
            } while ( retryAttempts < 10 );
            return text;
        }

        /// <summary>
        /// Deletes a file given a location
        /// </summary>
        /// <param name="location">The location of the file without a slash at the end</param>
        /// <param name="fileName">The name and extension of the file</param>
        public static void DeleteDocument(string location, string fileName)
        {
            int retryAttempts = 0;
            do
            {
                File.Delete(Path.Combine(location, fileName));
                Thread.Sleep( 100 );
                if (!File.Exists(Path.Combine(location, fileName)))
                {
                    break;
                }
                retryAttempts++;

            } while ( retryAttempts < 10 );
            if ( File.Exists(Path.Combine(location, fileName)) )
            {
                throw new Exception("File not properly deleted");
            }
        }
    }
}

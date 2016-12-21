using System;
using System.IO;
using System.Threading;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    public class Utilities
    {
        /// <summary>
        /// Parse a text file and return a string of its contents
        /// </summary>
        /// <param name="path">The full path of the location and file name</param>
        /// <returns>Returns the text of the chosen file</returns>
        public string ParseTextFile(string path)
        {
            string text = null;
            int retryAttempts = 0;
            while (retryAttempts < 10)
            {
                try
                {
                    text = File.ReadAllText(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read");
                    Console.Write(e.Message);
                }

                if (text != null)
                {
                    break;
                }

                Thread.Sleep(500);
                retryAttempts++;
            }
            return text;
        }

        /// <summary>
        /// Deletes a file given a location
        /// </summary>
        /// <param name="location">The location of the file without a slash at the end</param>
        /// <param name="filename">The name and extension of the file</param>
        public void DeleteDocument(string location, string filename)
        {
            try
            {
                File.Delete(Path.Combine(location, filename));
            }
            catch (Exception e)
            {
                Console.WriteLine("File could not be deleted");
                Console.Write(e.Message);
            }
        }
    }
}

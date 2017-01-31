using System;
using System.IO;
using System.Threading;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    public static class Utilities
    {
        /// <summary>
        /// Parse a text file and return a string of its contents
        /// </summary>
        /// <param name="path">The full path of the location and file name</param>
        /// <returns>Returns the text of the chosen file</returns>
        public static string ParseTextFile(string path)
        {
            string text = null;
            int retryAttempts = 0;
            while (retryAttempts < 10)
            {
                try
                {
                    text = File.ReadAllText(path);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("The file could not be found");
                    Console.Write(e.Message);
                    throw;
                }

                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read");
                    Console.Write(e.Message);
                    throw;
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
        /// <param name="fileName">The name and extension of the file</param>
        public static void DeleteDocument(string location, string fileName)
        {
            try
            {
                File.Delete(Path.Combine(location, fileName));
            }
            catch (Exception e)
            {
                Console.WriteLine("Issue with DeleteDocument method");
                Console.Write(e.Message);
                throw;
            }
        }
    }
}

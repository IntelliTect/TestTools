using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IntelliTect.TestTools.Console
{
    static public class ConsoleAssert
    {

        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        static public void Expect(string expected, Action action, bool replaceCRLF = true)
        {
            Expect(expected, action, (left, right) => left == right, replaceCRLF);
        }

        static public void ExpectNoTrimOutput(string expected, Action action)
        {
            Expect(expected, action, (left, right) => left == right, false);
        }

        static public void Expect(string expected, Func<string[], int> func, int expectedReturn = default(int), params string[] args)
        {
            Expect<int>(expected, func, expectedReturn, args);
        }

        static public void Expect<T>(string expected, Func<string[], T> func, T expectedReturn = default(T), params string[] args)
        {
            T @return = default(T);
            Expect(expected, () => { @return = func(args); });

            if (!expectedReturn.Equals(@return))
            {
                throw new Exception($"The value returned from {nameof(func)} ({@return}) was not the { nameof(expectedReturn) }({expectedReturn}) value.");
            }
        }

        static public void Expect(string expected, Func<int> func, int expectedReturn)
        {
            Expect(expected, (args) => func(), expectedReturn);
        }

        static public void Expect<T>(string expected, Func<T> func, T expectedReturn)
        {
            Expect(expected, (args) => func(), expectedReturn);
        }

        static public void Expect(string expected, Action<string[]> func, params string[] args) =>
            Expect(expected, () => func(args));

        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        static private void Expect(string expected, Action action, Func<string, string, bool> comparisonOperator, bool replaceCRLF = true)
        {

            string[] data = Parse(expected);

            string input = data[0];
            string output = data[1];

            Execute(input, output, action, comparisonOperator, replaceCRLF);
        }

        static private Func<string, string, bool> LikeOperator =
            (expected, output) => output.IsLike(expected);

        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        static public void ExpectLike(string expected, Action action)
        {
            Expect(expected, action, LikeOperator);
        }

        static public string ReplaceCRLF(string input)
        {
            // first, replace any \r\n with environment.newline
            //input = input.Replace("\r\n", Environment.NewLine);

            // now, replace any \n, with Environment.NewLine
            //Regex regex = new Regex(".*[^\r](\n)");
            input = Regex.Replace(input, "\r?\n", Environment.NewLine);

            //if(matches != null)
            //{
            //    foreach(Match match in matches)
            //    {
            //        input = input.Replace(match.Groups[1].Value, Environment.NewLine);
            //    }
            //}

            return input;
        }
        /// <summary>
        /// Executes the unit test while providing console input.
        /// </summary>
        /// <param name="givenInput">Input which will be given</param>
        /// <param name="expectedOutput">The expected output</param>
        /// <param name="action">Action to be tested</param>
        /// <param name="areEquivalentOperator">delegate for comparing the expected from actual output.</param>
        static private void Execute(string givenInput, string expectedOutput, Action action,
            Func<string, string, bool> areEquivalentOperator, bool replaceCRLF = true)
        {
            string output = Execute(givenInput, action, replaceCRLF);

            if(replaceCRLF)
            {
                expectedOutput = ReplaceCRLF(expectedOutput);
                if (expectedOutput.EndsWith(Environment.NewLine)) expectedOutput = expectedOutput.Substring(0, expectedOutput.Length - Environment.NewLine.Length);
                //expectedOutput = expectedOutput.Replace("\r\n", Environment.NewLine);
            }
            AssertExpectation(expectedOutput, output, areEquivalentOperator);

        }
        private static void AssertExpectation(string expectedOutput, string output)
        {
            AssertExpectation(expectedOutput, output, (left, right) => left == right);
        }

        private static void AssertExpectation(string expectedOutput, string output, Func<string, string, bool> areEquivalentOperator)
        {
            bool failTest = !areEquivalentOperator(expectedOutput, output);
            if (failTest)
            {
                throw new Exception(GetMessageText(expectedOutput, output));
            }
        }

        readonly static object ExecuteLock = new object();

        public static string Execute(string givenInput, Action action, bool replaceCRLF = true)
        {
            lock (ExecuteLock)
            {
                string output;
                using (TextWriter writer = new StringWriter())
                using (TextReader reader = new StringReader(string.IsNullOrWhiteSpace(givenInput) ? "" : givenInput))
                {
                    System.Console.SetOut(writer);

                    System.Console.SetIn(reader);
                    action();

                    output = writer.ToString();
                    if (replaceCRLF)
                    {
                        output = ReplaceCRLF(output);
                        //output = output.Replace("\r\n", Environment.NewLine);
                    }
                   
                    if (replaceCRLF && output.EndsWith(Environment.NewLine)) output = output.Substring(0, output.Length - Environment.NewLine.Length);
                }

                return output;
            }
        }

        private static string GetMessageText(string expectedOutput, string output)
        {
            string result = "";
            if (expectedOutput.Length <= 2 || output.Length <= 2)
            {
                // Don't display differing lengths.
            }
            else
            {
                result = $"expected: {(int)expectedOutput[expectedOutput.Length - 2]}{(int)expectedOutput[expectedOutput.Length - 1]}{Environment.NewLine}";
                result += $"actual: {(int)output[output.Length - 2]}{(int)output[output.Length - 1]}{Environment.NewLine}";
            }
            char[] wildCardChars = new char[] { '[', ']', '?', '*', '#' };
            if (wildCardChars.Any(c => expectedOutput.Contains(c)))
            {
                result += "NOTE: The expected string contains wildcard charaters: [,],?,*,#" + Environment.NewLine;
            }
            if (expectedOutput.Contains(Environment.NewLine))
            {
                result += string.Join(Environment.NewLine, "AreEqual failed:", "",
                    "Expected:", "-----------------------------------", expectedOutput, "-----------------------------------",
                    "Actual: ", "-----------------------------------", output, "-----------------------------------");
            }
            else
            {
                result += string.Join(Environment.NewLine, "AreEqual failed:",
                    "Expected: ", expectedOutput,
                    "Actual:   ", output);

            }

            int expectedOutputLength = expectedOutput.Length;
            int outputLength = output.Length;
            if (expectedOutputLength != outputLength)
            {
                result += $"{Environment.NewLine}The expected length of {expectedOutputLength} does not match the output length of {outputLength}. ";
                string[] items = (new string[] { expectedOutput, output }).OrderBy(item => item.Length).ToArray();
                if (items[1].StartsWith(items[0]))
                {
                    result += $"{Environment.NewLine}The additional characters are '"
                        + $"{CSharpStringEncode(items[1].Substring(items[0].Length))}'.";
                }
            }
            else
            {
                // Write the output that shows the difference.
                for (int counter = 0; counter < Math.Min(expectedOutput.Length, output.Length); counter++)
                {
                    if (expectedOutput[counter] != output[counter]) // TODO: The message is invalid when using wild cards.
                    {
                        result += Environment.NewLine
                            + $"Character {counter} did not match: "
                            + $"'{CSharpStringEncode(expectedOutput[counter])}' != '{CSharpStringEncode(output[counter])})'";
                        ;
                        break;
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// Convets text into a C# escaped string.
        /// </summary>
        /// <param name="text">The text to encode with C# escape characters.</param>
        /// <returns>The C# encoded value of <paramref name="text"/></returns>
        /// <example>
        /// <code>Console.WriteLine(CSharpStringEncode("    "));</code>
        /// Will display "\t". 
        /// </example>
        private static string CSharpStringEncode(string text)
        {
            return text;
            // TODO: Can we recreate this in .Net Core?
            //string result = "";
            //using (var stringWriter = new StringWriter())
            //{

            //    using (var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp"))
            //    {
            //        provider.GenerateCodeFromExpression(
            //            new System.CodeDom.CodePrimitiveExpression(text), stringWriter, null);
            //        result = stringWriter.ToString();
            //    }
            //}
            //return result;
        }
        private static string CSharpStringEncode(char character) =>
            CSharpStringEncode(character.ToString());


        /// <summary>
        /// This parses a "view" string into two separate strings, one
        /// representing virtual input and the other as expected output
        /// </summary>
        /// <param name="view">
        /// What a user would see in the console, but with input/output tokens.
        /// </param>
        /// <returns>[0] Input, and [1] Output</returns>
        static private string[] Parse(string view)
        {
            // Note: This could definitely be optimized, wanted to try it for experience. RegEx perhaps?
            bool isInput = false;
            char[] viewTemp = view.ToCharArray();

            string input = "";
            string output = "";

            // using the char array, categorize each entry as belonging to "input" or "output"
            for (int i = 0; i < viewTemp.Length; i++)
            {
                if (i != viewTemp.Length - 1)
                {
                    // find "<<" tokens which indicate beginning of input
                    if (viewTemp[i] == '<' && viewTemp[i + 1] == '<')
                    {
                        i++;    // skip the other character in token
                        isInput = true;
                        continue;
                    }
                    // find ">>" tokens which indicate end of input
                    else if (viewTemp[i] == '>' && viewTemp[i + 1] == '>')
                    {
                        i++;    // skip the other character in token
                        isInput = false;
                        continue;
                    }
                }
                if (isInput)
                    input += viewTemp[i].ToString();
                else
                    output += viewTemp[i].ToString();
            }

            return new string[] { input, output };
        }

        // TODO: Should not use LikeOperator by default.  Add a ConsoleTestsComparisonOptions enum 
        //       with support for LikeOperator and AvoidNormalizedCRLF in addition to supporting
        //       the comparison operator.
        public static Process ExecuteProcess(string expected, string fileName, string args, string directory = null)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, args);
            //processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WorkingDirectory = directory ?? Directory.GetCurrentDirectory();
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            Process process = Process.Start(processStartInfo);
            process.WaitForExit();
            AssertExpectation(expected, process.StandardOutput.ReadToEnd(), (left, right) => LikeOperator(left, right));
            return process;
        }
    }
}
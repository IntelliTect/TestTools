using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace IntelliTect.TestTools.Console
{
    /// <summary>
    /// Provides assertion methods for tests that use Console Output
    /// </summary>
    public static class ConsoleAssert
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
        /// <param name="normalizeLineEndings">Whether differences in line ending styles should be ignored.</param>
        [Obsolete("Use Expect with " + nameof(NormalizeOptions))]
        public static string Expect(string expected, Action action, bool normalizeLineEndings)
        {
            return Expect(expected, 
                action, 
                (left, right) => left == right, 
                normalizeLineEndings ? NormalizeOptions.NormalizeLineEndings : NormalizeOptions.None);
        }
        
        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double 
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        /// <param name="normalizeOptions">Options to normalize input and expected output</param>
        public static string Expect(string expected,
            Action action, 
            NormalizeOptions normalizeOptions = NormalizeOptions.Default)
        {
            return Expect(expected, 
                action, 
                (left, right) => left == right, 
                normalizeOptions);
        }

        /// <summary>
        /// <para>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </para>
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        /// <param name="args">Args to pass to the function.</param>
        /// <param name="normalizeLineEndings">Whether differences in line ending styles should be ignored.</param>
        [Obsolete("Use Expect with " + nameof(NormalizeOptions))]
        public static string Expect(string expected, 
            Action<string[]> action, 
            bool normalizeLineEndings = true, 
            params string[] args)
        {
            return Expect(expected, 
                ()=>action(args), 
                (left, right) => left == right, 
                normalizeLineEndings ? NormalizeOptions.NormalizeLineEndings : NormalizeOptions.None);
        }
        
        /// <summary>
        /// <para>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </para>
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        /// <param name="args">Args to pass to the function.</param>
        /// <param name="normalizeOptions">Options to normalize input and expected output</param>
        public static string Expect(string expected, 
            Action<string[]> action, 
            NormalizeOptions normalizeOptions = NormalizeOptions.Default, 
            params string[] args)
        {
            return Expect(expected, 
                ()=>action(args), 
                (left, right) => left == right, 
                normalizeOptions);
        }

        /// <summary>
        /// <para>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </para>
        /// <para>
        /// In addition to the checking of the console output, the return value of the 
        /// called function will be asserted for equality with <paramref name="expectedReturn"/>
        /// </para>
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="func">Method to be run</param>
        /// <param name="expectedReturn">Value against which equality with the method's return value will be asserted.</param>
        /// <param name="args">Args to pass to the function.</param>
        public static void Expect<T>(string expected, Func<string[], T> func, T expectedReturn = default, params string[] args)
        {
            T @return = default;
            Expect(expected, () => @return = func(args));

            if (!expectedReturn.Equals(@return))
            {
                throw new Exception($"The value returned from {nameof(func)} ({@return}) was not the { nameof(expectedReturn) }({expectedReturn}) value.");
            }
        }

        /// <summary>
        /// <para>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </para>
        /// <para>Newlines will not be normalized, and trailing newlines will not be trimmed.</para>
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        [Obsolete]
        public static string ExpectNoTrimOutput(string expected, Action action)
        {
            return Expect(expected, 
                action, 
                (left, right) => left == right);
        }
        
        /// <summary>
        /// <para>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </para>
        /// <para>
        /// In addition to the checking of the console output, the return value of the 
        /// called function will be asserted for equality with <paramref name="expectedReturn"/>
        /// </para>
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="func">Method to be run</param>
        /// <param name="expectedReturn">Value against which equality with the method's return value will be asserted.</param>
        public static void Expect<T>(string expected, Func<T> func, T expectedReturn)
        {
            Expect(expected, (_) => func(), expectedReturn);
        }

        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="func">Method to be run</param>
        /// <param name="args">Args to pass to the function.</param>
        public static void Expect(string expected, Action<string[]> func, params string[] args) =>
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
        /// <param name="comparisonOperator"></param>
        /// <param name="normalizeOptions">Options to normalize input and expected output</param>
        /// <param name="equivalentOperatorErrorMessage">A textual description of the message if the result of <paramref name="action"/> does not match the <paramref name="expected"/> value</param>
        private static string Expect(
            string expected, Action action, Func<string, string, bool> comparisonOperator,
            NormalizeOptions normalizeOptions = NormalizeOptions.Default, 
            string equivalentOperatorErrorMessage= "Values are not equal")
        {
            (string input, string output) = Parse(expected);

            return Execute(input, output, action, 
                (left, right)=>comparisonOperator(left,right), 
                normalizeOptions, equivalentOperatorErrorMessage);
        }

        private static readonly Func<string, string, bool> LikeOperator =
            (expected, output) => output.IsLike(expected);

        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="escapeCharacter"></param>
        /// <param name="action">Method to be run</param>
		[Obsolete]
        public static string ExpectLike(string expected, char escapeCharacter, Action action)
        {
            return Expect(expected, action, (pattern, output) => output.IsLike(pattern, escapeCharacter));
        }
		
        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        /// <param name="normalizeLineEndings">Whether differences in line ending styles should be ignored.</param>
        /// <param name="escapeCharacter">The escape character for the wildcard caracters.  Default is '\'.</param>
        [Obsolete]
        public static string ExpectLike(string expected, Action action, 
            bool normalizeLineEndings, char escapeCharacter = '\\')
        {
            return Expect(expected, 
                action, 
                (pattern, output) => output.IsLike(pattern, escapeCharacter),
                normalizeLineEndings ? NormalizeOptions.NormalizeLineEndings : NormalizeOptions.None, 
                "The values are not like (using wildcards) each other");
        }
        
        /// <summary>
        /// Performs a unit test on a console-based method. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="action">Method to be run</param>
        /// <param name="normalizeLineEndings">Whether differences in line ending styles should be ignored.</param>
        /// <param name="escapeCharacter">The escape character for the wildcard caracters.  Default is '\'.</param>
        public static string ExpectLike(string expected, 
            Action action, 
            NormalizeOptions normalizeLineEndings = NormalizeOptions.Default, 
            char escapeCharacter = '\\')
        {
            return Expect(expected, 
                action, 
                (pattern, output) => output.IsLike(pattern, escapeCharacter),
                normalizeLineEndings, 
                "The values are not like (using wildcards) each other");
        }

        /// <summary>
        /// Normalizes all line endings of the input string into <see cref="Environment.NewLine" />
        /// </summary>
        /// <param name="input">The input to normalize</param>
        /// <param name="trimTrailingNewline">True if trailing newlines should be trimmed.</param>
        /// <returns>The normalized input.</returns>
        public static string NormalizeLineEndings(string input, bool trimTrailingNewline = false)
        {
            // https://stackoverflow.com/questions/140926/normalize-newlines-in-c-sharp
            input = Regex.Replace(input, @"\r\n|\n\r|\n|\r", Environment.NewLine);

            if (trimTrailingNewline && input.EndsWith(Environment.NewLine))
            {
                input = input.Substring(0, input.Length - Environment.NewLine.Length);
            }

            return input;
        }
        
        /// <summary>
        /// Strips VT100 color characters from the input string
        /// </summary>
        /// <param name="input">The input to strip</param>
        /// <returns>The stripped input.</returns>
        private static string StripAnsiEscapeCodes(string input)
        {
            return Regex.Replace(input, @"\u001b\[\d{1,3}m", "");
        }

        /// <summary>
        /// Executes the unit test while providing console input.
        /// </summary>
        /// <param name="givenInput">Input which will be given</param>
        /// <param name="expectedOutput">The expected output</param>
        /// <param name="action">Action to be tested</param>
        /// <param name="areEquivalentOperator">delegate for comparing the expected from actual output.</param>
        /// <param name="normalizeOptions">Options to normalize input and expected output</param>
        /// <param name="equivalentOperatorErrorMessage">A textual description of the message if the <paramref name="areEquivalentOperator"/> returns false</param>
        private static string Execute(string givenInput,
            string expectedOutput,
            Action action,
            Func<string, string, bool> areEquivalentOperator,
            NormalizeOptions normalizeOptions = NormalizeOptions.Default,
            string equivalentOperatorErrorMessage = "Values are not equal"
        )
        {
            string output = Execute(givenInput, action);

            if ((normalizeOptions & NormalizeOptions.NormalizeLineEndings) != 0)
            {
                output = NormalizeLineEndings(output, true);
                expectedOutput = NormalizeLineEndings(expectedOutput, true);
            }

            if ((normalizeOptions & NormalizeOptions.StripAnsiEscapeCodes) != 0)
            {
                output = StripAnsiEscapeCodes(output);
                expectedOutput = StripAnsiEscapeCodes(expectedOutput);
            }

            AssertExpectation(expectedOutput, output, areEquivalentOperator, equivalentOperatorErrorMessage);
            return output;
        }

        /// <summary>
        /// Asserts whether the values are equivalent according to the <paramref name="areEquivalentOperator"/>"/>
        /// </summary>
        /// <param name="expectedOutput">The expected value of the output.</param>
        /// <param name="output">The actual value output.</param>
        /// <param name="areEquivalentOperator">The operator used to compare equivalency.</param>
        /// <param name="equivalentOperatorErrorMessage">A textual description of the message if the <paramref name="areEquivalentOperator"/> returns false</param>
        private static void AssertExpectation(string expectedOutput, string output, Func<string, string, bool> areEquivalentOperator,
            string equivalentOperatorErrorMessage = null)
        {
            bool failTest = !areEquivalentOperator(expectedOutput, output);
            if (failTest)
            {
                throw new Exception(GetMessageText(expectedOutput, output, equivalentOperatorErrorMessage));
            }
        }

        private static readonly object ExecuteLock = new object();

        /// <summary>
        /// Executes the <paramref name="action"/> while providing console input.
        /// </summary>
        /// <param name="givenInput">Input which will be given at the console when prompted</param>
        /// <param name="action">The action to run.</param>
        public static string Execute(string givenInput, Action action)
        {
            lock (ExecuteLock)
            {
                TextWriter savedOutputStream = System.Console.Out;
                TextReader savedInputStream = System.Console.In;
                try
                {
                        string output;
                        using (TextWriter writer = new StringWriter())
                        using (TextReader reader = new StringReader(string.IsNullOrWhiteSpace(givenInput) ? "" : givenInput))
                        {
                            System.Console.SetOut(writer);

                            System.Console.SetIn(reader);
                            action();

                            output = writer.ToString();
                        }

                        return output;
                    }
                finally
                {
                    System.Console.SetOut(savedOutputStream);
                    System.Console.SetIn(savedInputStream);
                }
            }
        }


        private static string GetMessageText(string expectedOutput, string output, string equivalentOperatorErrorMessage=null)
        {
            string result = "";

            result += string.Join(Environment.NewLine, $"{equivalentOperatorErrorMessage}:- ", 
                "-----------------------------------",
                $"Expected: { CSharpStringEncode( expectedOutput) }",
                $"Actual  : { CSharpStringEncode( output) }", 
                "-----------------------------------");

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
            string result;
            using (var writer = new StringWriter())
            using (var provider = CodeDomProvider.CreateProvider("CSharp")) 
            {
                provider.GenerateCodeFromExpression(new CodePrimitiveExpression(text), writer, 
                    new CodeGeneratorOptions() { BlankLinesBetweenMembers = false });
                result = writer.ToString();

                // Remove extra text added during formatting (..realtext" + "realtext..)
                return Regex.Replace(result, @"""\s+\+\s+""", "");
            }
        }

        private static char CSharpStringEncode(char input) => CSharpStringEncode(input.ToString())[0];


        /// <summary>
        /// This parses a "view" string into two separate strings, one
        /// representing virtual input and the other as expected output
        /// </summary>
        /// <param name="view">
        /// What a user would see in the console, but with input/output tokens.
        /// </param>
        /// <returns>[0] Input, and [1] Output</returns>
        private static (string Input,string Output) Parse(string view)  // TODO: Return Tuple instead.
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
                {
                    input += viewTemp[i].ToString();
                }
                else
                {
                    output += viewTemp[i].ToString();
                }
            }

            return (Input:input, Output:output);
        }

        // TODO: Should not use LikeOperator by default.  Add a ConsoleTestsComparisonOptions enum 
        //       with support for LikeOperator and AvoidNormalizedCRLF in addition to supporting
        //       the comparison operator.
        /// <summary>
        /// Performs a unit test on a console-based executable. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="fileName">Path to the process to be started.</param>
        /// <param name="args">Arguments string to be passed to the process.</param>
        /// <param name="workingDirectory">Working directory to start the process in.</param>
        public static Process ExecuteProcess(string expected, string fileName, string args, string workingDirectory = null)
        {
            return ExecuteProcess(expected, fileName, args, out _, out _, workingDirectory);
        }

        /// <summary>
        /// Performs a unit test on a console-based executable. A "view" of
        /// what a user would see in their console is provided as a string,
        /// where their input (including line-breaks) is surrounded by double
        /// less-than/greater-than signs, like so: "Input please: &lt;&lt;Input&gt;&gt;"
        /// </summary>
        /// <param name="expected">Expected "view" to be seen on the console,
        /// including both input and output</param>
        /// <param name="fileName">Path to the process to be started.</param>
        /// <param name="args">Arguments string to be passed to the process.</param>
        /// <param name="standardOutput">Full contents of stdout that was written by the process.</param>
        /// <param name="standardError">Full contents of stderr that was written by the process.</param>
        /// <param name="workingDirectory">Working directory to start the process in.</param>
        public static Process ExecuteProcess(string expected, string fileName, string args,
            out string standardOutput, out string standardError, string workingDirectory = null)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, args)
            {
                //processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            Process process = Process.Start(processStartInfo);
            process.WaitForExit();
            standardOutput = process.StandardOutput.ReadToEnd();
            standardError = process.StandardError.ReadToEnd();
            AssertExpectation(expected, standardOutput, (left, right) => LikeOperator(left, right), "The values are not like (using wildcards) each other");
            return process;
        }
    }
}
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntelliTect.TestTools.Console;

namespace IntelliTect.TestTools.Console.Tests
{
    [TestClass]
    public class ConsoleAssertTests
    {
        [TestMethod]
        public void ConsoleTester_Sample_InigoMontoya()
        {
            const string view =
@"First name: <<Inigo
>>Last name: <<Montoya
>>Hello, Inigo Montoya.";

            ConsoleAssert.Expect(view,
            () =>
            {
                System.Console.Write("First name: ");
                string fname = System.Console.ReadLine();

                System.Console.Write("Last name: ");
                string lname = System.Console.ReadLine();

                System.Console.Write("Hello, {0} {1}.", fname, lname);
            });
        }

        [TestMethod]
        public void ConsoleTester_HelloWorld_NoInput()
        {
            const string view = "Hello World";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.Write("Hello World");
            });
        }

        [TestMethod]
        public void ConsoleTester_HelloWorld_TrimLF()
        {
            const string view = "Hello World\n";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.WriteLine("Hello World");
            }, true);
        }

        [TestMethod]
        public void GivenStringLiteral_ExpectedOutputNormalized_OutputMatches()
        {
            const string view = @"Begin
Middle
End";
            ConsoleAssert.Expect(view, () =>
            {
                System.Console.WriteLine("Begin");
                System.Console.WriteLine("Middle");
                System.Console.WriteLine("End");
            }, true);
        }

        [TestMethod]
        public void ConsoleTester_HelloWorld_TrimCRLF()
        {
            const string view = "Hello World";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.Write("Hello World");
            }, true);
        }

        [TestMethod]
        public void ConsoleTester_HelloWorld_MissingNewline()
        {
            const string view = @"Hello World
";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.WriteLine("Hello World");
            });
        }

        [TestMethod]
        public void ExecuteProcess_PingLocalhost_Success()
        {

            string expected = $@"*
Pinging * ?::1? with 32 bytes of data:
Reply from ::1: time*";

            string pingArgs = "-c 4 localhost";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pingArgs = pingArgs.Replace("-c ", "-n ");
            }

            ConsoleAssert.ExecuteProcess(
                expected,
                "ping.exe", pingArgs, out string standardOutput, out _);
            Assert.IsTrue(standardOutput.ToLower().IsLike($"*{ Environment.MachineName.ToLower()}*"));
        }

        [TestMethod]
        public void ExecuteLike_GivenVariableCRLFWithNLComparedToCRNL_Success()
        {
            const string expected = "(abstract, 1)\n(abstract, 2)\n\n";
            const string output = "(abstract, 1)\r\n(abstract, 2)\r\n";

            IntelliTect.TestTools.Console.ConsoleAssert.ExpectLike(expected, () =>
            {
                System.Console.WriteLine(output);
            });
        }
    }
}
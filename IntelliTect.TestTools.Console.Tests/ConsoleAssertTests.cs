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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Platforms other than windows have not been tested.");

                /* Try the following:
                 * Command: ping.exe -c 4 localhost                 (the '-c 4' limits the ping to 4 times)
                 * PING localhost (104.24.0.68) 56(84) bytes of data.
                 *  64 bytes from 104.24.0.68: icmp_seq=1 ttl=58 time=47.3 ms
                 *  64 bytes from 104.24.0.68: icmp_seq=2 ttl=58 time=51.9 ms
                 *  64 bytes from 104.24.0.68: icmp_seq=3 ttl=58 time=57.4 ms
                 *  64 bytes from 104.24.0.68: icmp_seq=4 ttl=58 time=57.4 ms
                 */

            }
            else
            {
                ConsoleAssert.ExecuteProcess(
$@"
Pinging * ?::1? with 32 bytes of data:
Reply from ::1: time*",
                "ping.exe", "-n 4 localhost", out string standardOutput, out _);
                Assert.IsTrue(standardOutput.ToLower().IsLike($"*{ Environment.MachineName.ToLower()}*"));
            }
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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelliTect.TestTools.Console.Tests
{
    [TestClass]
    public class ConsoleAssertTests
    {
        [TestMethod]
        public void ConsoleTester_Sample_InigoMontoya()
        {
            string view =
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
            string view = @"Hello World";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.Write("Hello World");
            });
        }

        [TestMethod]
        public void ConsoleTester_HelloWorld_MissingNewline()
        {
            string view = @"Hello World";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.WriteLine("Hello World");
            });
        }


        [TestMethod]
        public void ExecuteProcess_PingLocalhost_Success()
        {
            ConsoleAssert.ExecuteProcess(
$@"
Pinging { Environment.MachineName } ?::1? with 32 bytes of data:
Reply from ::1: time*", 
                "ping.exe", "localhost");
        }
    }
}
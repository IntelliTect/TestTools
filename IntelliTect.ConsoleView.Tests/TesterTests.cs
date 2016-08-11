using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intellitect.ConsoleView.Tests
{
    [TestClass]
    public class TesterTests
    {
        [TestMethod]
        public void ConsoleView_Sample_InigoMontoya()
        {
            string view =
@"First name: <<Inigo
>>Last name: <<Montoya
>>Hello, Inigo Montoya.";

            Tester.Test(view,
            () =>
            {
                Console.Write("First name: ");
                string fname = Console.ReadLine();

                Console.Write("Last name: ");
                string lname = Console.ReadLine();

                Console.Write("Hello, {0} {1}.", fname, lname);
            });
        }

        [TestMethod]
        public void ConsoleView_HelloWorld_NoInput()
        {
            string view = @"Hello World";

            Tester.Test(view, () =>
            {
                Console.Write("Hello World");
            });
        }

        [TestMethod]
        public void ConsoleView_HelloWorld_MissingNewline()
        {
            string view = @"Hello World";

            Tester.Test(view, () =>
            {
                Console.WriteLine("Hello World");
            });
        }


        [TestMethod]
        public void ExecuteProcess_PingLocalhost_Success()
        {
            Tester.ExecuteProcess(
$@"^
Pinging { Environment.MachineName } \[::1\] with 32 bytes of data:
Reply from ::1: time", 
                "ping.exe", "localhost");
        }
    }
}
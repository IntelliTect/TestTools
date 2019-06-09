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
        public void ConsoleTester_HelloWorld_TrimLF()
        {
            string view = "Hello World\n";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.WriteLine("Hello World");
            }, true);
        }

        [TestMethod]
        public void GivenStringLiteral_ExpectedOutputNormalized_OutputMatches()
        {
            string view = @"Begin
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
            string view = @"Hello World";

            ConsoleAssert.Expect(view, () =>
            {
                System.Console.Write("Hello World");
            }, true);
        }

        [TestMethod]
        public void ConsoleTester_HelloWorld_MissingNewline()
        {
            string view = @"Hello World
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
                string standardOutput, standardInput;
                System.Diagnostics.Process process = ConsoleAssert.ExecuteProcess(
$@"
Pinging * ?::1? with 32 bytes of data:
Reply from ::1: time*",
                "ping.exe", "-n 4 localhost", out standardOutput, out standardInput);
                Assert.IsTrue(standardOutput.ToLower().IsLike($"*{ Environment.MachineName.ToLower()}*"));
            }
        }

        [TestMethod]
        public void ExecuteLike_GivenVariableCRLFWithNLComparedToCRNL_Success()
        {
            string expected = "(abstract, 1)\n(abstract, 2)\n(abstract, 3)\n(add\\*, 1)\n(add\\*, 2)\n(add\\*, 3)\n(alias\\*, 1)\n*\n(where\\*, 3)\n(while, 1)\n(while, 2)\n(while, 3)\n(yield\\*, 1)\n(yield\\*, 2)\n(yield\\*, 3)";
            string output = "(abstract, 1)\n(abstract, 2)\r\n(abstract, 3)\r\n(add*, 1)\r\n(add*, 2)\r\n(add*, 3)\r\n(alias*, 1)\r\n(alias*, 2)\r\n(alias*, 3)\r\n(as, 1)\r\n(as, 2)\r\n(as, 3)\r\n(ascending*, 1)\r\n(ascending*, 2)\r\n(ascending*, 3)\r\n(async*, 1)\r\n(async*, 2)\r\n(async*, 3)\r\n(await*, 1)\r\n(await*, 2)\r\n(await*, 3)\r\n(base, 1)\r\n(base, 2)\r\n(base, 3)\r\n(bool, 1)\r\n(bool, 2)\r\n(bool, 3)\r\n(break, 1)\r\n(break, 2)\r\n(break, 3)\r\n(by*, 1)\r\n(by*, 2)\r\n(by*, 3)\r\n(byte, 1)\r\n(byte, 2)\r\n(byte, 3)\r\n(case, 1)\r\n(case, 2)\r\n(case, 3)\r\n(catch, 1)\r\n(catch, 2)\r\n(catch, 3)\r\n(char, 1)\r\n(char, 2)\r\n(char, 3)\r\n(checked, 1)\r\n(checked, 2)\r\n(checked, 3)\r\n(class, 1)\r\n(class, 2)\r\n(class, 3)\r\n(const, 1)\r\n(const, 2)\r\n(const, 3)\r\n(continue, 1)\r\n(continue, 2)\r\n(continue, 3)\r\n(decimal, 1)\r\n(decimal, 2)\r\n(decimal, 3)\r\n(default, 1)\r\n(default, 2)\r\n(default, 3)\r\n(delegate, 1)\r\n(delegate, 2)\r\n(delegate, 3)\r\n(descending*, 1)\r\n(descending*, 2)\r\n(descending*, 3)\r\n(do, 1)\r\n(do, 2)\r\n(do, 3)\r\n(double, 1)\r\n(double, 2)\r\n(double, 3)\r\n(dynamic *, 1)\r\n(dynamic *, 2)\r\n(dynamic *, 3)\r\n(else, 1)\r\n(else, 2)\r\n(else, 3)\r\n(enum, 1)\r\n(enum, 2)\r\n(enum, 3)\r\n(event, 1)\r\n(event, 2)\r\n(event, 3)\r\n(equals*, 1)\r\n(equals*, 2)\r\n(equals*, 3)\r\n(explicit, 1)\r\n(explicit, 2)\r\n(explicit, 3)\r\n(extern, 1)\r\n(extern, 2)\r\n(extern, 3)\r\n(false, 1)\r\n(false, 2)\r\n(false, 3)\r\n(finally, 1)\r\n(finally, 2)\r\n(finally, 3)\r\n(fixed, 1)\r\n(fixed, 2)\r\n(fixed, 3)\r\n(from *, 1)\r\n(from *, 2)\r\n(from *, 3)\r\n(float, 1)\r\n(float, 2)\r\n(float, 3)\r\n(for, 1)\r\n(for, 2)\r\n(for, 3)\r\n(foreach, 1)\r\n(foreach, 2)\r\n(foreach, 3)\r\n(get *, 1)\r\n(get *, 2)\r\n(get *, 3)\r\n(global *, 1)\r\n(global *, 2)\r\n(global *, 3)\r\n(group *, 1)\r\n(group *, 2)\r\n(group *, 3)\r\n(goto, 1)\r\n(goto, 2)\r\n(goto, 3)\r\n(if, 1)\r\n(if, 2)\r\n(if, 3)\r\n(implicit, 1)\r\n(implicit, 2)\r\n(implicit, 3)\r\n(in, 1)\r\n(in, 2)\r\n(in, 3)\r\n(int, 1)\r\n(int, 2)\r\n(int, 3)\r\n(into*, 1)\r\n(into*, 2)\r\n(into*, 3)\r\n(interface, 1)\r\n(interface, 2)\r\n(interface, 3)\r\n(internal, 1)\r\n(internal, 2)\r\n(internal, 3)\r\n(is, 1)\r\n(is, 2)\r\n(is, 3)\r\n(lock, 1)\r\n(lock, 2)\r\n(lock, 3)\r\n(long, 1)\r\n(long, 2)\r\n(long, 3)\r\n(join*, 1)\r\n(join*, 2)\r\n(join*, 3)\r\n(let*, 1)\r\n(let*, 2)\r\n(let*, 3)\r\n(nameof*, 1)\r\n(nameof*, 2)\r\n(nameof*, 3)\r\n(namespace, 1)\r\n(namespace, 2)\r\n(namespace, 3)\r\n(new, 1)\r\n(new, 2)\r\n(new, 3)\r\n(null, 1)\r\n(null, 2)\r\n(null, 3)\r\n(on*, 1)\r\n(on*, 2)\r\n(on*, 3)\r\n(operator, 1)\r\n(operator, 2)\r\n(operator, 3)\r\n(orderby*, 1)\r\n(orderby*, 2)\r\n(orderby*, 3)\r\n(out, 1)\r\n(out, 2)\r\n(out, 3)\r\n(override, 1)\r\n(override, 2)\r\n(override, 3)\r\n(object, 1)\r\n(object, 2)\r\n(object, 3)\r\n(params, 1)\r\n(params, 2)\r\n(params, 3)\r\n(partial*, 1)\r\n(partial*, 2)\r\n(partial*, 3)\r\n(private, 1)\r\n(private, 2)\r\n(private, 3)\r\n(protected, 1)\r\n(protected, 2)\r\n(protected, 3)\r\n(public, 1)\r\n(public, 2)\r\n(public, 3)\r\n(readonly, 1)\r\n(readonly, 2)\r\n(readonly, 3)\r\n(ref, 1)\r\n(ref, 2)\r\n(ref, 3)\r\n(remove*, 1)\r\n(remove*, 2)\r\n(remove*, 3)\r\n(return, 1)\r\n(return, 2)\r\n(return, 3)\r\n(sbyte, 1)\r\n(sbyte, 2)\r\n(sbyte, 3)\r\n(sealed, 1)\r\n(sealed, 2)\r\n(sealed, 3)\r\n(select*, 1)\r\n(select*, 2)\r\n(select*, 3)\r\n(set*, 1)\r\n(set*, 2)\r\n(set*, 3)\r\n(short, 1)\r\n(short, 2)\r\n(short, 3)\r\n(sizeof, 1)\r\n(sizeof, 2)\r\n(sizeof, 3)\r\n(stackalloc, 1)\r\n(stackalloc, 2)\r\n(stackalloc, 3)\r\n(static, 1)\r\n(static, 2)\r\n(static, 3)\r\n(string, 1)\r\n(string, 2)\r\n(string, 3)\r\n(struct, 1)\r\n(struct, 2)\r\n(struct, 3)\r\n(switch, 1)\r\n(switch, 2)\r\n(switch, 3)\r\n(this, 1)\r\n(this, 2)\r\n(this, 3)\r\n(throw, 1)\r\n(throw, 2)\r\n(throw, 3)\r\n(true, 1)\r\n(true, 2)\r\n(true, 3)\r\n(try, 1)\r\n(try, 2)\r\n(try, 3)\r\n(typeof, 1)\r\n(typeof, 2)\r\n(typeof, 3)\r\n(uint, 1)\r\n(uint, 2)\r\n(uint, 3)\r\n(ulong, 1)\r\n(ulong, 2)\r\n(ulong, 3)\r\n(unsafe, 1)\r\n(unsafe, 2)\r\n(unsafe, 3)\r\n(ushort, 1)\r\n(ushort, 2)\r\n(ushort, 3)\r\n(using, 1)\r\n(using, 2)\r\n(using, 3)\r\n(value*, 1)\r\n(value*, 2)\r\n(value*, 3)\r\n(var*, 1)\r\n(var*, 2)\r\n(var*, 3)\r\n(virtual, 1)\r\n(virtual, 2)\r\n(virtual, 3)\r\n(unchecked, 1)\r\n(unchecked, 2)\r\n(unchecked, 3)\r\n(void, 1)\r\n(void, 2)\r\n(void, 3)\r\n(volatile, 1)\r\n(volatile, 2)\r\n(volatile, 3)\r\n(where*, 1)\r\n(where*, 2)\r\n(where*, 3)\r\n(while, 1)\r\n(while, 2)\r\n(while, 3)\r\n(yield*, 1)\r\n(yield*, 2)\r\n(yield*, 3)";

            IntelliTect.TestTools.Console.ConsoleAssert.ExpectLike(expected, '\\',
            () =>
            {
                System.Console.WriteLine(output);
            });
        }
    }
}
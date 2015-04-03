using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncronousConsole;
using AsyncronousConsole.Support;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConsole test = ConsoleAsync.CreateConsole("TestConsole");
            
            ConsoleAsync.SendCommand("TestConsole", "print one two three");
            test.SendCommand("print one two three");

            test.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());

            ConsoleAsync.Run();
        }
    }
}

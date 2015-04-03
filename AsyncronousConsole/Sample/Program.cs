using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncronousConsole;
using AsyncronousConsole.BuiltIn;
using AsyncronousConsole.Support;

namespace Sample
{
    class Program
    {

        static void Main(string[] args)
        {

            IConsole console = ConsoleAsync.CreateConsole("TestConsole");

            console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
            console.AddCommand("start", (writer, strings) => console.WriteStandardOutput());
            console.AddCommand("stop", (writer, strings) => console.DiscardStandardOutput());

            console.AddWorker(new TimedWorker(TimeSpan.FromMilliseconds(500), (writer, span) => Console.WriteLine("{0}", span)));

            ConsoleAsync.Run(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncronousConsole;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConsole console = ConsoleAsync.CreateConsole("First Console");

            console.Execute(writer => writer.NewLine().Info(writer.ConsoleName).NewLine());

            ConsoleAsync.Run();
        }
    }
}

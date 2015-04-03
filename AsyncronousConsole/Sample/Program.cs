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
            IConsole console1 = ConsoleAsync.CreateConsole("First Console");
            IConsole console2 = ConsoleAsync.CreateConsole("Second Console");
            IConsole console3 = ConsoleAsync.CreateConsole("Third Console");
            IConsole console4 = ConsoleAsync.CreateConsole("Fourth Console");

            ConsoleAsync.ExecuteCommandToAllConsole(writer =>
            {
                for (int i = 0; i < 50; i++)
                {
                    writer.Text("{0} {1:0000}", writer.ConsoleName, i).NewLine();
                }
            });


            ConsoleAsync.Run();
        }
    }
}

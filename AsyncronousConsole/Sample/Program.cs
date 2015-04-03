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

            console.AddKeyFilter((writer, info) =>
            {
                if ("0123456789".Contains(info.KeyChar))
                {
                    writer.Warning("Pressed number {0}", info.KeyChar).NewLine();
                    return true;
                }
                return false;
            });

            console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());

            console.AddCommand("first", (writer, strings) =>
            {
                if (strings.Length > 0)
                    writer.Text(strings[0]).NewLine();
            });

            

            ConsoleAsync.Run();
        }
    }
}

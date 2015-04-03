using AsyncronousConsole;

namespace BasicSample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConsole console = ConsoleAsync.CreateConsole("Console");

            console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
            console.AddCommand("print", (writer, strings) => strings.ForEach(s => writer.Text(s).NewLine()));

            console.Execute(writer =>
            {
                writer.Info("ConsoleAsync").NewLine().NewLine();
                writer.Text(@"
Available commands:

quit   : close entirely the app
print  : print in console all the arguments
").NewLine();
            });

            ConsoleAsync.Run();
        }

    }
}

using AsyncronousConsole;

namespace BasicSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // This application demostrate the basics of ConsoleAsync

            // Create forst console
            IConsole console = ConsoleAsync.CreateConsole("Console");

            // Add commands
            console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
            console.AddCommand("print", (writer, strings) => strings.ForEach(s => writer.Text(s).NewLine()));

            // Execute operation on console
            console.Execute(writer =>
            {
                writer.Info("ConsoleAsync").NewLine().NewLine();
                writer.Text(@"
Available commands:

quit   : close entirely the app
print  : print in console all the arguments
").NewLine();
            });

            // Wait commands from user
            ConsoleAsync.Run();
        }

    }
}

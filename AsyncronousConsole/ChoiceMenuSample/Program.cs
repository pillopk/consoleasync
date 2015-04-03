using System.Globalization;
using System.Linq;
using AsyncronousConsole;

namespace ChoiceMenuSample
{
    class Program
    {
        private const string systemConsoleName = "Menu Console";

        private static int consoleIndex;

        static void Main(string[] args)
        {
            IConsole menuConsole = ConsoleAsync.CreateConsole(systemConsoleName);

            menuConsole.AddKeyFilter((writer, info) =>
            {
                string ch = info.KeyChar.ToString(CultureInfo.InvariantCulture);

                if (EvaluateCommand(writer, ch))
                    WriteMenu(writer);

                return ConsoleAsync.AvailableInputChars.Contains(ch);
            });

            menuConsole.Execute(WriteMenu);

            ConsoleAsync.Run();
        }

        static void WriteMenu(IConsoleWriter writer)
        {
            writer.Clear();
            writer.Info("Use number to select an option").NewLine().NewLine();
            writer.Text("01 - Create Console").NewLine();
            writer.Text("02 - Reset").NewLine();
            writer.Text("03 - Quit").NewLine();
            writer.NewLine();

            int index = 4;
            foreach (IConsole console in ConsoleAsync.EnumerateConsoles())
            {
                if (console.Name != systemConsoleName)
                    writer.Text("{0:00} - Delete console '{1}'", index++, console.Name).NewLine();
            }

            writer.NewLine().NewLine();
        }

        static bool EvaluateCommand(IConsoleWriter writer, string command)
        {
            int index;
            IConsole[] consoles = ConsoleAsync.EnumerateConsoles().ToArray();

            if (int.TryParse(command, out index) == false) return false;

            if (index == 1)
            {
                if (consoles.Length == 6)
                {
                    writer.Error("Maximum console reached (for this app :)").NewLine();
                    return false;
                }

                CreateNewConsole();
            }
            else if (index == 2)
            {
                foreach (IConsole console in ConsoleAsync.EnumerateConsoles())
                {
                    if (console.Name != systemConsoleName)
                        ConsoleAsync.DestroyConsole(console.Name);
                }
                consoleIndex = 0;
            }
            else if (index == 3)
            {
                ConsoleAsync.Quit();
            }
            else
            {
                ConsoleAsync.DestroyConsole(consoles[index - 3].Name);
            }

            return true;
        }

        static void CreateNewConsole()
        {
            string name = string.Format("Console #{0:00}", consoleIndex + 1);
            IConsole console = ConsoleAsync.CreateConsole(name);

            console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
            console.AddCommand("print", (writer, strings) => strings.ForEach(s => writer.Text(s).NewLine()));

            console.Execute(writer =>
            {
                writer.Info(name).NewLine().NewLine();
                writer.Text(@"
Available commands:

quit   : close entirely the app
print  : print in console all the arguments
");
            });

            consoleIndex++;
        }
    }
}

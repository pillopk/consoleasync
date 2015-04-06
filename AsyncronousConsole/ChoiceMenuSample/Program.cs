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
            // Create first console
            IConsole menuConsole = ConsoleAsync.CreateConsole(systemConsoleName);

            // Create a key filter for every key pressed in app
            menuConsole.AddKeyFilter((writer, info) =>
            {
                string ch = info.KeyChar.ToString(CultureInfo.InvariantCulture);

                // check if the cher is a valid commend, then refresh menu
                if (EvaluateCommand(writer, ch))
                    WriteMenu(writer);

                // return true only if char NOT in available chars
                // input row remains empty
                return ConsoleAsync.AvailableInputChars.Contains(ch);
            });

            menuConsole.Execute(WriteMenu);

            ConsoleAsync.Run();
        }

        static void WriteMenu(IConsoleWriter writer)
        {
            // Clear console and write the static part of menu
            writer.Clear();
            writer.Info("Use number to select an option").NewLine().NewLine();
            writer.Text("01 - Create Console").NewLine();
            writer.Text("02 - Reset").NewLine();
            writer.Text("03 - Quit").NewLine();
            writer.NewLine();

            // Cicle through console and write a menu item for each
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

            // Check if char is a number, else return false
            if (int.TryParse(command, out index) == false) return false;

            if (index == 1)
            {
                // Add console maximum five
                if (consoles.Length == 6)
                {
                    writer.Error("Maximum console reached (for this app :)").NewLine();
                    return false;
                }

                CreateNewConsole();
            }
            else if (index == 2)
            {
                // Destroy all console, except menu
                foreach (IConsole console in ConsoleAsync.EnumerateConsoles())
                {
                    if (console.Name != systemConsoleName)
                        ConsoleAsync.DestroyConsole(console.Name);
                }
                consoleIndex = 0;
            }
            else if (index == 3)
            {
                // Exit from app
                ConsoleAsync.Quit();
            }
            else
            {
                // Destroy console specified in menu item
                ConsoleAsync.DestroyConsole(consoles[index - 3].Name);
            }

            return true;
        }

        static void CreateNewConsole()
        {
            // Create console name
            string name = string.Format("Console #{0:00}", consoleIndex + 1);

            // Create console
            IConsole console = ConsoleAsync.CreateConsole(name);

            // Add default command
            console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
            console.AddCommand("print", (writer, strings) => strings.ForEach(s => writer.Text(s).NewLine()));

            // Call execute to write a message
            console.Execute(writer =>
            {
                writer.Info(name).NewLine().NewLine();
                writer.Text(@"
Available commands:

quit   : close entirely the app
print  : print in console all the arguments
");
            });

            // Increment console count
            consoleIndex++;
        }
    }
}

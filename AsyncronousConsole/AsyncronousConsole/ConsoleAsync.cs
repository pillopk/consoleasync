using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AsyncronousConsole.Engine;

namespace AsyncronousConsole
{
    public static class ConsoleAsync
    {
        internal const string SYSTEM_CONSOLE_TITLE = "Asyncronous Console v1.0";

        private static string availableChars = @"abcdefghijklmnopqrstuvwxyz0123456789 ,.;:_@#+-*/!£$%&()[]{}?^/|\'<>";
        private static ConsoleManager managerInstance;
        private static int consoleWidth = 100;
        private static int consoleHeight = 36;

        public static int ConsoleWidth
        {
            get { return consoleWidth; }
            private set { consoleWidth = value; }
        }

        public static int ConsoleHeight
        {
            get { return consoleHeight; }
            private set { consoleHeight = value; }
        }

        public static string AvailableInputChars
        {
            get { return availableChars; }
            set { availableChars = value; }
        }

        internal static ConsoleManager Manager
        {
            get
            {
                if (managerInstance == null)
                {
                    Console.Title = SYSTEM_CONSOLE_TITLE;
                    Console.Clear();

                    managerInstance = new ConsoleManager();
                }
                return managerInstance;
            }
        }

        public static void SetConsoleSize(int width, int height)
        {
            if (width < 60)
                throw new ArgumentException("Width must be greater than 59");

            if (width < 30)
                throw new ArgumentException("Height must be greater than 29");

            if (Manager == null)
            {
                ConsoleWidth = width;
                ConsoleHeight = height;
            }
            else
            {
                Console.WindowWidth = width;
                Console.WindowHeight = height;
                Manager.Renderer.ForceRender();
            }
        }

        /// <summary>
        /// Return the actual active console
        /// </summary>
        public static IConsole ActiveConsole
        {
            get { return Manager.ActiveConsole; }
        }

        /// <summary>
        /// Create new console
        /// </summary>
        /// <param name="consoleName">name of new console</param>
        /// <returns>Interface to manage console</returns>
        public static IConsole CreateConsole(string consoleName)
        {
            return Manager.CreateConsole(consoleName);
        }

        /// <summary>
        /// Destroy console
        /// </summary>
        /// <param name="consoleName">Name of console to destroy</param>
        public static void DestroyConsole(string consoleName)
        {
            Manager.DestroyConsole(consoleName);
        }

        /// <summary>
        /// IEnumerable of the existing consoles
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IConsole> EnumerateConsoles()
        {
            return Manager.Consoles;
        }

        /// <summary>
        /// Show the specified console
        /// </summary>
        /// <param name="consoleName">Name of console to destroy</param>
        public static void ShowConsole(string consoleName)
        {
            Manager.ShowConsole(consoleName);
            Manager.Renderer.ForceRender();
        }

        /// <summary>
        /// Add command to all existing consoles
        /// </summary>
        /// <param name="commandText">Text of the command</param>
        /// <param name="action">Action to be executed</param>
        public static void AddCommandToAllConsole(string commandText, Action<IConsoleWriter, List<string>> action)
        {
            Manager.AddCommandToAll(commandText, action);
        }

        /// <summary>
        /// Remove specified command from all console
        /// </summary>
        /// <param name="commandText">Text of the command</param>
        public static void RemoveCommandFromAllConsole(string commandText)
        {
            Manager.RemoveCommandFromAll(commandText);
        }

        /// <summary>
        /// Execute an action in all consoles
        /// </summary>
        /// <param name="action">Action to be executed</param>
        public static void ExecuteCommandToAllConsole(Action<IConsoleWriter> action)
        {
            Manager.ExecuteCommandToAll(action);
        }

        public static bool SendCommand(string consoleName, string commandText)
        {
            ConsoleInstance console = Manager.GetConsole(consoleName, false);
            if (console == null) return false;
            return Manager.ExecuteCommand(console, commandText);
        }

        /// <summary>
        /// Execute main cicle of ConsoleAsync object
        /// </summary>
        public static void Run()
        {
            Manager.Run();
        }

        /// <summary>
        /// Execute main cicle of ConsoleAsync object
        /// </summary>
        /// <param name="startAllWorker">If true all worker in this console will started</param>
        public static void Run(bool startAllWorker)
        {
            Manager.Run(startAllWorker);
        }

        /// <summary>
        /// Quit all the consoles and relative eworkers
        /// </summary>
        public static void Quit()
        {
            Manager.Quit();
        }

        /// <summary>
        /// Execute specified action when a command is issued
        /// </summary>
        /// <param name="action">Action to be executed</param>
        public static void CommandsReceived(Action<string, bool> action)
        {
            Manager.SetReceivedAction(action);
        }

    }
}

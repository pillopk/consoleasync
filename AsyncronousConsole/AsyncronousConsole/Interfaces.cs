using System;
using System.Collections.Generic;
using AsyncronousConsole.Engine;
using AsyncronousConsole.Support;

namespace AsyncronousConsole
{
    public interface IConsole
    {
        /// <summary>
        /// Name of the console instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Return the current writer of the console
        /// </summary>
        /// <returns>Console writer interface</returns>
        IConsoleWriter GetWriter();

        /// <summary>
        /// Add command to console
        /// </summary>
        /// <param name="commandText">Text of the command</param>
        /// <param name="action">Action to be executed</param>
        void AddCommand(string commandText, Action<IConsoleWriter, List<string>> action);

        /// <summary>
        /// Remove command from console
        /// </summary>
        /// <param name="commandText">Command text</param>
        void RemoveCommand(string commandText);

        /// <summary>
        /// Add command to a function key
        /// </summary>
        /// <param name="key">Function key enumeration</param>
        /// <param name="command">Command to be linked</param>
        void AddKeyCommand(KeyCommandEnum key, string command);

        /// <summary>
        /// Add command to a function key
        /// </summary>
        /// <param name="key">Function key enumeration</param>
        /// <param name="command">Command to be linked</param>
        /// <param name="autoSend">If true the command is immediately executed, if false the command will only writed in input bar</param>
        void AddKeyCommand(KeyCommandEnum key, string command, bool autoSend);

        /// <summary>
        /// Remove command from a function key
        /// </summary>
        /// <param name="key">Function key enumeration</param>
        void ClearKeyCommand(KeyCommandEnum key);
        /// <summary>
        /// Execute a command line
        /// </summary>
        /// <param name="commandText">Text of the command</param>
        /// <returns></returns>
        bool SendCommand(string commandText);

        /// <summary>
        /// Immediately execute action in console
        /// </summary>
        /// <param name="action">Action to be executed</param>
        void Execute(Action<IConsoleWriter> action);

        /// <summary>
        /// Add a worker to a console worker cicle
        /// </summary>
        /// <param name="newWorker">Worker object</param>
        /// <returns>Interface to control the worker</returns>
        IConsoleWorker AddWorker(ConsoleWorker newWorker);

        /// <summary>
        /// Stop worker and remove from console
        /// </summary>
        /// <param name="existingWorker">Control interface of worker to remove</param>
        void RemoveWorker(IConsoleWorker existingWorker);

        /// <summary>
        /// Make visible the console on viewport
        /// </summary>
        void Show();

        /// <summary>
        /// Destroy the current console instance
        /// </summary>
        void Destroy();

        /// <summary>
        /// Add a function for key filtering
        /// </summary>
        /// <param name="filter">Function to key filtering, if return true the key will be ignored from input</param>
        void AddKeyFilter(Func<IConsoleWriter, ConsoleKeyInfo, bool> filter);

        /// <summary>
        /// Save the console output to file
        /// </summary>
        /// <param name="directory">Directory path where save the file sequence</param>
        /// <param name="name">Base name of the file sequence</param>
        void SaveOutputToFile(string directory, string name);

        /// <summary>
        /// Save the console output to file
        /// </summary>
        /// <param name="directory">Directory path where save the file sequence</param>
        /// <param name="name">Base name of the file sequence</param>
        /// <param name="linesPerFile">Number of line per text file</param>
        /// <param name="linesPerFlush">Number of line to force flush</param>
        void SaveOutputToFile(string directory, string name, int linesPerFile, int linesPerFlush);

        /// <summary>
        /// Cancel the saving cicle from console
        /// </summary>
        void CancelSaveOutputToFile();

        /// <summary>
        /// Redirect the console standard output to this console
        /// </summary>
        void WriteStandardOutput();

        /// <summary>
        /// Ignore the console standard output
        /// </summary>
        void DiscardStandardOutput();
    }

    public interface IConsoleWriter
    {
        /// <summary>
        /// Return the name of the owner console
        /// </summary>
        string ConsoleName { get; }

        /// <summary>
        /// Return the actual row count of this console
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Scroll vievport to bottom line
        /// </summary>
        void ScrollBottom();

        /// <summary>
        /// Scroll vievport to top line
        /// </summary>
        void ScrollTop();

        /// <summary>
        /// Write text with info style
        /// </summary>
        /// <param name="text">Text to be writed</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Info(string text);

        /// <summary>
        /// Write text with info style
        /// </summary>
        /// <param name="format">A composite format string (see String.Format)</param>
        /// <param name="parameters">The objects to format</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Info(string format, params object[] parameters);

        /// <summary>
        /// Write text with success style
        /// </summary>
        /// <param name="text">Text to be writed</param>
        /// <returns></returns>
        IConsoleWriter Success(string text);

        /// <summary>
        /// Write text with success style
        /// </summary>
        /// <param name="format">A composite format string (see String.Format)</param>
        /// <param name="parameters">The objects to format</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Success(string format, params object[] parameters);

        /// <summary>
        /// Write text with warning style
        /// </summary>
        /// <param name="text">Text to be writed</param>
        /// <returns></returns>
        IConsoleWriter Warning(string text);

        /// <summary>
        /// Write text with warning style
        /// </summary>
        /// <param name="format">A composite format string (see String.Format)</param>
        /// <param name="parameters">The objects to format</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Warning(string format, params object[] parameters);

        /// <summary>
        /// Write text with error style
        /// </summary>
        /// <param name="text">Text to be writed</param>
        /// <returns></returns>
        IConsoleWriter Error(string text);

        /// <summary>
        /// Write text with error style
        /// </summary>
        /// <param name="format">A composite format string (see String.Format)</param>
        /// <param name="parameters">The objects to format</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Error(string format, params object[] parameters);

        /// <summary>
        /// Write text with muted style
        /// </summary>
        /// <param name="text">Text to be writed</param>
        /// <returns></returns>
        IConsoleWriter Muted(string text);

        /// <summary>
        /// Write text with muted style
        /// </summary>
        /// <param name="format">A composite format string (see String.Format)</param>
        /// <param name="parameters">The objects to format</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Muted(string format, params object[] parameters);

        /// <summary>
        /// Write text with default style
        /// </summary>
        /// <param name="text">Text to be writed</param>
        /// <returns></returns>
        IConsoleWriter Text(string text);

        /// <summary>
        /// Write text with default style
        /// </summary>
        /// <param name="format">A composite format string (see String.Format)</param>
        /// <param name="parameters">The objects to format</param>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Text(string format, params object[] parameters);

        /// <summary>
        /// Clear current line and move cursor to the first character
        /// </summary>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter ClearCurrentLine();

        /// <summary>
        /// Clear entirely the console
        /// </summary>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter Clear();

        /// <summary>
        /// Move cursor to the next line
        /// </summary>
        /// <returns>Return console writer interface to permit fluid syntax</returns>
        IConsoleWriter NewLine();
    }

    public interface IConsoleWorker
    {

        /// <summary>
        /// Unique identifier of the worker
        /// </summary>
        Guid UniqueID { get; }

        /// <summary>
        /// State of the worker
        /// </summary>
        WorkerStateEnum State { get; }

        /// <summary>
        /// Start current worker
        /// </summary>
        void Start();

        /// <summary>
        /// Suspend worker execution
        /// </summary>
        void Suspend();

        /// <summary>
        /// Force execution of worker
        /// </summary>
        void Execute();

        /// <summary>
        /// Resume worker execution
        /// </summary>
        void Resume();

        /// <summary>
        /// Stop current worker
        /// </summary>
        void Stop();
    }

}

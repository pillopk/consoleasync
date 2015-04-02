ConsoleAsync
------------
ConsoleAsync is a static class needed for create/destroy console, for worker management cicle 
and helper method to make operation to all existing consoles

##Properties

* `string AvailableInputChars { get; set; }` This property contain a string with all allowed char for input, all char not in this string will be ignored
* `IConsole ActiveConsole { get; }` This property return the actual visible console

##Methods

* `[IConsole](#IConsole) CreateConsole(string consoleName)` Create a new console with specified name and return the control object
* `void DestroyConsole(string consoleName)` Destroy console with specified name if the console is the last one this method throws an exception
* `void ShowConsole(string consoleName)` Make console with specified name visible
* `IEnumerable<[IConsole](#IConsole)> EnumerateConsoles()` Return anumeration of consoles
* `void AddCommandToAllConsole(string commandText, Action<[IConsoleWriter](#IConsoleWriter), string[]> action)` Add a command to all consoles, with text specified in commandText parameter. The associated action require an [IConsoleWriter](#IConsoleWriter) parameter to write in selected console and a string array that contain all the parameter passed to a command through input. If command already exist in one console the method throws an exception
* `void RemoveCommandFromAllConsole(string commandText)` Remove specified command from all console, if exist
* `void ExecuteCommandToAllConsole(Action<[IConsoleWriter](#IConsoleWriter)> action)` yyyyy
* `void CommandsReceived(Action<string, bool>; action)` The specified action will be fired at every command sended, the string parameter is the command, the boolean parameters is true if this command was managed by a console
* `void Run()` This method raise the ConsoleAsync cicle, and waiting for worker or console command, until Quit method was called
* `void Quit()` Stop and destroy every worker and console, then quit all ConsoleAsync functionality


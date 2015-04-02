[back to summary](summary.md)

References
------------------------------------------------------------------------

* [ConsoleAsync](#consoleasync)
* [IConsole](#iconsole)
* [IConsoleWriter](#iconsolewriter)
* [IConsoleWorker](#iconsoleworker)
* [ConsoleWorker](#consoleworker)
* [Extensions](#extensions)



------------------------------------------------------------------------
##ConsoleAsync
ConsoleAsync is a static class needed for create/destroy console, for worker management cicle 
and helper method to make operation to all existing consoles

####Properties

#####ConsoleAsync.AvailableInputChars
```c#
string AvailableInputChars { get; set; }
```
This property contain a string with all allowed char for input, all char not in this string will be ignored
 
#####ConsoleAsync.ActiveConsole
```c#
IConsole ActiveConsole { get; }
```
This property return the actual visible console

####Methods

#####ConsoleAsync.CreateConsole
```c#
 IConsole CreateConsole(string consoleName)
```
Create a new console with specified name and return [IConsole](#IConsole) interface

#####ConsoleAsync.DestroyConsole
```c#
void DestroyConsole(string consoleName)
```
Destroy console with specified name if the console is the last one this method throws an exception

#####ConsoleAsync.ShowConsole
```c#
void ShowConsole(string consoleName)
```
Make console with specified name visible

#####ConsoleAsync.EnumerateConsoles
```c#
IEnumerable<IConsole> EnumerateConsoles()
```
Return anumeration of existing [IConsole](#IConsole)

#####ConsoleAsync.AddCommandToAllConsole
```c#
void AddCommandToAllConsole(string commandText, Action<IConsoleWriter, string[]> action)
```
Add a command to all consoles, with text specified in commandText parameter. The associated action require an [IConsoleWriter](#IConsoleWriter) parameter to write in selected console and a string array that contain all the parameter passed to a command through input. If command already exist in one console the method throws an exception

#####ConsoleAsync.RemoveCommandFromAllConsole
```c#
void RemoveCommandFromAllConsole(string commandText)
```
Remove specified command from all console, if exist

#####ConsoleAsync.ExecuteCommandToAllConsole
```c#
void ExecuteCommandToAllConsole(Action<IConsoleWriter> action)
```
Execute an action to all consoles, with [IConsoleWriter](#IConsoleWriter) parameter to write in selected console

#####ConsoleAsync.CommandsReceived
```c#
void CommandsReceived(Action<string, bool>; action)
```
The specified action will be fired at every command sended, the string parameter is the command, the boolean parameters is true if this command was managed by a console

#####ConsoleAsync.Run
```c#
void Run()
```
This method raise the ConsoleAsync cicle, and waiting for worker or console command, until Quit method was called

#####ConsoleAsync.Quit
```c#
void Quit()
```
Stop and destroy every worker and console, then quit all ConsoleAsync functionality



------------------------------------------------------------------------
##IConsole
This interface manage the single console instance

####Properties

#####IConsole.Name
```c#
string Name { get; }
```
The name of the console

####Methods

#####IConsole.GetWriter
```c#
IConsoleWriter GetWriter();
```
Return the writer interface to access the console output

#####IConsole.AddCommand
```c#
void AddCommand(string commandText, Action<IConsoleWriter, string[]> action)
```
Add a command to console, with text specified in commandText parameter. The associated action require an [IConsoleWriter](#IConsoleWriter)
parameter to write in selected console and a string array that contain all the parameter passed to a command through input
 
*If command already exist in console the method throws an exception*

#####IConsole.RemoveCommand
```c#
void RemoveCommand(string commandText action)
```
Remove command with specified name from console

#####IConsole.AddKeyCommand
```c#
void AddKeyCommand(KeyCommandEnum key, string command)
void AddKeyCommand(KeyCommandEnum key, string command, bool autoSend)
```
Associate a string command to a specified function key (F1...F10), when the key is pressed the command is issued
 
*If autoSend is false, when user press relative key, the command will only writed in input line*

#####IConsole.ClearKeyCommand
```c#
void ClearKeyCommand(KeyCommandEnum key)
```
Remove association of specified function key

#####IConsole.ClearKeyCommand
```c#
void Execute(Action<IConsoleWriter, string[]> action)
```
Immediately execute action command in actual console with the relative writer as parameter

#####IConsole.AddWorker
```c#
IConsoleWorker AddWorker(ConsoleWorker consoleWorker)
```
Add worker to execution queue of this console, and return an interface with management helper method

#####IConsole.AddWorker
```c#
void Destroy()
```
Stop all worker and destroy current console.
 
**if the console is the last one this method throws an exception**



------------------------------------------------------------------------
##IConsoleWriter
This interface manage the output of the console instance, all the output methods return same IConsoleWriter object to permit fluent syntax like:
```c#
Writer.Text("Hey ").Info("John Doe").Text(" you are welcome !").NewLine();
```

####Properties

#####IConsoleWriter.ConsoleName
```c#
string ConsoleName { get; }
```
Return the name of the parent console

####Output Methods

#####IConsoleWriter.Info
```c#
IConsoleWriter Info(string text)
IConsoleWriter Info(string format, params object[] parameters)
```
Write content of text parameter with information style, methos support also [String.Format](https://msdn.microsoft.com/it-it/library/b1csw23d(v=vs.110).aspx) parameters

#####IConsoleWriter.Warning
```c#
IConsoleWriter Warning(string text)
IConsoleWriter Warning(string format, params object[] parameters)
```
Write content of text parameter with warning style, methos support also [String.Format](https://msdn.microsoft.com/it-it/library/b1csw23d(v=vs.110).aspx) parameters

#####IConsoleWriter.Error
```c#
IConsoleWriter Error(string text)
IConsoleWriter Error(string format, params object[] parameters)
```
Write content of text parameter with error style, methos support also [String.Format](https://msdn.microsoft.com/it-it/library/b1csw23d(v=vs.110).aspx) parameters

#####IConsoleWriter.Muted
```c#
IConsoleWriter Muted(string text)
IConsoleWriter Muted(string format, params object[] parameters)
```
Write content of text parameter with muted style, methos support also [String.Format](https://msdn.microsoft.com/it-it/library/b1csw23d(v=vs.110).aspx) parameters

#####IConsoleWriter.Text
```c#
IConsoleWriter Text(string text)
IConsoleWriter Text(string format, params object[] parameters)
```
Write content of text parameter with default style, methos support also [String.Format](https://msdn.microsoft.com/it-it/library/b1csw23d(v=vs.110).aspx) parameters

#####IConsoleWriter.ClearCurrentLine
```c#
IConsoleWriter ClearCurrentLine()
```
Clear the current line of output and back the cursor on the first character

#####IConsoleWriter.Clear
```c#
IConsoleWriter Clear()
```
Clear all output of the console instance

#####IConsoleWriter.NewLine
```c#
IConsoleWriter NewLine()
```
Terminate current output line and create a new one

####Other Methods

#####IConsoleWriter.CancelSaveOutputToFile
```c#
void CancelSaveOutputToFile()
```
Stop the save to file operation

#####IConsoleWriter.SaveOutputToFile
```c#
void SaveOutputToFile(string directory, string name)
void SaveOutputToFile(string directory, string name, int linesPerFile, int linesPerFlush)
```
Save the output of the current console to multiple files, every file will have the linesPerFile parameter of line each and raise a flush every linesPerFlush parameter line.
The generated path is *(directory parameter)/(name parameter)-(formatted date)-(counter).txt*
If linesPerFile and linesPerFlush are omitted the default values is 1000 line per file and 50 line per flush

#####IConsoleWriter.ScrollTop
```c#
void ScrollTop()
```
Scroll the current view to the top

#####IConsoleWriter.ScrollBottom
```c#
void ScrollBottom()
```
Scroll the current view to the bottom


------------------------------------------------------------------------
##IConsoleWorker
This interface manage the worker instance

####Properties

#####IConsoleWorker.State
```c#
WorkerStateEnum State { get; }
```
Return the state of the worker (Stopped, Running, Suspended)

####Methods

#####IConsoleWorker.State
```c#
void Start()
```
Start the relative worker by call OnStart method then run the execution cicle

#####IConsoleWorker.Stop
```c#
void Stop()
```
Stop the execution cicle the call OnStop method

#####IConsoleWorker.Suspend
```c#
void Suspend()
```
Suspend temporarely the execution cicle

#####IConsoleWorker.Resume
```c#
void Resume()
```
Resume execution cicle

#####IConsoleWorker.Execute
```c#
void Execute()
```
Raise OnExecute method immediately


------------------------------------------------------------------------
##ConsoleWorker
Abstract class for the creation of custom worker, all method and properties must be overrided

####Properties

#####ConsoleWorker.IntervalBetweenExecution
```c#
TimeSpan IntervalBetweenExecution { get; }
```
Value returned from this property is the interval between call of OnExecute method

####Methods

#####ConsoleWorker.OnStart
```c#
void OnStart(IConsoleWriter writer);
```
Called before start of the execution cicle

#####ConsoleWorker.OnExecute
```c#
void OnExecute(IConsoleWriter writer, TimeSpan elapsed);
```
Called at interval specified by IntervalBetweenExecution property, parameter elapsed indicate exact interval occourred from previous call

#####ConsoleWorker.OnStop
```c#
void OnStop(IConsoleWriter writer, TimeSpan elapsed);
```
Called at the stop of execution cicle parameter elapsed indicate exact interval occourred from previous call



------------------------------------------------------------------------
##Extensions
Extension class with some utility method

####String Methods

```c#
string Left(int length)
```
Return the first characters of a string specified by length parameter

```c#
string LeftRest(int length)
```
Return the last characters of a string without the first characters specified by length parameter, inverse of Left function

```c#
string Right(int length)
```
Return the last characters of a string specified by length parameter

```c#
string RightRest(int length)
```
Return the first characters of a string without the last characters specified by length parameter, inverse of Left function

```c#
string Chunk(int startIndex, int endIndex)
```
Return subset of string starting and ending from specified parameters

```c#
string Fit(int totalLength, char filler = ' ')
```
Return a string with length specified by totalLength parameter, eventually filled or truncated

```c#
string Truncate(int totalLength, string ellipsis = "_")
```
Truncate string at the specified length and insert ellipsis character only if the string length is greater than the specified total length

```c#
string RemoveChar(int charIndex)
```
Return string without a specified character

```c#
string FitMultiline(int lineLength)
```
Cut source string in multiple lines with a length specified by lineLength parameter

####Long Methods

```c#
string ToFileSize()
```
Return a string with the long value converted as file size (Eg. 10Bt / 10Mb / 10Gb)

[back to top](#References) - [back to summary](summary.md)

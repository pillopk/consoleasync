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

```c#
string AvailableInputChars { get; set; }
```
This property contain a string with all allowed char for input, all char not in this string will be ignored
 
```c#
IConsole ActiveConsole { get; }
```
This property return the actual visible console

####Methods

```c#
 IConsole CreateConsole(string consoleName)
```
Create a new console with specified name and return [IConsole](#IConsole) interface

```c#
void DestroyConsole(string consoleName)
```
Destroy console with specified name if the console is the last one this method throws an exception

```c#
void ShowConsole(string consoleName)
```
Make console with specified name visible

```c#
IEnumerable<IConsole> EnumerateConsoles()
```
Return anumeration of existing [IConsole](#IConsole)

```c#
void AddCommandToAllConsole(string commandText, Action<IConsoleWriter, string[]> action)
```
Add a command to all consoles, with text specified in commandText parameter. The associated action require an [IConsoleWriter](#IConsoleWriter) parameter to write in selected console and a string array that contain all the parameter passed to a command through input. If command already exist in one console the method throws an exception

```c#
void RemoveCommandFromAllConsole(string commandText)
```
Remove specified command from all console, if exist

```c#
void ExecuteCommandToAllConsole(Action<IConsoleWriter> action)
```
Execute an action to all consoles, with [IConsoleWriter](#IConsoleWriter) parameter to write in selected console

```c#
void CommandsReceived(Action<string, bool>; action)
```
The specified action will be fired at every command sended, the string parameter is the command, the boolean parameters is true if this command was managed by a console

```c#
void Run()
```
This method raise the ConsoleAsync cicle, and waiting for worker or console command, until Quit method was called

```c#
void Quit()
```
Stop and destroy every worker and console, then quit all ConsoleAsync functionality



------------------------------------------------------------------------
##IConsole
blah blah blah blah blah blah blah blah


------------------------------------------------------------------------
##IConsoleWriter
blah blah blah blah blah blah blah blah


------------------------------------------------------------------------
##IConsoleWorker
blah blah blah blah blah blah blah blah


------------------------------------------------------------------------
##ConsoleWorker
blah blah blah blah blah blah blah blah



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

[back to summary](summary.md)

[back to summary](summary.md)

##Console Commands
------------------------------------------------------------------------

When a console is create a new instance of [IConsole](references.md#iconsole) will be returned
```c#
IConsole console = ConsoleAsync.CreateConsole("First Console");
```
with this object we can create/remove commands for this specific console
 

####Managing commands
```c#
console.AddCommand("print", (writer, strings) =>
{
    if (strings.Length > 0)
        writer.Text(strings[0]).NewLine();
});
```
This sample add a new command called "print" and define the action to execute when this command is issued via user input.
The action consist in write the first argument passed (if exist) to console output
>The user input is splitted upon spaces, the first item is considered the command, others are parameters
>The command comparison is case insensitive

```c#
console.RemoveCommand("print");
```
This sample remove previously created command , in this case the command 'print' will be removed from console
 

####Managing commands keys
```c#
console.AddKeyCommand(KeyCommandEnum.F1, "print");
```
In this case a command line is linked to key F1, when this key is pressed the command was launched
 

```c#
console.AddKeyCommand(KeyCommandEnum.F1, "first par1 par2", false);
```
In this case a command line is linked to key F1, when this key is pressed the command was launched.
When autoSend parameter is false the command is not executed, instead is write in the user input line
>The specified command does not necessarily contain only the command, can also contain parameters
 
 
```c#
console.ClearKeyCommand(KeyCommandEnum.F1);
```
In this case the key F1 is cleared, no more command line is bound
 


####Managing commands in all consoles
The main object [ConsoleAsync](references.md#consoleasync) contain utility method for manage command in all consoles
 
 
```c#
ConsoleAsync.AddCommandToAllConsole("identify", (writer, strings) =>
{
    writer.Text(writer.ConsoleName).NewLine();
});
```
This sample add a command named 'identify' to all consoles, when issued the console write is name on the output.
 
  
```c#
ConsoleAsync.RemoveCommandFromAllConsole("identify");
```
This sample remove the command named 'identify' from all consoles (if exist)
 

####Send commands
 
 
```c#
IConsole testConsole = ConsoleAsync.CreateConsole("TestConsole");

bool managed1 = ConsoleAsync.SendCommand("TestConsole", "print one two three");  // Send command to a console by name
bool managed2 = testConsole.SendCommand("print one two three");                  // Send command to a console by object
```
Send a command to a console, return true if command was found and executed
 

####Manage inputs
 
 
```c#
IConsole fallbackConsole = ConsoleAsync.CreateConsole("Fallback Console");
ConsoleAsync.CommandsReceived((command, managed) =>
{
    if (!managed)
        fallbackConsole.GetWriter().Info("Unknow command '{0}'", command).NewLine();
});
```
All commands, existing or not in the console, call the method ConsoleAsync.CommandsReceived with a Boolean parameter 
that identifies whether the command was executed in any console
This sample demonstrate how intercept all unknow command and send a message to a utility console
>Note that every console object can retrieve its own writer through [IConsole.GetWriter](references.md#iconsolegetwriter) method



[back to top](#console-commands) - [back to summary](summary.md)
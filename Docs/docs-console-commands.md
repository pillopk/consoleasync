[back to summary](summary.md)

##Console Commands
------------------------------------------------------------------------

When a console is create a new instance of [IConsole](references.md#IConsole) will be returned
```c#
IConsole console = ConsoleAsync.CreateConsole("First Console");
```
with this object we can create/remove commands for this specific console
 

####Managing Commands
```c#
console.AddCommand("first", (writer, strings) =>
{
    if (strings.Length > 0)
        writer.Text(strings[0]).NewLine();
});
```
In this sample AddCommand method add a new command called "first" and define the action to execute when this command is issued via user input.
The action consist in write the first argument passed (if exist) to console output
>The user input is splitted upon spaces, the first item is considered the command, others are parameters
>The command comparison is case insensitive

```c#
console.RemoveCommand("first");
```
To remove previously created command we need to use RemoveCommand methods, in this case the command 'first' will be removed from console
 


[back to top](#console-commands) - [back to summary](summary.md)
[back to summary](summary.md)

##Console Management
------------------------------------------------------------------------

[ConsoleAsync](references.md#consoleasync) is the main static object that manage all the console in use,
with this object console can be created or destroyed
 

```c#
IConsole console1 = ConsoleAsync.CreateConsole("First Console");
IConsole console2 = ConsoleAsync.CreateConsole("Second Console");
```
With these commands three different consoles will be created, with TAB key consoles is cicled in the viewport.
[ConsoleAsync.ActiveConsole](references.md#consoleasync.activeconsole) return the [IConsole](references.md#iconsole) of the actual visible console.
>In the bottom/left corner of console window is present a string that indicate wath console is visible over the total
 

```c#
IConsole testConsole = ConsoleAsync.CreateConsole("TestConsole");

ConsoleAsync.ShowConsole("TestConsole"); //Show console from its name
testConsole.Show();                      //Show console from its object
```
This sample make "Third Console" visible programmatically
 

```c#
foreach (IConsole console in ConsoleAsync.EnumerateConsoles())
{
    console.ClearKeyCommand(KeyCommandEnum.F3);
}
```
Method ConsoleAsync.EnumerateConsoles return an IEnumerable of actual consoles, in this case is used to remove command from key F3
 

```c#
IConsole testConsole = ConsoleAsync.CreateConsole("TestConsole");

ConsoleAsync.DestroyConsole("TestConsole"); //Destroy console from its name
testConsole.Destroy();                      //Destroy console from its object
```
This sample show how to destroy specific console





[back to top](#console-management) - [back to summary](summary.md)
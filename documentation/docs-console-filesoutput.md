[back to summary](summary.md)

##File output
------------------------------------------------------------------------

The output of any console may be saved on a multiple text file

```c#
IConsole console = ConsoleAsync.CreateConsole("Console");

console1.SaveOutputToFile(@"c:\backup", "log");
```
This sample create a console and send output to multiple file in folder *c:\backup*, the file name have *log* as prefix
generated file have the default parameters of 1000 row per file and 50 row every stream flush

```c#
IConsole console = ConsoleAsync.CreateConsole("Console");

console.SaveOutputToFile(@"c:\backup", "log, 3000, 100);
```
This sample create a console and send output to multiple file in folder *c:\backup*, the file name have *log* as prefix
generated file have 3000 row per file and 100 row every stream flush


```c#
console.CancelSaveOutputToFile();
```
This code cancel the output to file


[back to top](#console-commands) - [back to summary](summary.md)
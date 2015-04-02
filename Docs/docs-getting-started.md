[back to summary](summary.md)

Getting Started
------------------------------------------------------------------------
```c#
IConsole console = ConsoleAsync.CreateConsole("Getting Started");

console.Execute(writer =>
{
	writer.Info("Getting started console").NewLine();
});

console.AddCommand("print", (writer, strings) =>
{
	foreach (string s in strings)
	{
		writer.Text(s).NewLine();
	}
});

console.AddCommand("quit", (writer, strings) =>
{
	ConsoleAsync.Quit();
});

ConsoleAsync.Run();
```

This sample create a console, add some commands and execute it. When user type a command in the input line (Eg. print one two three) the first word is considered a command (in this case 'print') then check if exist a command with that name.
If the command exist the relative action will be called with writer object and the rest of command parameter (in this case ['one', 'two', 'three']).<br /><br />
 
 
**Take a deeper look on the code:**
 
```c#
IConsole console = ConsoleAsync.CreateConsole("Getting Started");
```
Create a console called 'Getting Started' and return its control object, the first time ConsoleAsync.CreateConsole() be called entire ConsoleAsync rendering cicle will be initialized

```c#
console.Execute(writer => {
	writer.Info("Getting started console").NewLine();
});
```
This fragment execute an action immediately on the console object, in this case write a text in info style and create a new line

```c#
console.AddCommand("print", (writer, strings) =>
{
	foreach (string s in strings)
	{
		writer.Text(s).NewLine();
	}
});
```
This fragment add 'print' command to a console, when the command is issued all the parameters will be written on the console.
Eg. command 'print one two three' will print 'one', 'two' and 'three' word in console

```c#
console.AddCommand("quit", (writer, strings) =>
{
	ConsoleAsync.Quit();
});
```
This fragment add 'quit' command to a console, when typed the ConsoleAsync.Quit() method is called entire ConsoleAsync object with all the console and worker is closed

```c#
ConsoleAsync.Run();
```
Execute the console cicle, the program execution is stopped on this method until ConsoleAsync.Quit() is called

>Note that the same functionality will be achieved with expression syntax

```c#
IConsole console = ConsoleAsync.CreateConsole("Getting Started");
console.Execute(writer => writer.Info("Getting started console").NewLine());
console.AddCommand("print", (writer, strings) => strings.ToList().ForEach(s => writer.Text(s).NewLine()));
console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
ConsoleAsync.Run();
```

[back to summary](summary.md)

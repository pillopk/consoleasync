ConsoleAsync
========
Restyling of standard C# console with commands creation, multiple consoles and support for asyncronous worker

* Documentation
	* [Getting Started](#getting-started)
	* [Console Management](#console-management)
	* [Console Commands](#console-commands)
	* [Asyncronous Worker](#asyncronous-worker)
* References
	* [ConsoleAsync](References.md#consoleasync)
	* [IConsole](References.md#iconsole)
	* [IConsoleWriter](References.md#iconsolewriter)
	* [IConsoleWorker](References.md#iconsoleworker)
	* [ConsoleWorker](References.md#consoleworker)
	* [Extensions](References.md#extensions)
* Builtin
	* [TimedWorker](#timedworker)
	* [FolderWatcherWorker](#folderwatcherworker)
	* [ObjectWatcherWorker](#objectwatcherworker)


Getting Started
---------------

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

This sample create a console, add some commands and execute it. When user type a command in the input line (Eg. print one two three) the first word is considered a command (in this case 'print') then check if exist a command with that name.
If the command exist the relative action will be called with writer object and the rest of command parameter (in this case ['one', 'two', 'three']).<br /><br />
 
 
**Take a deeper look on the code:**
 
	IConsole console = ConsoleAsync.CreateConsole("Getting Started");
Create a console called 'Getting Started' and return its control object, the first time ConsoleAsync.CreateConsole() be called entire ConsoleAsync rendering cicle will be initialized

	console.Execute(writer => {
		writer.Info("Getting started console").NewLine();
	});
This fragment execute an action immediately on the console object, in this case write a text in info style and create a new line

	console.AddCommand("print", (writer, strings) =>
	{
		foreach (string s in strings)
		{
			writer.Text(s).NewLine();
		}
	});
This fragment add 'print' command to a console, when the command is issued all the parameters will be written on the console.
Eg. command 'print one two three' will print 'one', 'two' and 'three' word in console

	console.AddCommand("quit", (writer, strings) =>
	{
		ConsoleAsync.Quit();
	});
This fragment add 'quit' command to a console, when typed the ConsoleAsync.Quit() method is called entire ConsoleAsync object with all the console and worker is closed

	ConsoleAsync.Run();
Execute the console cicle, the program execution is stopped on this method until ConsoleAsync.Quit() is called

>Note that the same functionality will be achieved with expression syntax

	IConsole console = ConsoleAsync.CreateConsole("Getting Started");
	console.Execute(writer => writer.Info("Getting started console").NewLine());
	console.AddCommand("print", (writer, strings) => strings.ToList().ForEach(s => writer.Text(s).NewLine()));
	console.AddCommand("quit", (writer, strings) => ConsoleAsync.Quit());
	ConsoleAsync.Run();

## Extensions
Extension class with some utility method

###*String Methods*

* `string Left(int length)`  Return the first characters of a string specified by length parameter

* `string LeftRest(int length)`  Return the last characters of a string without the first characters specified by length parameter, inverse of Left function

* `string Right(int length)` Return the last characters of a string specified by length parameter

* `string RightRest(int length)` Return the first characters of a string without the last characters specified by length parameter, inverse of Left function

* `string Chunk(int startIndex, int endIndex)` Return subset of string starting and ending from specified parameters

* `string Fit(int totalLength, char filler = ' ')` Return a string with length specified by totalLength parameter, eventually filled or truncated

* `string Truncate(int totalLength, string ellipsis = "_")` Truncate string at the specified length and insert ellipsis character only if the string length is greater than the specified total length

* `string RemoveChar(int charIndex)` Return string without a specified character

* `string FitMultiline(int lineLength)` Cut source string in multiple lines with a length specified by lineLength parameter

###*Long Methods*

* `string ToFileSize()` Return a string with the long value converted as file size (Eg. 10Bt / 10Mb / 10Gb)


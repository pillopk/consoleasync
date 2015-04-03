ConsoleAsync Documentation
------------------------------------------------------------------------

* Documentation
	* [Getting Started](docs-getting-started.md)
	* [Console Management](docs-console-management.md)
	* [Console Commands](docs-console-commands.md)
	* [Output to File](docs-console-filesoutput.md)
	* [Asyncronous Worker](docs-asyncronous-worker.md)
	* [Keyboard Settings](docs-keyboard-settings.md)
* References
	* [ConsoleAsync](references.md#consoleasync)
	* [IConsole](references.md#iconsole)
	* [IConsoleWriter](references.md#iconsolewriter)
	* [IConsoleWorker](references.md#iconsoleworker)
	* [ConsoleWorker](references.md#consoleworker)
	* [Extensions](references.md#extensions)
* Builtin Workers
	* [TimedWorker](builtin.md#timedworker)
     



##Developer Notes
------------------------------------------------------------------------
*ConsoleAsync is not suitable for high precision asyncrony, actual precision is +/- 50 millisecond between [OnExecute](references.md#consoleworkeronexecute) cicle
*Console output is never clear automatically, for long output (like application log) plan to clear console over time, consider use of [output to file](docs-console-filesoutput.md) feature


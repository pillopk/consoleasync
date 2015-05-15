[back to summary](summary.md)

##Asyncronous Workers
------------------------------------------------------------------------
Custom worker are used for complex asyncronous operation, for simple opaeration use builtin [TimedWorker](builtin.md)

```c#
static void Main(string[] args)
{
	TestWorker customWorker = new TestWorker();
    
    IConsole console = ConsoleAsync.CreateConsole("Control Console");
    IConsoleWorker worker = console.AddWorker(customWorker);
    ConsoleAsync.Run(true);
}
```
This is the minimum program to execute a console with single custom worker,
the [IConsoleWorker](references.md#iconsoleworker) object is used for management of worker  

>The parameter of method [Run](references.md#consoleasync.run) starts all the workers
>registered in all consoles.

###Implementation
```c#
public class TestWorker : ConsoleWorker
{
    public override TimeSpan? IntervalBetweenExecution {}

    public override void OnStart(IConsoleWriter writer) {}

    public override void OnExecute(IConsoleWriter writer, TimeSpan elapsed) {}

    public override void OnStop(IConsoleWriter writer, TimeSpan elapsed) {}
}
```

ConsoleWorker is the abstract class to create a custom console worker, 
here the list of properties and methods required for the implementation.

#####ConsoleWorker.IntervalBetweenExecution
```c#
public override TimeSpan? IntervalBetweenExecution {}
```
The property IntervalBetweenExecution return the interval between asyncronous call,
if this value is null the process must be started manually 
with [Execute](references.md#iconsoleworker.execute) method.

#####ConsoleWorker.OnStart
```c#
public override void OnStart(IConsoleWriter writer) {}
```
This method will be called at the start of worker, immediately before the first OnExecute() call,
the worker can be started through the [Start](references.md#iconsoleworker.start) method

#####ConsoleWorker.OnExecute
```c#
public override void OnExecute(IConsoleWriter writer, TimeSpan elapsed) {}
```
This method will be called at interval specified by the  IntervalBetweenExecution property
or manually through the [Execute](references.md#iconsoleworker.execute) method

#####ConsoleWorker.OnStop
```c#
public override void OnStop(IConsoleWriter writer, TimeSpan elapsed) {}
```
This method will be called when the [Stop](references.md#iconsoleworker.stop) method is called,
after the last OnExecute() or at  [Quit](references.md#consoleasync.quit) of whole console.

>For info on controliing worker please refer the [IConsoleWorker](references.md#iconsoleworker) reference

[back to top](#asyncronous-worker) - [back to summary](summary.md) 
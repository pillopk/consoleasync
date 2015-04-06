[back to summary](summary.md)

##BuiltIn Workers
------------------------------------------------------------------------

####TimedWorker

Most of time dont need a complex logic for worker, need only a call every interval.
In this case is useful the class TimedWorker, here the constructors:

```c#
TimedWorker(TimeSpan interval, Action<IConsoleWriter, TimeSpan> onExecute)
TimedWorker(TimeSpan interval, Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart)
TimedWorker(TimeSpan interval, Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart, Action<IConsoleWriter> onStop)
```
The timespan parameter indicate the interval of call, the other parameters the action to call on start/execute/stop
  
    
```c#
TimedWorker(Action<IConsoleWriter, TimeSpan> onExecute)
TimedWorker(Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart)
TimedWorker(Action<IConsoleWriter, TimeSpan> onExecute, Action<IConsoleWriter> onStart, Action<IConsoleWriter> onStop)
```
These constructor are similar to other, but without interval, the newly created worker are manual, look [Asyncronous Workers](docs-asyncronous-worker.md) for further explanation
  
  

[back to top](#builtin-workers) - [back to summary](summary.md)
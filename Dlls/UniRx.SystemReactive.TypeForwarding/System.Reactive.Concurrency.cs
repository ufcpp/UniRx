using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(IScheduler))]
[assembly: TypeForwardedTo(typeof(Scheduler))]
[assembly: TypeForwardedTo(typeof(CurrentThreadScheduler))]
#if NET45
[assembly: TypeForwardedTo(typeof(ThreadPoolScheduler))]
#endif
[assembly: TypeForwardedTo(typeof(ImmediateScheduler))]

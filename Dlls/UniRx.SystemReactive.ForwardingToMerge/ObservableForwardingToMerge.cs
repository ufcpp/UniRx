using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace System.Reactive.Linq
{
    public static class ObservableForwardingToMerge
    {
        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            return MergeEx(sources, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources, IScheduler scheduler)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler == Scheduler.CurrentThread);
        }

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent)
        {
            return MergeEx(sources, maxConcurrent, Scheduler.DefaultSchedulers.ConstantTimeOperations);
        }

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), maxConcurrent, scheduler == Scheduler.CurrentThread);
        }

        public static IObservable<TSource> MergeEx<TSource>(params IObservable<TSource>[] sources)
        {
            return MergeEx(Scheduler.DefaultSchedulers.ConstantTimeOperations, sources);
        }

        public static IObservable<TSource> MergeEx<TSource>(IScheduler scheduler, params IObservable<TSource>[] sources)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler == Scheduler.CurrentThread);
        }

        public static IObservable<T> MergeEx<T>(this IObservable<T> first, params IObservable<T>[] seconds)
        {
            return MergeEx(CombineSources(first, seconds));
        }

        static IEnumerable<IObservable<T>> CombineSources<T>(IObservable<T> first, IObservable<T>[] seconds)
        {
            yield return first;
            for (int i = 0; i < seconds.Length; i++)
            {
                yield return seconds[i];
            }
        }

        public static IObservable<T> MergeEx<T>(this IObservable<T> first, IObservable<T> second, IScheduler scheduler)
        {
            return MergeEx(scheduler, new[] { first, second });
        }

        public static IObservable<T> MergeEx<T>(this IObservable<IObservable<T>> sources)
        {
            return new MergeObservable<T>(sources, false);
        }

        public static IObservable<T> MergeEx<T>(this IObservable<IObservable<T>> sources, int maxConcurrent)
        {
            return new MergeObservable<T>(sources, maxConcurrent, false);
        }
    }
}

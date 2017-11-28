using System.Collections.Generic;
using System.Reactive.Concurrency;
using UniRx.SystemReactive.UnityWorkaround;

namespace System.Reactive.Linq
{
    /// <summary>
    /// 本家System.ReactiveにあるMergeだとエラーが発生してしまうため
    /// 独自実装したMergeへ明示的にフォワードさせる拡張メソッド
    /// </summary>
    public static class ObservableForwardingToMerge
    {
        /// <summary>
        /// UniRxにあるScheduler.DefaultSchedulers.ConstantTimeOperations相当の処理
        /// </summary>
        private static IScheduler DefaultScheduler
        {
            get => _defaultScheduler ?? (_defaultScheduler = Scheduler.Immediate);
            set => _defaultScheduler = value;
        }
        private static IScheduler _defaultScheduler;

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            return MergeEx(sources, DefaultScheduler);
        }

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources, IScheduler scheduler)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler);
        }

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent)
        {
            return MergeEx(sources, maxConcurrent, DefaultScheduler);
        }

        public static IObservable<TSource> MergeEx<TSource>(this IEnumerable<IObservable<TSource>> sources, int maxConcurrent, IScheduler scheduler)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), maxConcurrent, scheduler);
        }

        public static IObservable<TSource> MergeEx<TSource>(params IObservable<TSource>[] sources)
        {
            return MergeEx(DefaultScheduler, sources);
        }

        public static IObservable<TSource> MergeEx<TSource>(IScheduler scheduler, params IObservable<TSource>[] sources)
        {
            return new MergeObservable<TSource>(sources.ToObservable(scheduler), scheduler);
        }

        public static IObservable<T> MergeEx<T>(this IObservable<T> first, int maxConcurrent, params IObservable<T>[] seconds)
        {
            return MergeEx(CombineSources(first, seconds), maxConcurrent);
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
            return new MergeObservable<T>(sources, DefaultScheduler);
        }

        public static IObservable<T> MergeEx<T>(this IObservable<IObservable<T>> sources, int maxConcurrent)
        {
            return new MergeObservable<T>(sources, maxConcurrent, DefaultScheduler);
        }
    }
}

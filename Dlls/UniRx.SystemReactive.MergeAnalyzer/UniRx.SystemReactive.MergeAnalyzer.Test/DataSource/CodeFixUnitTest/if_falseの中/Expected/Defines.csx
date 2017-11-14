namespace System
{
    public interface IObservable<T> { }
}
namespace System.Reactive.Subjects
{
    public class Subject<T> : IObservable<T>
    {
        public void OnNext(T t) { }
    }
}
namespace System.Reactive.Linq
{
    public static class Observable
    {
        public static IObservable<T> Where<T>(this IObservable<T> o, Func<T, bool> predicate) => null;
        public static IObservable<T> Merge<T>(this IObservable<T> o, IObservable<T> o2) => null;
        public static IDisposable Subscribe<T>(this IObservable<T> o, Action<T> a) => new EmptyDisposables();

        private class EmptyDisposables : IDisposable { void IDisposable.Dispose() { } }
    }
}
namespace MyNameSpace
{
    using System;

    public static class Observable
    {
        public static IObservable<T> MergeEx<T>(this IObservable<T> o, IObservable<T> o2) => null;
    }
}

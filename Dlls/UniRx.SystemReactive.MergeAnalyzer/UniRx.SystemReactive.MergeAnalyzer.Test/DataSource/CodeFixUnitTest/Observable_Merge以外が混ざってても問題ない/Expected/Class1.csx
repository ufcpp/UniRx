namespace ConsoleApp
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using MyNameSpace;

    static class MergeClass
    {
        public static IObservable<T> Merge<T>(this IObservable<T> a, IObservable<T> b) => null;
    }

    internal class Program
    {
        private static void Main()
        {
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            s1.Merge(s2);
            Observable.MergeEx(s1, s2);
        }
    }
}

namespace ConsoleApp
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using MyNameSpace;

    internal class Program
    {
        private static void Main()
        {
            var s = new Subject<int>();
            var o1 = s.Where(_ => true);
            var o2 = s.Where(_ => false);

            using (Observable.Merge(o1, o2).Subscribe(x => Console.WriteLine(x)))
            {
                foreach (var x in Enumerable.Range(0, 100)) s.OnNext(x);
            }
        }
    }
}

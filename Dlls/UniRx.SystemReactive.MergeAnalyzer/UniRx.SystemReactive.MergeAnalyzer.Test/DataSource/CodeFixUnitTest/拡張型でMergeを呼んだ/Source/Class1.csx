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
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();

            using (s1.Merge(s2).Subscribe(x => Console.WriteLine(x)))
            {
                foreach (var x in Enumerable.Range(0, 100)) s1.OnNext(x);
            }
        }
    }
}

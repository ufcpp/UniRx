using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace UniRx.SystemReactive.UnityWorkaround.UnitTest
{
    public class ObservableForwardingToMergeTest
    {
        [Fact]
        public void Merge()
        {
            var l = new List<int>();
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            using (s1.MergeEx(s2).Subscribe(x => l.Add(x)))
            {
                for (var i = 0; i < 10; i++)
                {
                    s1.OnNext(10 + i);
                    s2.OnNext(20 + i);
                }
            }

            var actual = Enumerable.Range(0, 10).SelectMany(x => new[] { 10 + x, 20 + x });
            Assert.Equal(actual, l);
        }

        [Fact]
        public void ConcurrentMerge()
        {
            var l = new List<int>();
            var s1 = new Subject<int>();
            var s2 = new Subject<int>();
            var s3 = new Subject<int>();
            using (s1.MergeEx(2, s2, s3).Subscribe(x => l.Add(x)))
            {
                for (var i = 0; i < 10; i++)
                {
                    s1.OnNext(10 + i);
                    s2.OnNext(20 + i);
                    s3.OnNext(30 + i);
                }

                s2.OnCompleted();

                for (var i = 0; i < 10; i++)
                {
                    s1.OnNext(10 + i);
                    s3.OnNext(30 + i);
                }
            }

            var actual = Enumerable.Range(0, 10).SelectMany(x => new[] { 10 + x, 20 + x })
                .Concat(Enumerable.Range(0, 10).SelectMany(x => new[] { 10 + x, 30 + x }));
            Assert.Equal(actual, l);
        }
    }
}

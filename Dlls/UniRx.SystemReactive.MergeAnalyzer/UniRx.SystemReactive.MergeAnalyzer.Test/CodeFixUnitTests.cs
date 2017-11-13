using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace UniRx.SystemReactive.MergeAnalyzer.Test
{
    [TestClass]
    public class CodeFixUnitTest : CodeFixVerifier
    {
        #region DefineCode

        private const string DefineCode = @"
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
";

        #endregion

        [TestMethod]
        public void Mergeを呼んだ()
        {
            #region code

            var code = DefineCode + @"
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

            using (o1.Merge(o2).Subscribe(x => Console.WriteLine(x)))
            {
                foreach (var x in Enumerable.Range(0, 100)) s.OnNext(x);
            }
        }
    }
}
";

            #endregion

            VerifyCSharpFix(code, code.Replace("o1.Merge", "o1.MergeEx"));
        }

        [TestMethod]
        public void 拡張型でMergeを呼んだ()
        {
            #region code

            var code = DefineCode + @"
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
";

            #endregion

            VerifyCSharpFix(code, code.Replace("s1.Merge", "s1.MergeEx"));
        }

        //[TestMethod]
        // 拡張メソッドの直呼び出しについては名前空間解決がうまくいかないので無視する
        public void 拡張メソッドのMergeを呼んだ()
        {
            #region code

            var code = DefineCode + @"
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
";

            #endregion

            VerifyCSharpFix(code, code.Replace("Observable.Merge", "Observable.MergeEx"));
        }

        [TestMethod]
        public void 空ドキュメント()
        {
            VerifyCSharpFix("", "");
        }

        [TestMethod]
        public void Mergeを呼ばない()
        {
            #region code

            var code = DefineCode + @"
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
            var o = s.Where(_ => true);

            using (o.Subscribe(x => Console.WriteLine(x)))
            {
                foreach (var x in Enumerable.Range(0, 100)) s.OnNext(x);
            }
        }
    }
}
";

            #endregion

            VerifyCSharpFix(code, code);
        }

        [TestMethod]
        public void if_falseの中()
        {
            #region code

            var code = DefineCode + @"
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
#if false
            var s = new Subject<int>();
            var o1 = s.Where(_ => true);
            var o2 = s.Where(_ => false);

            using (o1.Merge(o2).Subscribe(x => Console.WriteLine(x)))
            {
                foreach (var x in Enumerable.Range(0, 100)) s.OnNext(x);
            }
#endif
        }
    }
}
";

            #endregion

            VerifyCSharpFix(code, code);
        }

        [TestMethod]
        public void Observable_Merge以外は使用可能()
        {
            #region code

            var code = DefineCode + @"
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
        }
    }
}
";

            #endregion

            VerifyCSharpFix(code, code);
        }

        //[TestMethod]
        // 拡張メソッドの直呼び出しについては名前空間解決がうまくいかないので無視する
        public void Observable_Merge以外が混ざってても問題ない()
        {
            #region code

            var code = DefineCode + @"
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
            Observable.Merge(s1, s2);
        }
    }
}
";

            #endregion

            VerifyCSharpFix(code, code.Replace("Observable.Merge", "Observable.MergeEx"));
        }

        #region Impliments

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UniRxSystemReactiveMergeAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UniRxSystemReactiveMergeAnalyzerAnalyzer();
        }

        #endregion
    }
}
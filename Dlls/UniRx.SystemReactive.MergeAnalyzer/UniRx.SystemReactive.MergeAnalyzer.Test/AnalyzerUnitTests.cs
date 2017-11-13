using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace UniRx.SystemReactive.MergeAnalyzer.Test
{
    [TestClass]
    public class AnalyzerUnitTest : CodeFixVerifier
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

        private DiagnosticResult CreateResult(int line, int column)
            => new DiagnosticResult
            {
                Id = "UniRxSystemReactiveMergeAnalyzer",
                Message = "IObservable<T>.Merge()の使用禁止",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) }
            };

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

            var expected = CreateResult(49, 23);
            VerifyCSharpDiagnostic(code, expected);
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

            var expected = CreateResult(48, 23);
            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
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

            var expected = CreateResult(49, 31);
            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void 空ドキュメント()
        {
            VerifyCSharpDiagnostic("");
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

            VerifyCSharpDiagnostic(code);
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

            VerifyCSharpDiagnostic(code);
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

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void Observable_Merge以外が混ざっていても問題ない()
        {
            #region code

            var code = DefineCode + @"
namespace ConsoleApp
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

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

            var expected = CreateResult(53, 24);
            VerifyCSharpDiagnostic(code, expected);
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
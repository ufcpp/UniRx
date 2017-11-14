using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace UniRx.SystemReactive.MergeAnalyzer.Test
{
    public class CodeFixUnitTest : ConventionCodeFixVerifier
    {
        [Fact]
        public void Mergeを呼んだ() => VerifyCSharpByConvention();

        [Fact]
        public void 拡張型でMergeを呼んだ() => VerifyCSharpByConvention();

        //[Fact]
        // 拡張メソッドの直呼び出しについては名前空間解決がうまくいかないので無視する
        void 拡張メソッドのMergeを呼んだ() => VerifyCSharpByConvention();

        [Fact]
        public void 空ドキュメント() => VerifyCSharpFix("", "");

        [Fact]
        public void Mergeを呼ばない() => VerifyCSharpByConvention();

        [Fact]
        public void if_falseの中() => VerifyCSharpByConvention();

        [Fact]
        public void Observable_Merge以外は使用可能() => VerifyCSharpByConvention();

        //[Fact]
        // 拡張メソッドの直呼び出しについては名前空間解決がうまくいかないので無視する
        void Observable_Merge以外が混ざってても問題ない() => VerifyCSharpByConvention();

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
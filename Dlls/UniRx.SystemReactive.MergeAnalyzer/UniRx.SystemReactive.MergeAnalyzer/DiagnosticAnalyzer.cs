using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UniRx.SystemReactive.MergeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UniRxSystemReactiveMergeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        internal const string ReplaceMethodName = "MergeEx"; // TODO:名前が決まったら差し替え
        public const string DiagnosticId = "UniRxSystemReactiveMergeAnalyzer";

        private const string Title = "IObservable<T>.Merge()の使用禁止";
        private const string MessageFormat = Title;
        private const string Description = ReplaceMethodName + "を使用してください";
        private const string Category = "Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.InvocationExpression);

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var expr = context.Node as InvocationExpressionSyntax;
            if (expr == null) return;

            var methodSyntax = GetMethodCallIdentifier(expr);
            if (methodSyntax == null) return;
            if (methodSyntax.Identifier.Text != "Merge") return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(expr).Symbol;
            var typeName = symbolInfo?.ContainingType?.MetadataName;
            if (typeName != "Observable") return;

            var diagnostic = Diagnostic.Create(Rule, methodSyntax.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static SimpleNameSyntax GetMethodCallIdentifier(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is IdentifierNameSyntax i) return i;
            if (invocation.Expression is MemberAccessExpressionSyntax m) return m.Name;
            return null;
        }
    }
}

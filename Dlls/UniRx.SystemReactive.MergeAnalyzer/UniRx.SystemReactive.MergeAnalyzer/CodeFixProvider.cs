using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UniRx.SystemReactive.MergeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UniRxSystemReactiveMergeAnalyzerCodeFixProvider)), Shared]
    public class UniRxSystemReactiveMergeAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Use " + UniRxSystemReactiveMergeAnalyzerAnalyzer.ReplaceMethodName;

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(UniRxSystemReactiveMergeAnalyzerAnalyzer.DiagnosticId);
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();

            var targetToken = root.FindToken(context.Span.Start);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => MakeUppercaseAsync(context.Document, root, targetToken, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private Task<Document> MakeUppercaseAsync(Document document, SyntaxNode root, SyntaxToken targetToken, CancellationToken cancellationToken)
        {
            var newToken = SyntaxFactory.Identifier(targetToken.LeadingTrivia, UniRxSystemReactiveMergeAnalyzerAnalyzer.ReplaceMethodName, targetToken.TrailingTrivia);
            var newRoot = root.ReplaceToken(targetToken, newToken);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
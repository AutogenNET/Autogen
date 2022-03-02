using Autogen.Enum;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace Autogen.SourceGeneration;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Core.ISourceGenerator sourceGenerator = new Enum.SourceGenerator();

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "Autogen.SourceGeneration.SourceGenerator.g.cs",
            SourceText.From(sourceGenerator.AddSourcePostInit(), Encoding.UTF8)));

        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => sourceGenerator.IsSyntaxTargetForGeneration(s),
                transform: (ctx, _) => sourceGenerator.GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndEnums = context.CompilationProvider.Combine(enumDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndEnums,
            (spc, source) => sourceGenerator.Execute(source.Left, source.Right!, spc));
    }
}

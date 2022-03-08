
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace Autogen.SourceGeneration;

public abstract class SourceGenerator : IIncrementalGenerator
{
    protected abstract string AddSourcePostInit();
    protected abstract void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context);
    protected abstract EnumDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context);
    protected abstract bool IsSyntaxTargetForGeneration(SyntaxNode node);


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "Autogen.SourceGeneration.SourceGenerator.g.cs",
            SourceText.From(AddSourcePostInit(), Encoding.UTF8)));

        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => IsSyntaxTargetForGeneration(s),
                transform: (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndEnums = context.CompilationProvider.Combine(enumDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndEnums,
            (spc, source) => Execute(source.Left, source.Right!, spc));
    }
}

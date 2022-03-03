using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Autogen.Core
{
    public interface ISourceGenerator
    {
        string AddSourcePostInit();
        bool IsSyntaxTargetForGeneration(SyntaxNode node);
        EnumDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context);
        void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context);
    }
}

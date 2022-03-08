
using Autogen.SourceGeneration;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace Autogen.Enum;

[Generator]
public class EnumSourceGenerator : SourceGenerator
{
    private const string TargetAttribute = "Autogen.Enum.AutogenEnumAttribute";


    protected override string AddSourcePostInit()
    {
        return SourceGenerationHelper.Attribute;
    }

    protected override void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context)
    {
        if (enums.IsDefaultOrEmpty)
            return;

        var distinctEnums = enums.Distinct();

        var enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);
        if (enumsToGenerate.Any() is true)
        {
            // 소스 코드를 생성하고 출력에 추가
            string result = SourceGenerationHelper.GenerateExtensionClass(enumsToGenerate);
            context.AddSource("Autogen.Enum.EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    protected override EnumDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var enumDeclarationSyntax = (EnumDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in enumDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName is TargetAttribute)
                    return enumDeclarationSyntax;
            }
        }

        return null;
    }

    protected override bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private IEnumerable<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken ct)
    {
        var enumsToGenerate = new List<EnumToGenerate>();
        var enumAttribute = compilation.GetTypeByMetadataName(TargetAttribute);

        if (enumAttribute == null)
            return enumsToGenerate;

        foreach (EnumDeclarationSyntax enumDeclarationSyntax in enums)
        {
            ct.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
                continue;

            var enumName = enumSymbol.ToString();

            var enumMembers = enumSymbol.GetMembers();
            var members = new List<(string Name, string Text)>(enumMembers.Length);

            foreach (ISymbol member in enumMembers)
            {
                if (member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    var textAttribute = member.GetAttributes().Where(x => x.AttributeClass?.Name is "TextAttribute").FirstOrDefault();
                    if (textAttribute is null)
                        members.Add((member.Name, $"nameof({enumName}.{member.Name})"));
                    else
                    {
                        var text = textAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                        if (string.IsNullOrWhiteSpace(text) is true)
                            members.Add((member.Name, $"nameof({enumName}.{member.Name})"));
                        else
                            members.Add((member.Name, $"\"{text!}\""));
                    }
                }
            }

            enumsToGenerate.Add(new EnumToGenerate(enumName, members));
        }

        return enumsToGenerate;
    }
}

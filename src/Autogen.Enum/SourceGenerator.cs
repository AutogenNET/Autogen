
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System.Collections.Immutable;
using System.Text;

namespace Autogen.Enum;

public class SourceGenerator : Core.ISourceGenerator
{
    private const string TargetAttribute = "Autogen.Enum.AutogenEnumAttribute";


    public string AddSourcePostInit()
    {
        return SourceGenerationHelper.Attribute;
    }

    public void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context)
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

    public EnumDeclarationSyntax? GetSemanticTargetForGeneration(Microsoft.CodeAnalysis.GeneratorSyntaxContext context)
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

    public bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private IEnumerable<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken ct)
    {
        // 출력을 저장할 목록을 만듭니다.
        var enumsToGenerate = new List<EnumToGenerate>();
        // 마커 특성의 의미론적 표현을 얻습니다.
        INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName(TargetAttribute);

        if (enumAttribute == null)
        {
            // 이것이 null이면 Compilation에서 마커 특성 유형을 찾을 수 없습니다.
            // 이는 무언가 매우 잘못되었음을 나타냅니다. 구제..
            return enumsToGenerate;
        }

        foreach (EnumDeclarationSyntax enumDeclarationSyntax in enums)
        {
            // 우리가 요청하면 중지
            ct.ThrowIfCancellationRequested();

            // 열거형 구문의 의미론적 표현 얻기 
            SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
            {
                // 뭔가 잘못되었습니다, 구제  
                continue;
            }

            // 열거형의 전체 유형 이름을 가져옵니다. e.g. Colour,
            // 또는 OuterClass<T>.Colour가 제네릭 형식에 중첩된 경우(예)
            string enumName = enumSymbol.ToString();

            // 열거형의 모든 멤버 가져오기 
            ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
            var members = new List<string>(enumMembers.Length);

            // 열거형에서 모든 필드를 가져오고 해당 이름을 목록에 추가합니다. 
            foreach (ISymbol member in enumMembers)
            {
                if (member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    members.Add(member.Name);
                }
            }

            // 생성 단계에서 사용할 EnumToGenerate 생성 
            enumsToGenerate.Add(new EnumToGenerate(enumName, members));
        }

        return enumsToGenerate;
    }
}

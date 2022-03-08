using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using VerifyXunit;
using Autogen.SourceGeneration;
using Autogen.Enum;

namespace Autogen.Tests;

public class TestHelper
{
    public static Task Verify(string source)
    {
        // 제공된 문자열을 C# 구문 트리로 구문 분석
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        // 필요한 어셈블리에 대한 참조 생성
        // 필요한 경우 여러 참조를 추가할 수 있습니다.
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        // 구문 트리에 대한 Roslyn 컴파일 생성
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references
            );

        // EnumGenerator 증분 소스 생성기의 인스턴스 생성
        var generator = new EnumSourceGenerator();

        // GeneratorDriver는 컴파일에 대해 생성기를 실행하는데 사용됨
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // 소스 생성기를 실행!
        driver = driver.RunGenerators(compilation);

        // 소스 생성기 출력을 스냅샷 테스트하려면 Verifier를 사용!
        return Verifier.Verify(driver);
    }
}

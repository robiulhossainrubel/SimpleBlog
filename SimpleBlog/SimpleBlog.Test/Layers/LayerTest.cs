using NetArchTest.Rules;
using Shouldly;

namespace SimpleBlog.Test.Layers;

public class LayerTest : BaseTest
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Any_Layer()
    {
        List<string> Assemblies = [
            ApplicationAssembly.GetName().Name,
            InfrastructureAssembly.GetName().Name,
            PresentationAssembly.GetName().Name
            ];

        foreach (var assembly in Assemblies)
        {
            TestResult result = Types.InAssembly(DomainAssembly)
                .ShouldNot()
                .HaveDependencyOn(assembly)
                .GetResult();
            result.IsSuccessful.ShouldBeTrue();
        }
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Any_Layer_Expect_Domain()
    {
        List<string> Assemblies = [
            InfrastructureAssembly.GetName().Name,
            PresentationAssembly.GetName().Name
        ];

        foreach (var assembly in Assemblies)
        {
            TestResult result = Types.InAssembly(ApplicationAssembly)
                .ShouldNot()
                .HaveDependencyOn(assembly)
                .GetResult();
            result.IsSuccessful.ShouldBeTrue();
        }
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Presentation()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationAssembly.GetName().Name)
            .GetResult();
        result.IsSuccessful.ShouldBeTrue();
    }
}
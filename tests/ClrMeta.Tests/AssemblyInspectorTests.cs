using System.Linq;
using ClrMeta.Core;

namespace ClrMeta.Tests;

public class AssemblyInspectorTests
{
    private static AssemblyMetadata InspectCore() =>
        AssemblyInspector.Inspect(typeof(AssemblyInspector).Assembly.Location);

    [Fact]
    public void PublicTypes_Contains_AssemblyInspector()
    {
        var meta = InspectCore();
        Assert.Contains(meta.PublicTypes, t => t.FullName == "ClrMeta.Core.AssemblyInspector");
    }

    [Fact]
    public void ReferencedAssemblies_IsNotEmpty()
    {
        var meta = InspectCore();
        Assert.NotEmpty(meta.ReferencedAssemblies);
    }
}

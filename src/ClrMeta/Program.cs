using ClrMeta.Core;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: clr-meta <assembly.dll>");
    return 1;
}

var path = args[0];
if (!File.Exists(path))
{
    Console.Error.WriteLine($"File not found: {path}");
    return 1;
}

var meta = AssemblyInspector.Inspect(path);

Console.WriteLine($"Assembly: {meta.Name}");
Console.WriteLine($"Version:  {meta.Version}");

Console.WriteLine($"\nReferenced assemblies ({meta.ReferencedAssemblies.Length}):");
foreach (var r in meta.ReferencedAssemblies)
    Console.WriteLine($"  - {r}");

Console.WriteLine($"\nPublic types ({meta.PublicTypes.Length}):");
foreach (var t in meta.PublicTypes)
{
    Console.WriteLine($"  {t.FullName}");
    foreach (var m in t.MethodNames)
        Console.WriteLine($"    - {m}()");
}

return 0;

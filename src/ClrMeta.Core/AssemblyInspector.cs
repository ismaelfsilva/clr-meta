using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ClrMeta.Core;

/// <summary>A public type and its public method names.</summary>
public sealed record TypeMetadata(string FullName, ImmutableArray<string> MethodNames);

/// <summary>Metadata extracted from a .NET assembly without loading it.</summary>
public sealed record AssemblyMetadata(
    string Name,
    string Version,
    ImmutableArray<string> ReferencedAssemblies,
    ImmutableArray<TypeMetadata> PublicTypes);

/// <summary>
/// Reads .NET assembly metadata via <see cref="MetadataReader"/> without loading
/// or executing the target assembly, so it works cross-platform.
/// </summary>
public static class AssemblyInspector
{
    public static AssemblyMetadata Inspect(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var pe = new PEReader(stream);
        var reader = pe.GetMetadataReader();

        var asm = reader.GetAssemblyDefinition();

        var references = reader.AssemblyReferences
            .Select(h => reader.GetString(reader.GetAssemblyReference(h).Name))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToImmutableArray();

        var types = ImmutableArray.CreateBuilder<TypeMetadata>();
        foreach (var handle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(handle);
            if ((type.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
                continue; // top-level public types only

            var ns = reader.GetString(type.Namespace);
            var name = reader.GetString(type.Name);
            var fullName = string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";

            var methods = type.GetMethods()
                .Select(reader.GetMethodDefinition)
                .Where(m => (m.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
                .Select(m => reader.GetString(m.Name))
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToImmutableArray();

            types.Add(new TypeMetadata(fullName, methods));
        }

        return new AssemblyMetadata(
            reader.GetString(asm.Name),
            asm.Version.ToString(),
            references,
            types.OrderBy(t => t.FullName, StringComparer.Ordinal).ToImmutableArray());
    }
}

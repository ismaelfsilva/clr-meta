# clr-meta

A .NET assembly metadata dumper for reverse engineering and inspection. It reads
assembly metadata using `System.Reflection.Metadata` (`MetadataReader`) without
loading or executing the target assembly, so it works cross-platform.

It reports:

- Assembly name and version
- Referenced assembly names
- Public type full-names with their public method names

## Build

```bash
dotnet build
```

## Usage

```bash
clr-meta <assembly.dll>
```

Example:

```bash
dotnet run --project src/ClrMeta -- path/to/Some.dll
```

## Test

```bash
dotnet test
```

## License

MIT

#!/usr/local/bin/dotnet run
// Файл file-based app скрипта для определения текущей версии NuGet пакета из удаленного репозитория.
// Использование: dotnet run .scripts/nuget-ver-remote.cs <package-name>

#nullable enable

#:package NuGet.Protocol@7.0.1
#:package NuGet.Configuration@7.0.1

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run .scripts/nuget-ver-remote.cs <package-name>");
    Environment.Exit(1);
}

var package_name = args[0]; // имя пакета из аргумента
if (string.IsNullOrWhiteSpace(package_name))
{
    Console.Error.WriteLine("Package name is empty");
    Environment.Exit(1);
}

try
{
    // Создадим репозиторий NuGet (v3 API)
    var source_repo = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

    // Получим ресурс метаданных пакета
    using var cache = new SourceCacheContext();
    var logger = NullLogger.Instance;
    var resource = await source_repo.GetResourceAsync<PackageMetadataResource>().ConfigureAwait(false);

    // Запросим все метаданные по пакету (включая pre-release), не включая unlisted
    var metadata = await resource.GetMetadataAsync(package_name, includePrerelease: true, includeUnlisted: false, cache, logger, CancellationToken.None).ConfigureAwait(false);

    var metadata_list = metadata?.ToList() ?? new System.Collections.Generic.List<IPackageSearchMetadata>();
    if (metadata_list.Count == 0)
    {
        Console.Error.WriteLine($"Package not found: {package_name}");
        Environment.Exit(2);
    }

    // Соберём версии и выберем последнюю стабильную, если есть, иначе последнюю доступную
    var versions = metadata_list
        .Select(m => m.Identity.Version)
        .Where(v => v is not null)
        .OrderBy(v => v)
        .ToArray();

    if (versions.Length == 0)
    {
        Console.Error.WriteLine("No versions found");
        Environment.Exit(2);
    }

    var stable_version = versions
        .Where(v => !v.IsPrerelease)
        .OrderByDescending(v => v)
        .FirstOrDefault()
        ?? versions.OrderByDescending(v => v).First();

    var latest_version = stable_version.ToNormalizedString();

    Console.WriteLine(latest_version); // вывод версии в stdout
    Environment.Exit(0);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(2);
}
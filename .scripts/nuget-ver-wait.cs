#!/usr/local/bin/dotnet run
// Файл file-based app скрипта для ожидания появления указанной версии NuGet пакета на сервере.
// Использование: dotnet run .scripts/nuget-ver-wait.cs <package-name> <target-version> [-n <tries>] [-t <timeout-ms>]

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

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet run .scripts/nuget-ver-wait.cs <package-name> <target-version> [-n <tries>] [-t <timeout-ms>]");
    Environment.Exit(1);
}

var package_name = args[0]; // имя пакета
var target_version_str = args[1]; // требуемая версия
if (string.IsNullOrWhiteSpace(package_name) || string.IsNullOrWhiteSpace(target_version_str))
{
    Console.Error.WriteLine("Package name or target version is empty");
    Environment.Exit(1);
}

// Значения по умолчанию
var tries = 10; // -n по умолчанию
var timeout_ms = 1000; // -t по умолчанию

// Разбор дополнительных аргументов
for (var i = 2; i < args.Length; i++)
{
    var a = args[i];
    const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
    if (string.Equals(a, "-n", cmp) && i + 1 < args.Length)
    {
        if (int.TryParse(args[++i], out var v)) tries = v;
    }
    else if (string.Equals(a, "-t", cmp) && i + 1 < args.Length)
    {
        if (int.TryParse(args[++i], out var v)) timeout_ms = v;
    }
}

if (tries <= 0) tries = 1;
if (timeout_ms < 0) timeout_ms = 0;

if (!NuGetVersion.TryParse(target_version_str, out var target_version))
{
    Console.Error.WriteLine($"Невозможно распарсить целевую версию: {target_version_str}");
    Environment.Exit(1);
}

Console.WriteLine($"Ожидание версии {target_version} пакета {package_name} на nuget.org ({tries} попыток, таймаут {timeout_ms}ms)");

try
{
    var source_repo = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
    using var cache = new SourceCacheContext();
    var logger = NullLogger.Instance;
    var resource = await source_repo.GetResourceAsync<PackageMetadataResource>();

    for (var attempt = 1; attempt <= tries; attempt++)
    {
        // Получим метаданные пакета
        var metadata = await resource.GetMetadataAsync(package_name, includePrerelease: true, includeUnlisted: false, cache, logger, CancellationToken.None);
        var metadata_list = metadata?.ToList() ?? [];

        if (metadata_list.Count == 0)
            Console.WriteLine($"[{attempt}/{tries}] Пакет не найден на сервере");
        else
        {
            var versions = metadata_list
                .Select(m => m.Identity.Version)
                .Where(v => v is not null)
                .OrderByDescending(v => v)
                .ToArray();

            if (versions.Length == 0)
                Console.WriteLine($"[{attempt}/{tries}] На сервере нет версий пакета");
            else
            {
                var latest = versions[0];
                Console.WriteLine($"[{attempt}/{tries}] Серверная последняя версия: {latest}");

                // Сравним последнюю серверную версию с целевой
                if (latest < target_version)
                    Console.WriteLine($"Серверная версия {latest} младше требуемой {target_version}, ожидаем...");
                else
                {
                    Console.WriteLine($"Требуемая версия {target_version} доступна на сервере (серверная версия {latest})");
                    Environment.Exit(0);
                }
            }
        }

        if (attempt < tries)
            await Task.Delay(timeout_ms);
    }

    Console.Error.WriteLine($"Не удалось дождаться версии {target_version} для пакета {package_name} после {tries} попыток");
    Environment.Exit(2);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Ошибка: {ex.Message}");
    Environment.Exit(2);
}
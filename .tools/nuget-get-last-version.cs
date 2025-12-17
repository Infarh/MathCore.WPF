#!/usr/bin/env dotnet
#:package NuGet.Protocol@6.10.1
#:package NuGet.Configuration@6.10.1
#:package NuGet.Versioning@6.10.1
#:package NuGet.Common@6.10.1

/*
Использование:
    dotnet run nuget-get-last-version.cs [<путь_к_.csproj> | <идентификатор_пакета>]


Коды возврата:
    0  — успешное завершение
   -1  — запрос помощи (--help, -h)
   -2  — ошибка использования (отсутствует аргумент, пустой аргумент)
    1  — непредвиденная ошибка выполнения (catch)
    2  — ошибка чтения/парсинга .csproj
    3  — отсутствуют элементы <PackageReference> в .csproj
    5  — версия пакета не найдена
*/

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Common; // для NullLogger

if (args.Length < 1)
{
    Console.Error.WriteLine("Не указан путь к .csproj"); // ошибка использования
    Console.Out.WriteLine("Использование: dotnet run nuget-get-last-version.cs [<путь_к_.csproj> | <идентификатор_пакета>]");
    Environment.Exit(-2);
}

var input = args[0].Trim();
if (input.Length == 0)
{
    Console.Error.WriteLine("Аргумент пуст"); // ошибка использования
    Console.Out.WriteLine("Использование: dotnet run nuget-get-last-version.cs [<путь_к_.csproj> | <идентификатор_пакета>]");
    Environment.Exit(-2);
}

if(input == "--help" || input == "-h")
{
    Console.Out.WriteLine("Использование: dotnet run nuget-get-last-version.cs [<путь_к_.csproj> | <идентификатор_пакета>]");
    Environment.Exit(-1);
}

// Если передан путь к .csproj, извлечь идентификатор пакета из PackageReference
string package_id;
if (File.Exists(input) && string.Equals(Path.GetExtension(input), ".csproj", StringComparison.OrdinalIgnoreCase))
    try
    {
        var xml = XDocument.Load(input); // загрузка проекта
        // Пытаемся найти ссылку на пакет MathCore, иначе берём первый PackageReference
        var pkg_mathcore = xml
            .Root?
            .Descendants("PackageReference")
            .FirstOrDefault(e => string.Equals((string?)e.Attribute("Include"), "MathCore", StringComparison.OrdinalIgnoreCase));

        var pkg = pkg_mathcore ?? xml.Root?.Descendants("PackageReference").FirstOrDefault();
        package_id = (string?)pkg?.Attribute("Include") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(package_id))
        {
            Console.Error.WriteLine("В .csproj не найдены элементы <PackageReference>"); // отсутствуют требуемые данные
            Environment.Exit(3);
        }
    }
    catch (Exception e)
    {
        Console.Error.WriteLine($"Ошибка чтения .csproj: {e.Message}"); // ошибка парсинга
        Environment.Exit(2);
        return;
    }
else
    package_id = input; // непосредственно ID пакета

try
{
    if (await GetLatestStableVersionAsync(package_id, CancellationToken.None) is not { } version)
    {
        Console.Error.WriteLine($"Не удалось определить последнюю версию пакета {package_id}"); // ресурс/версия не найдены
        Environment.Exit(5);
    }

    Console.Out.Write(version.ToString()); // вывод версии
}
catch (Exception e)
{
    Console.Error.WriteLine($"Ошибка: {e.Message}"); // общая непредвиденная ошибка выполнения
    Environment.Exit(1);
}

static async Task<NuGetVersion?> GetLatestStableVersionAsync(string PackageId, CancellationToken Cancel)
{
    var providers = Repository.Provider.GetCoreV3(); // провайдеры ресурсов
    var source = new PackageSource("https://api.nuget.org/v3/index.json"); // индекс nuget.org
    var repo = new SourceRepository(source, providers);

    var metadata = await repo.GetResourceAsync<PackageMetadataResource>(Cancel); // ресурс метаданных
    var search = await metadata.GetMetadataAsync(
        PackageId,
        includePrerelease: true,
        includeUnlisted: false,
        new SourceCacheContext(),
        NullLogger.Instance,
        Cancel);

    var versions = search.Select(m => m.Identity.Version).OrderBy(v => v); // сортировка по возрастанию
    var latest_stable = versions.LastOrDefault(v => !v.IsPrerelease); // последняя стабильная

    return latest_stable ?? versions.LastOrDefault(); // если стабильной нет, взять последнюю вообще
}
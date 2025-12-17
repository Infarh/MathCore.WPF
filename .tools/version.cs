#!/usr/bin/env dotnet

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

if (args.Length < 1)
{
    Console.Error.WriteLine("Укажите путь к .csproj файлу"); // проверка аргументов
    Environment.Exit(1);
}

var project_path = args[0]; // путь к проекту

if (!File.Exists(project_path))
{
    Console.Error.WriteLine($"Файл не найден: {project_path}"); // проверка наличия файла
    Environment.Exit(2);
}

XDocument doc;
try
{
    doc = XDocument.Load(project_path); // загрузка XML
}
catch (Exception e)
{
    Console.Error.WriteLine($"Ошибка загрузки XML: {e.Message}"); // ошибка парсинга
    Environment.Exit(3);
    return;
}

// поиск версии в PropertyGroup/Version
var version = doc
    .Root?
    .Descendants("PropertyGroup")
    .Elements("Version")
    .Select(v => (string?)v.Value)
    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

if (string.IsNullOrWhiteSpace(version))
{
    Console.Error.WriteLine("Свойство <Version> не найдено"); // версия не обнаружена
    Environment.Exit(4);
}
else
{
    Console.Out.Write(version.Trim()); // вывод версии в stdout
}
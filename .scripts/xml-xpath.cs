#!/usr/local/bin/dotnet run
// Файл file-based app скрипта для выполнения XPath запросов к XML файлам.
// запуск через команду: dotnet run .scripts/xml-xpath.cs <xml-file> <xpath-query>

using System;
using System.Xml;

if (args.Length != 2)
{
    Console.WriteLine("Usage: dotnet run .scripts/xml-xpath.cs <xml-file> <xpath-query>");
    return;
}

var xml_file = args[0];
var xpath_query = args[1];

XmlDocument xml_doc = new XmlDocument();
xml_doc.Load(xml_file);

var nodes = xml_doc.SelectNodes(xpath_query);
if (nodes is null || nodes.Count == 0)
    Console.WriteLine("No nodes found."); // нет найденных узлов
else
    // используем явный тип XmlNode чтобы не получать object и иметь доступ к OuterXml
    foreach (XmlNode node in nodes)
        Console.WriteLine(node.OuterXml);

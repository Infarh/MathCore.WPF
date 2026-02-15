////////////////////////////////////////////////////////////////////////////////
//
//  SvgReader.cs - This file is part of Svg2Xaml.
//
//    Copyright (C) 2009 Boris Richter <himself@boris-richter.net>
//
//  --------------------------------------------------------------------------
//
//  Svg2Xaml is free software: you can redistribute it and/or modify it under 
//  the terms of the GNU Lesser General Public License as published by the 
//  Free Software Foundation, either version 3 of the License, or (at your 
//  option) any later version.
//
//  Svg2Xaml is distributed in the hope that it will be useful, but WITHOUT 
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//  FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
//  License for more details.
//  
//  You should have received a copy of the GNU Lesser General Public License 
//  along with Svg2Xaml. If not, see <http://www.gnu.org/licenses/>.
//
//  --------------------------------------------------------------------------
//
//  $LastChangedRevision: 25245 $
//  $LastChangedDate: 2009-06-19 13:33:47 +0200 (Fri, 19 Jun 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System.IO;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

///****************************************************************************
/// <summary>Предоставляет методы для загрузки и рендеринга SVG документов</summary>
public static class SvgReader
{

    //==========================================================================
    /// <summary>
    /// Загружает SVG документ и преобразует его в объект <see cref="DrawingImage"/>
    /// </summary>
    /// <param name="reader">
    /// Объект <see cref="XmlReader"/> для чтения XML структуры SVG документа
    /// </param>
    /// <param name="options">
    /// Объект <see cref="SvgReaderOptions"/> для настройки параметров парсинга и рендеринга SVG
    /// </param>
    /// <returns>Объект <see cref="DrawingImage"/> с преобразованным SVG документом</returns>
    /// <exception cref="XmlException">Корневой элемент не находится в пространстве имён 'http://www.w3.org/2000/svg' или не является элементом &lt;svg&gt;</exception>
    /// <remarks>
    /// Этот метод является основным методом для загрузки SVG документов. Документ должен иметь корректное пространство имён и корневой элемент &lt;svg&gt;
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// using (var stream = File.OpenRead("image.svg"))
    /// {
    ///     var xmlReader = XmlReader.Create(stream);
    ///     var drawingImage = SvgReader.Load(xmlReader);
    ///     // Использование DrawingImage для отображения SVG
    /// }
    /// ]]></code>
    /// </example>
    public static DrawingImage Load(XmlReader reader, SvgReaderOptions options)
    {
        options ??= new();

        var document = XDocument.Load(reader);
        if(document.Root.Name.NamespaceName != "http://www.w3.org/2000/svg")
            throw new XmlException("Корневой элемент не находится в пространстве имён 'http://www.w3.org/2000/svg'.");
        return document.Root.Name.LocalName == "svg"
            ? new SvgDocument(document.Root, options).Draw()
            : throw new XmlException("Корневой элемент не является элементом <svg>.");
    }

    //==========================================================================
    /// <summary>
    /// Загружает SVG документ и преобразует его в объект <see cref="DrawingImage"/> с параметрами по умолчанию
    /// </summary>
    /// <param name="reader">
    /// Объект <see cref="XmlReader"/> для чтения XML структуры SVG документа
    /// </param>
    /// <returns>Объект <see cref="DrawingImage"/> с преобразованным SVG документом</returns>
    /// <example>
    /// <code><![CDATA[
    /// var xmlReader = XmlReader.Create("image.svg");
    /// var drawingImage = SvgReader.Load(xmlReader);
    /// ]]></code>
    /// </example>
    public static DrawingImage Load(XmlReader reader) => Load(reader, null);

    //==========================================================================
    /// <summary>
    /// Загружает SVG документ из потока и преобразует его в объект <see cref="DrawingImage"/>
    /// </summary>
    /// <param name="stream">
    /// Объект <see cref="Stream"/> для чтения содержимого SVG документа
    /// </param>
    /// <param name="options">
    /// Объект <see cref="SvgReaderOptions"/> для настройки параметров парсинга и рендеринга SVG
    /// </param>
    /// <returns>Объект <see cref="DrawingImage"/> с преобразованным SVG документом</returns>
    /// <remarks>
    /// Этот метод автоматически создаёт XmlReader с отключённой обработкой DTD для безопасности
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// using (var fileStream = File.OpenRead("image.svg"))
    /// {
    ///     var options = new SvgReaderOptions { IgnoreEffects = false };
    ///     var drawingImage = SvgReader.Load(fileStream, options);
    ///     // Использование DrawingImage для отображения или сохранения
    /// }
    /// ]]></code>
    /// </example>
    public static DrawingImage Load(Stream stream, SvgReaderOptions options)
    {
        using var reader = XmlReader.Create(stream, new() { DtdProcessing = DtdProcessing.Ignore });
        return Load(reader, options);
    }

    //==========================================================================
    /// <summary>
    /// Загружает SVG документ из потока и преобразует его в объект <see cref="DrawingImage"/> с параметрами по умолчанию
    /// </summary>
    /// <param name="stream">
    /// Объект <see cref="Stream"/> для чтения содержимого SVG документа
    /// </param>
    /// <returns>Объект <see cref="DrawingImage"/> с преобразованным SVG документом</returns>
    /// <example>
    /// <code><![CDATA[
    /// var stream = new MemoryStream(svgBytes);
    /// var drawingImage = SvgReader.Load(stream);
    /// ]]></code>
    /// </example>
    public static DrawingImage Load(Stream stream) => Load(stream, null);
} // class SvgReader
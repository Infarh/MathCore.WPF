////////////////////////////////////////////////////////////////////////////////
//
//  SvgBaseElement.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18569 $
//  $LastChangedDate: 2009-03-18 14:05:21 +0100 (Wed, 18 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System.Xml;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>
/// Базовый класс для всех элементов SVG документа
/// </summary>
/// <remarks>
/// Класс содержит общие свойства и логику обработки для всех элементов SVG.
/// Выполняет парсинг атрибутов style и href, управление идентификаторами элементов
/// </remarks>
internal class SvgBaseElement
{

    //==========================================================================
    /// <summary>Документ, к которому принадлежит данный элемент</summary>
    public readonly SvgDocument Document;

    //==========================================================================
    /// <summary>Ссылка на другой элемент (при наличии атрибута href)</summary>
    public readonly string? Reference;

    //==========================================================================
    /// <summary>Корневой элемент SVG документа</summary>
    public SvgSvgElement Root => Document.Root;

    //==========================================================================
    /// <summary>Родительский элемент в иерархии SVG</summary>
    public readonly SvgBaseElement Parent;

    //==========================================================================
    /// <summary>Уникальный идентификатор элемента (из атрибута id)</summary>
    public readonly string Id;

    //==========================================================================
    /// <summary>Исходный XElement из которого был создан данный элемент</summary>
    public readonly XElement Element;

    //==========================================================================
    /// <summary>Инициализирует новый экземпляр класса <see cref="SvgBaseElement"/></summary>
    /// <param name="document">SVG документ, содержащий элемент</param>
    /// <param name="parent">Родительский элемент в иерархии SVG</param>
    /// <param name="element">XElement, представляющий этот элемент в XML дереве</param>
    /// <remarks>
    /// Конструктор выполняет парсинг style атрибутов, регистрирует элемент в документе
    /// если у него есть id, и извлекает информацию о ссылке (href)
    /// </remarks>
    protected SvgBaseElement(SvgDocument document, SvgBaseElement parent, XElement element)
    {
        Document = document;
        Parent   = parent;

        // Преобразование стилей в атрибуты...
        var style_attribute = element.Attribute("style");
        if(style_attribute != null)
        {
            foreach(var property in style_attribute.Value.Split(';'))
            {
                var tokens = property.Split(':');
                if (tokens.Length != 2) continue;

                try
                {
                    element.SetAttributeValue(tokens[0], tokens[1]);
                }
                catch(XmlException)
                { }
            }
            style_attribute.Remove();
        }

        // Регистрация элемента по идентификатору
        var id_attribute = element.Attribute("id");
        if(id_attribute != null)
            Document.Elements[Id = id_attribute.Value] = this;

        // Извлечение ссылки на элемент (xlink:href)
        if(element.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")) is { Value: [ '#', _ ] reference })
            Reference = reference[1..];

        Element = element;
    }

} // class SvgBaseElement
////////////////////////////////////////////////////////////////////////////////
//
//  SvgDocument.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18341 $
//  $LastChangedDate: 2009-03-17 13:03:30 +0100 (Tue, 17 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////

using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>
/// Представляет внутреннюю структуру загруженного SVG документа для рендеринга
/// </summary>
/// <remarks>
/// Класс используется внутренне механизмом рендеринга SVG и не должен использоваться напрямую.
/// Выполняет роль контейнера элементов SVG документа и отвечает за управление их состоянием
/// </remarks>
internal sealed class SvgDocument
{
    //==========================================================================
    /// <summary>
    /// Словарь элементов документа, индексируемый по идентификаторам (id атрибутам)
    /// </summary>
    public readonly Dictionary<string, SvgBaseElement> Elements = [];

    //==========================================================================
    /// <summary>Корневой элемент SVG документа</summary>
    public readonly SvgSvgElement Root;
    
    /// <summary>Параметры для рендеринга документа</summary>
    public readonly SvgReaderOptions Options;

    //==========================================================================
    /// <summary>Инициализирует новый экземпляр класса <see cref="SvgDocument"/></summary>
    /// <param name="root">Корневой элемент (svg) из XElement</param>
    /// <param name="options">Параметры для настройки рендеринга</param>
    /// <remarks>
    /// Конструктор создаёт структуру документа на основе XElement корневого элемента
    /// и параметров рендеринга
    /// </remarks>
    public SvgDocument(XElement root, SvgReaderOptions options)
    {
        Root    = new(this, null, root);
        Options = options;
    }

    //==========================================================================
    /// <summary>
    /// Выполняет рендеринг SVG документа в объект <see cref="DrawingImage"/>
    /// </summary>
    /// <returns>Объект <see cref="DrawingImage"/> содержащий отрисованный SVG</returns>
    /// <remarks>
    /// Метод вызывает метод Draw корневого элемента и оборачивает результат в DrawingImage
    /// для использования в WPF элементах управления
    /// </remarks>
    public DrawingImage Draw() => new(Root.Draw());
} // class SvgDocument
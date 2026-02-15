////////////////////////////////////////////////////////////////////////////////
//
//  SvgReaderOptions.cs - This file is part of Svg2Xaml.
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

using System.Windows.Media.Effects;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>
/// Определяет набор параметров для настройки парсинга и рендеринга SVG документов
/// </summary>
/// <remarks>
/// Класс используется для передачи опций в методы загрузки SvgReader.
/// Позволяет контролировать применение эффектов и другие параметры рендеринга
/// </remarks>
/// <example>
/// <code><![CDATA[
/// // Создание опций с отключением эффектов
/// var options = new SvgReaderOptions { IgnoreEffects = true };
/// 
/// using (var stream = File.OpenRead("image.svg"))
/// {
///     var drawingImage = SvgReader.Load(stream, options);
/// }
/// ]]></code>
/// </example>
public class SvgReaderOptions
{

    //==========================================================================
    private bool _IgnoreEffects;

    //==========================================================================
    /// <summary>Инициализирует новый экземпляр класса <see cref="SvgReaderOptions"/></summary>
    /// <remarks>Создаёт параметры с значениями по умолчанию</remarks>
    public SvgReaderOptions()
    {
        // ...
    }

    //==========================================================================
    /// <summary>Инициализирует новый экземпляр класса <see cref="SvgReaderOptions"/> с указанными параметрами</summary>
    /// <param name="IgnoreEffects">
    /// Значение, указывающее должны ли фильтр-эффекты SVG игнорироваться или преобразовываться в растровые эффекты WPF
    /// </param>
    /// <example>
    /// <code><![CDATA[
    /// // Создание опций с игнорированием эффектов
    /// var options = new SvgReaderOptions(ignoreEffects: true);
    /// ]]></code>
    /// </example>
    public SvgReaderOptions(bool IgnoreEffects) => _IgnoreEffects = IgnoreEffects;

    //==========================================================================
    /// <summary>
    /// Получает или устанавливает значение, указывающее должны ли SVG эффекты игнорироваться или преобразовываться в растровые эффекты WPF
    /// </summary>
    /// <remarks>
    /// Значение true отключает обработку фильтр-эффектов, значение false включает преобразование эффектов в <see cref="BitmapEffect"/>
    /// </remarks>
    public bool IgnoreEffects 
    {
        get => _IgnoreEffects;
        set => _IgnoreEffects = value;
    }

} // class SvgReaderOptions
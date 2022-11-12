////////////////////////////////////////////////////////////////////////////////
//
//  SvgColorPaint.cs - This file is part of Svg2Xaml.
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

using System.Windows.Media;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  A paint with a solid color.</summary>
class SvgColorPaint
    : SvgPaint
{
    //==========================================================================
    public readonly SvgColor Color;

    //==========================================================================
    public SvgColorPaint(SvgColor color) => Color = color;

    //==========================================================================
    public override Brush ToBrush(SvgBaseElement element) => new SolidColorBrush(Color.ToColor());
} // class SvgColorPaint
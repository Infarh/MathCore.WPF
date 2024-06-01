////////////////////////////////////////////////////////////////////////////////
//
//  SvgCircleElement.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18622 $
//  $LastChangedDate: 2009-03-18 17:37:38 +0100 (Wed, 18 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents an &lt;circle&gt; element.</summary>
internal class SvgCircleElement
    : SvgDrawableBaseElement
{
    //==========================================================================
    /// <summary>  The x-coordinate of the circle's center.</summary>
    public readonly SvgCoordinate CenterX = new(0);

    //==========================================================================
    /// <summary>  The y-coordinate of the circle's center.</summary>
    public readonly SvgCoordinate CenterY = new(0);

    //==========================================================================
    /// <summary>  The circle's radius.</summary>
    public readonly SvgLength Radius = new(0);

    //==========================================================================
    public SvgCircleElement(SvgDocument document, SvgBaseElement parent, XElement CircleElement)
        : base(document, parent, CircleElement)
    {
        var cx_attribute = CircleElement.Attribute("cx");
        if(cx_attribute != null)
            CenterX = SvgCoordinate.Parse(cx_attribute.Value);

        var cy_attribute = CircleElement.Attribute("cy");
        if(cy_attribute != null)
            CenterY = SvgCoordinate.Parse(cy_attribute.Value);

        var r_attribute = CircleElement.Attribute("r");
        if(r_attribute != null)
            Radius = SvgLength.Parse(r_attribute.Value);
    }

    //==========================================================================
    public override Geometry GetBaseGeometry() => new EllipseGeometry(new(CenterX.ToDouble(), CenterY.ToDouble()), 
        Radius.ToDouble(), Radius.ToDouble());
} // class SvgCircleElement
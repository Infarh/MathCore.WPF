////////////////////////////////////////////////////////////////////////////////
//
//  SvgEllipseElement.cs - This file is part of Svg2Xaml.
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
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents an &lt;ellipse&gt; element.</summary>
internal class SvgEllipseElement
    : SvgDrawableBaseElement
{
    //==========================================================================
    public readonly SvgCoordinate CenterX = new(0.0);
    public readonly SvgCoordinate CenterY = new(0.0);
    public readonly SvgLength RadiusX = new(0.0);
    public readonly SvgLength RadiusY = new(0.0);

    //==========================================================================
    public SvgEllipseElement(SvgDocument document, SvgBaseElement parent, XElement EllipseElement)
        : base(document, parent, EllipseElement)
    {
        var cx_attribute = EllipseElement.Attribute("cx");
        if(cx_attribute != null)
            CenterX = SvgCoordinate.Parse(cx_attribute.Value);

        var cy_attribute = EllipseElement.Attribute("cy");
        if(cy_attribute != null)
            CenterY = SvgCoordinate.Parse(cy_attribute.Value);

        var rx_attribute = EllipseElement.Attribute("rx");
        if(rx_attribute != null)
            RadiusX = SvgCoordinate.Parse(rx_attribute.Value);

        var ry_attribute = EllipseElement.Attribute("ry");
        if(ry_attribute != null)
            RadiusY = SvgCoordinate.Parse(ry_attribute.Value);
    }

    //==========================================================================
    public override Geometry GetBaseGeometry() => new EllipseGeometry(
        new(CenterX.ToDouble(), CenterY.ToDouble()), RadiusX.ToDouble(),
        RadiusY.ToDouble());
} // class SvgEllipseElement
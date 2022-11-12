////////////////////////////////////////////////////////////////////////////////
//
//  SvgLinearGradientElement.cs - This file is part of Svg2Xaml.
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
/// <summary>  Represents a &lt;linearGradient&gt; element.</summary>
class SvgLinearGradientElement
    : SvgGradientBaseElement
{
    //==========================================================================
    public readonly SvgCoordinate X1 = new(0);
    public readonly SvgCoordinate Y1 = new(0);
    public readonly SvgCoordinate X2 = new(1);
    public readonly SvgCoordinate Y2 = new(0);

    //==========================================================================
    public SvgLinearGradientElement(SvgDocument document, SvgBaseElement parent, XElement LinearGradientElement)
        : base(document, parent, LinearGradientElement)
    {
        var x1_attribute = LinearGradientElement.Attribute("x1");
        if (x1_attribute != null)
            X1 = SvgCoordinate.Parse(x1_attribute.Value);

        var y1_attribute = LinearGradientElement.Attribute("y1");
        if (y1_attribute != null)
            Y1 = SvgCoordinate.Parse(y1_attribute.Value);

        var x2_attribute = LinearGradientElement.Attribute("x2");
        if (x2_attribute != null)
            X2 = SvgCoordinate.Parse(x2_attribute.Value);

        var y2_attribute = LinearGradientElement.Attribute("y2");
        if (y2_attribute != null)
            Y2 = SvgCoordinate.Parse(y2_attribute.Value);
    }

    //==========================================================================
    protected override GradientBrush CreateBrush() => new LinearGradientBrush();

    //==========================================================================
    protected override GradientBrush SetBrush(GradientBrush brush)
    {
        if (base.SetBrush(brush) is not LinearGradientBrush linear_gradient_brush) return brush;
        linear_gradient_brush.StartPoint = new Point(X1.ToDouble(), Y1.ToDouble());
        linear_gradient_brush.EndPoint   = new Point(X2.ToDouble(), Y2.ToDouble());
        return brush;
    }

} // class SvgLinearGradientElement
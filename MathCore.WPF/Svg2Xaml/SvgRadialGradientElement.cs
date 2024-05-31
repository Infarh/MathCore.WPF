////////////////////////////////////////////////////////////////////////////////
//
//  SvgRadialGradientElement.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 17282 $
//  $LastChangedDate: 2009-03-12 23:10:41 +0100 (Thu, 12 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
class SvgRadialGradientElement
    : SvgGradientBaseElement
{
    //==========================================================================
    public readonly SvgCoordinate Cx = new(0.5);
    public readonly SvgCoordinate Cy = new(0.5);
    public readonly SvgLength R = new SvgCoordinate(0.5);
    public readonly SvgCoordinate Fx;
    public readonly SvgCoordinate Fy;

    //==========================================================================
    public SvgRadialGradientElement(SvgDocument document, SvgBaseElement parent, XElement RadialGradientElement)
        : base(document, parent, RadialGradientElement)
    {
        var cx_attribute = RadialGradientElement.Attribute("cx");
        if (cx_attribute != null)
            Cx = SvgCoordinate.Parse(cx_attribute.Value);

        var cy_attribute = RadialGradientElement.Attribute("cy");
        if (cy_attribute != null)
            Cy = SvgCoordinate.Parse(cy_attribute.Value);

        var r_attribute = RadialGradientElement.Attribute("r");
        if (r_attribute != null)
            R = SvgCoordinate.Parse(r_attribute.Value);

        var fx_attribute = RadialGradientElement.Attribute("fx");
        if (fx_attribute != null)
            Fx = SvgCoordinate.Parse(fx_attribute.Value);

        var fy_attribute = RadialGradientElement.Attribute("fy");
        if (fy_attribute != null)
            Fy = SvgCoordinate.Parse(fy_attribute.Value);

    }

    //==========================================================================
    protected override GradientBrush CreateBrush()
    {
        var brush = new RadialGradientBrush();
        brush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
        return brush;
    }

    //==========================================================================
    protected override GradientBrush SetBrush(GradientBrush brush)
    {
        if (base.SetBrush(brush) is not RadialGradientBrush radial_gradient_brush) return brush;
        var cx = Cx.ToDouble();
        var cy = Cy.ToDouble();
        var fx = Fx?.ToDouble() ?? cx;
        var fy = Fy?.ToDouble() ?? cy;

        radial_gradient_brush.GradientOrigin = new(fx, fy);
        radial_gradient_brush.RadiusX        = R.ToDouble();
        radial_gradient_brush.RadiusY        = R.ToDouble();
        radial_gradient_brush.Center         = new(cx, cy);
        return brush;
    }
} // class SvgRadialGradientElement
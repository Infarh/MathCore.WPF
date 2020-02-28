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

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  class SvgRadialGradientElement
    : SvgGradientBaseElement
  {
    //==========================================================================
    public readonly SvgCoordinate CX = new SvgCoordinate(0.5);
    public readonly SvgCoordinate CY = new SvgCoordinate(0.5);
    public readonly SvgLength R = new SvgCoordinate(0.5);
    public readonly SvgCoordinate FX;
    public readonly SvgCoordinate FY;

    //==========================================================================
    public SvgRadialGradientElement(SvgDocument document, SvgBaseElement parent, XElement radialGradientElement)
      : base(document, parent, radialGradientElement)
    {
      var cx_attribute = radialGradientElement.Attribute("cx");
      if(cx_attribute != null)
        CX = SvgCoordinate.Parse(cx_attribute.Value);

      var cy_attribute = radialGradientElement.Attribute("cy");
      if(cy_attribute != null)
        CY = SvgCoordinate.Parse(cy_attribute.Value);

      var r_attribute = radialGradientElement.Attribute("r");
      if(r_attribute != null)
        R = SvgCoordinate.Parse(r_attribute.Value);

      var fx_attribute = radialGradientElement.Attribute("fx");
      if(fx_attribute != null)
        FX = SvgCoordinate.Parse(fx_attribute.Value);

      var fy_attribute = radialGradientElement.Attribute("fy");
      if(fy_attribute != null)
        FY = SvgCoordinate.Parse(fy_attribute.Value);
      
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
      var radial_gradient_brush = base.SetBrush(brush) as RadialGradientBrush;
      if(radial_gradient_brush != null)
      {
        var cx = CX.ToDouble();
        var cy = CY.ToDouble();
        var fx = FX?.ToDouble() ?? cx;
        var fy = FY?.ToDouble() ?? cy;

        radial_gradient_brush.GradientOrigin = new Point(fx, fy);
        radial_gradient_brush.RadiusX = R.ToDouble();
        radial_gradient_brush.RadiusY = R.ToDouble();
        radial_gradient_brush.Center = new Point(cx, cy);
      }
      return brush;
    }

  } // class SvgRadialGradientElement

}

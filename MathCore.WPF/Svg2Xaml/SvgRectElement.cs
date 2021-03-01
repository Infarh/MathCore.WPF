////////////////////////////////////////////////////////////////////////////////
//
//  SvgRectElement.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18580 $
//  $LastChangedDate: 2009-03-18 14:55:24 +0100 (Wed, 18 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;rect&gt; element.
  /// </summary>
  class SvgRectElement
    : SvgDrawableBaseElement
  {
    //==========================================================================
    public readonly SvgCoordinate X = new(0.0);
    public readonly SvgCoordinate Y = new(0.0);
    public readonly SvgLength Width = new(0.0);
    public readonly SvgLength Height = new(0.0);
    public readonly SvgLength CornerRadiusX = new(0.0);
    public readonly SvgLength CornerRadiusY = new(0.0);

    //==========================================================================
    public SvgRectElement(SvgDocument document, SvgBaseElement parent, XElement rectElement)
      : base(document, parent, rectElement)
    {
      var x_attribute = rectElement.Attribute("x");
      if(x_attribute != null)
        X = SvgCoordinate.Parse(x_attribute.Value);

      var y_attribute = rectElement.Attribute("y");
      if(y_attribute != null)
        Y = SvgCoordinate.Parse(y_attribute.Value);

      var width_attribute = rectElement.Attribute("width");
      if(width_attribute != null)
        Width = SvgLength.Parse(width_attribute.Value);

      var height_attribute = rectElement.Attribute("height");
      if(height_attribute != null)
        Height = SvgLength.Parse(height_attribute.Value);

      var rx_attribute = rectElement.Attribute("rx");
      if(rx_attribute != null)
        CornerRadiusX = SvgCoordinate.Parse(rx_attribute.Value);

      var ry_attribute = rectElement.Attribute("ry");
      if(ry_attribute != null)
        CornerRadiusY = SvgCoordinate.Parse(ry_attribute.Value);
    }

    //==========================================================================
    public override Geometry GetBaseGeometry() => new RectangleGeometry(new Rect(new Point(X.ToDouble(), Y.ToDouble()),
        new Size(Width.ToDouble(), Height.ToDouble())), 
        CornerRadiusX.ToDouble(),
        CornerRadiusY.ToDouble());
  } // class SvgRectElement

}

////////////////////////////////////////////////////////////////////////////////
//
//  SvgLineElement.cs - This file is part of Svg2Xaml.
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

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;line&gt; element.
  /// </summary>
  class SvgLineElement
    : SvgDrawableBaseElement
  {
    //==========================================================================
    public readonly SvgCoordinate X1 = new SvgCoordinate(0.0);
    public readonly SvgCoordinate Y1 = new SvgCoordinate(0.0);
    public readonly SvgCoordinate X2 = new SvgCoordinate(0.0);
    public readonly SvgCoordinate Y2 = new SvgCoordinate(0.0);
    
    //==========================================================================
    public SvgLineElement(SvgDocument document, SvgBaseElement parent, XElement lineElement)
      : base(document, parent, lineElement)
    {
      XAttribute x1_attribute = lineElement.Attribute("x1");
      if(x1_attribute != null)
        X1 = SvgCoordinate.Parse(x1_attribute.Value);

      XAttribute y1_attribute = lineElement.Attribute("y1");
      if(y1_attribute != null)
        Y1 = SvgCoordinate.Parse(y1_attribute.Value);

      XAttribute x2_attribute = lineElement.Attribute("x2");
      if(x2_attribute != null)
        X2 = SvgCoordinate.Parse(x2_attribute.Value);

      XAttribute y2_attribute = lineElement.Attribute("y2");
      if(y2_attribute != null)
        Y2 = SvgCoordinate.Parse(y2_attribute.Value);
    }

    //==========================================================================
    public override Geometry GetBaseGeometry() => new LineGeometry(new Point(X1.ToDouble(), Y1.ToDouble()),
        new Point(X2.ToDouble(), Y2.ToDouble()));
  } // class SvgLineElement

}

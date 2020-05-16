////////////////////////////////////////////////////////////////////////////////
//
//  SvgStopElement.cs - This file is part of Svg2Xaml.
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
using System;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  class SvgStopElement
    : SvgBaseElement
  {
    //==========================================================================
    public readonly SvgLength Offset = new SvgLength(0);
    public readonly SvgColor Color = new SvgColor(0,0,0);
    public readonly SvgLength Opacity = new SvgLength(1);

    //==========================================================================
    public SvgStopElement(SvgDocument document, SvgBaseElement parent, XElement stopElement)
      : base(document, parent, stopElement)
    {
      var offset_attribute = stopElement.Attribute("offset");
      if(offset_attribute != null)
        Offset = SvgLength.Parse(offset_attribute.Value);

      var stop_color_attribute = stopElement.Attribute("stop-color");
      if(stop_color_attribute != null)
        Color = SvgColor.Parse(stop_color_attribute.Value);

      var stop_opacity_attribute = stopElement.Attribute("stop-opacity");
      if(stop_opacity_attribute != null)
        Opacity = SvgLength.Parse(stop_opacity_attribute.Value);
    }

    //==========================================================================
    public GradientStop ToGradientStop()
    {
      var color = Color.ToColor();
      color.A = (byte)Math.Round(Opacity.ToDouble() * 255);

      var stop = new GradientStop();
      stop.Color = color;
      stop.Offset = Offset.ToDouble();

      return stop;
    }

  } // class SvgStopElement

}

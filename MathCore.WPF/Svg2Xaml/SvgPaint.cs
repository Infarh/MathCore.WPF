////////////////////////////////////////////////////////////////////////////////
//
//  SvgPaint.cs - This file is part of Svg2Xaml.
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
using System.Globalization;
using System.Windows.Media;

namespace MathCore.WPF.SVG
{

  //****************************************************************************
  abstract class SvgPaint
  {

    //==========================================================================
    public static SvgPaint Parse(string value)
    {
      if(value == null)
        throw new ArgumentNullException("value");
      
      value = value.Trim();
      if(value == "")
        throw new ArgumentException("value must not be empty", "value");

      if(value.StartsWith("url"))
      {
        string url = value.Substring(3).Trim();
        if(url.StartsWith("(") && url.EndsWith(")"))
        {
          url = url.Substring(1, url.Length - 2).Trim();
          if(url.StartsWith("#"))
            return new SvgUrlPaint(url.Substring(1).Trim());
        }
      }

      if(value.StartsWith("#"))
      {
        string color = value.Substring(1).Trim();
        if(color.Length == 3)
        {
          byte r = byte.Parse(string.Format("{0}{0}", color[0]), NumberStyles.HexNumber);
          byte g = byte.Parse(string.Format("{0}{0}", color[1]), NumberStyles.HexNumber);
          byte b = byte.Parse(string.Format("{0}{0}", color[2]), NumberStyles.HexNumber);
          return new SvgColorPaint(new SvgColor(r, g, b));
        }

        if(color.Length == 6)
        {
          byte r = byte.Parse(color.Substring(0,2), NumberStyles.HexNumber);
          byte g = byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
          byte b = byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
          return new SvgColorPaint(new SvgColor(r, g, b));
        }
      }

      if(value.StartsWith("rgb"))
      {
        string color = value.Substring(3).Trim();
        if(color.StartsWith("(") && color.EndsWith(")"))
        {
          color = color.Substring(1, color.Length - 2).Trim();

          string[] components = color.Split(',');
          if(components.Length == 3)
          {
            float r,g,b;

            components[0] = components[0].Trim();
            if(components[0].EndsWith("%"))
            {
              components[0] = components[0].Substring(0, components[0].Length - 1).Trim();
              r = float.Parse(components[0], CultureInfo.InvariantCulture.NumberFormat) / 100;
            }
            else
              r = (float)(byte.Parse(components[0]) / 255.0);

            components[1] = components[1].Trim();
            if(components[1].EndsWith("%"))
            {
              components[1] = components[1].Substring(0, components[1].Length - 1).Trim();
              g = float.Parse(components[1], CultureInfo.InvariantCulture.NumberFormat) / 100;
            }
            else
              g = (float)(byte.Parse(components[1]) / 255.0);

            components[2] = components[1].Trim();
            if(components[2].EndsWith("%"))
            {
              components[2] = components[2].Substring(0, components[2].Length - 1).Trim();
              b = float.Parse(components[2], CultureInfo.InvariantCulture.NumberFormat) / 100;
            }
            else
              b = (float)(byte.Parse(components[2]) / 255.0);

            return new SvgColorPaint(new SvgColor(r, g, b));
          }
        }
      }

      if(value == "none")
        return null;


      switch(value)
      {
        case "black":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0)));
        case "green":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0)));
        case "silver":
          return new SvgColorPaint(new SvgColor((float)(192 / 255.0), (float)(192 / 255.0), (float)(192 / 255.0)));
        case "lime":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0)));
        case "gray":
          return new SvgColorPaint(new SvgColor((float)(128 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0)));
        case "olive":
          return new SvgColorPaint(new SvgColor((float)(128 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0)));
        case "white":
          return new SvgColorPaint(new SvgColor((float)(255 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0)));
        case "yellow":
          return new SvgColorPaint(new SvgColor((float)(255 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0)));
        case "maroon":
          return new SvgColorPaint(new SvgColor((float)(128 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0)));
        case "navy":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0)));
        case "red":
          return new SvgColorPaint(new SvgColor((float)(255 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0)));
        case "blue":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0)));
        case "purple":
          return new SvgColorPaint(new SvgColor((float)(128 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0)));
        case "teal":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0)));
        case "fuchsia":
          return new SvgColorPaint(new SvgColor((float)(255 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0)));
        case "aqua":
          return new SvgColorPaint(new SvgColor((float)(0 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0)));
      }

      throw new ArgumentException($"Unsupported paint value: {value}");
    }

    //==========================================================================
    public abstract Brush ToBrush(SvgBaseElement element);

  } // class SvgPaint

}

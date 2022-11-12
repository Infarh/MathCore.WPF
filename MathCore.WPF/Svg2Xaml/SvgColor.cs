////////////////////////////////////////////////////////////////////////////////
//
//  SvgColor.cs - This file is part of Svg2Xaml.
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

using System.Globalization;
using System.Windows.Media;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents an RGB color.</summary>
class SvgColor
{

    //==========================================================================
    public readonly float Red;
    
    //==========================================================================
    public readonly float Green;
    
    //==========================================================================
    public readonly float Blue;

    //==========================================================================
    public SvgColor(float red, float green, float blue)
    {
        Red   = red;
        Green = green;
        Blue  = blue;
    }

    //==========================================================================
    public SvgColor(byte red, byte green, byte blue)
    {
        Red   = red / 255.0f;
        Green = green / 255.0f;
        Blue  = blue / 255.0f;
    }

    //==========================================================================
    public Color ToColor() => Color.FromScRgb(1, Red, Green, Blue);

    //==========================================================================
    public static SvgColor Parse(string value)
    {
        if(value.StartsWith("#"))
        {
            var color = value[1..].Trim();
            if(color.Length == 3)
            {
                var r = (float)(byte.Parse(string.Format("{0}{0}", color[0]), NumberStyles.HexNumber) / 255.0);
                var g = (float)(byte.Parse(string.Format("{0}{0}", color[1]), NumberStyles.HexNumber) / 255.0);
                var b = (float)(byte.Parse(string.Format("{0}{0}", color[2]), NumberStyles.HexNumber) / 255.0);
                return new SvgColor(r, g, b);
            }

            if(color.Length == 6)
            {
                var r = (float)(byte.Parse(color[..2], NumberStyles.HexNumber) / 255.0);
                var g = (float)(byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255.0);
                var b = (float)(byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255.0);
                return new SvgColor(r, g, b);
            }
        }

        if(value.StartsWith("rgb"))
        {
            var color = value[3..].Trim();
            if(color.StartsWith("(") && color.EndsWith(")"))
            {
                color = color[1..^1].Trim();

                var components = color.Split(',');
                if(components.Length == 3)
                {
                    float r, g, b;

                    components[0] = components[0].Trim();
                    if(components[0].EndsWith("%"))
                    {
                        components[0] = components[0][..^1].Trim();
                        r             = (float)(double.Parse(components[0], CultureInfo.InvariantCulture.NumberFormat) / 100.0);
                    }
                    else
                        r = (float)(byte.Parse(components[0]) / 255.0);

                    components[1] = components[1].Trim();
                    if(components[1].EndsWith("%"))
                    {
                        components[1] = components[1][..^1].Trim();
                        g             = (float)(double.Parse(components[1], CultureInfo.InvariantCulture.NumberFormat) / 100.0);
                    }
                    else
                        g = (float)(byte.Parse(components[1]) / 255.0);

                    components[2] = components[1].Trim();
                    if(components[2].EndsWith("%"))
                    {
                        components[2] = components[2][..^1].Trim();
                        b             = (float)(double.Parse(components[2], CultureInfo.InvariantCulture.NumberFormat) / 100.0);
                    }
                    else
                        b = (float)(byte.Parse(components[2]) / 255.0);

                    return new SvgColor(r, g, b);
                }
            }
        }

        if(value == "none")
            return null;


        switch(value)
        {
            case "black":
                return new SvgColor((float)(0 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0));
            case "green":
                return new SvgColor((float)(0 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0));
            case "silver":
                return new SvgColor((float)(192 / 255.0), (float)(192 / 255.0), (float)(192 / 255.0));
            case "lime":
                return new SvgColor((float)(0 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0));
            case "gray":
                return new SvgColor((float)(128 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0));
            case "olive":
                return new SvgColor((float)(128 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0));
            case "white":
                return new SvgColor((float)(255 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0));
            case "yellow":
                return new SvgColor((float)(255 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0));
            case "maroon":
                return new SvgColor((float)(128 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0));
            case "navy":
                return new SvgColor((float)(0 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0));
            case "red":
                return new SvgColor((float)(255 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0));
            case "blue":
                return new SvgColor((float)(0 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0));
            case "purple":
                return new SvgColor((float)(128 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0));
            case "teal":
                return new SvgColor((float)(0 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0));
            case "fuchsia":
                return new SvgColor((float)(255 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0));
            case "aqua":
                return new SvgColor((float)(0 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0));
        }

        throw new ArgumentException($"Unsupported color value: {value}");

    }

} // class SvgColor
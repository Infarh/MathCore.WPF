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
    public static SvgColor? Parse(string value)
    {
        if(value.StartsWith("#"))
        {
            var color = value[1..].Trim();

            switch (color)
            {
                case [ var sr, var sg, var sb ]:
                {
                    var r = (float)(byte.Parse(string.Format("{0}{0}", sr), NumberStyles.HexNumber) / 255.0);
                    var g = (float)(byte.Parse(string.Format("{0}{0}", sg), NumberStyles.HexNumber) / 255.0);
                    var b = (float)(byte.Parse(string.Format("{0}{0}", sb), NumberStyles.HexNumber) / 255.0);
                    return new SvgColor(r, g, b);
                }

                case { Length: 6 }:
                {
                    var r = (float)(byte.Parse(color[..2], NumberStyles.HexNumber) / 255.0);
                    var g = (float)(byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255.0);
                    var b = (float)(byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255.0);
                    return new SvgColor(r, g, b);
                }
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

        return value switch
        {
            "black"   => new((float)(0 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0)),
            "green"   => new((float)(0 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0)),
            "silver"  => new((float)(192 / 255.0), (float)(192 / 255.0), (float)(192 / 255.0)),
            "lime"    => new((float)(0 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0)),
            "gray"    => new((float)(128 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0)),
            "olive"   => new((float)(128 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0)),
            "white"   => new((float)(255 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0)),
            "yellow"  => new((float)(255 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0)),
            "maroon"  => new((float)(128 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0)),
            "navy"    => new((float)(0 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0)),
            "red"     => new((float)(255 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0)),
            "blue"    => new((float)(0 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0)),
            "purple"  => new((float)(128 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0)),
            "teal"    => new((float)(0 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0)),
            "fuchsia" => new((float)(255 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0)),
            "aqua"    => new((float)(0 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0)),
            _         => throw new ArgumentException($"Unsupported color value: {value}")
        };
    }

} // class SvgColor
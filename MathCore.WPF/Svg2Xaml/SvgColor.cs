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

/// <summary>  Represents an RGB color.</summary>
internal class SvgColor(float red, float green, float blue)
{

    public readonly float Red = red;
    
    public readonly float Green = green;
    
    public readonly float Blue = blue;

    public SvgColor(byte red, byte green, byte blue) : this(red / 255f, green / 255f, blue / 255f)
    {
    }

    public Color ToColor() => Color.FromScRgb(1, Red, Green, Blue);

    public static SvgColor? Parse(string value)
    {
        if(value.StartsWith("#"))
        {
            var color = value[1..].Trim();

            switch (color)
            {
                case [ var sr, var sg, var sb ]:
                {
                    var r = byte.Parse(string.Format("{0}{0}", sr), NumberStyles.HexNumber) / 255f;
                    var g = byte.Parse(string.Format("{0}{0}", sg), NumberStyles.HexNumber) / 255f;
                    var b = byte.Parse(string.Format("{0}{0}", sb), NumberStyles.HexNumber) / 255f;
                    return new(r, g, b);
                }

                case { Length: 6 }:
                {
                    var r = byte.Parse(color[..2], NumberStyles.HexNumber) / 255f;
                    var g = byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber) / 255f;
                    var b = byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber) / 255f;
                    return new(r, g, b);
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
                        r             = (float)(double.Parse(components[0], CultureInfo.InvariantCulture.NumberFormat) / 100);
                    }
                    else
                        r = byte.Parse(components[0]) / 255f;

                    components[1] = components[1].Trim();
                    if(components[1].EndsWith("%"))
                    {
                        components[1] = components[1][..^1].Trim();
                        g             = (float)(double.Parse(components[1], CultureInfo.InvariantCulture.NumberFormat) / 100);
                    }
                    else
                        g = byte.Parse(components[1]) / 255f;

                    components[2] = components[1].Trim();
                    if(components[2].EndsWith("%"))
                    {
                        components[2] = components[2][..^1].Trim();
                        b             = (float)(double.Parse(components[2], CultureInfo.InvariantCulture.NumberFormat) / 100);
                    }
                    else
                        b = byte.Parse(components[2]) / 255f;

                    return new(r, g, b);
                }
            }
        }

        if(value == "none")
            return null;

        const float f000_255 = 0 / 255f;
        const float f128_255 = 128 / 255f;
        const float f192_255 = 192 / 255f;
        const float f255_255 = 255 / 255f;
        return value switch
        {
            "black"   => new(f000_255, f000_255, f000_255),
            "green"   => new(f000_255, f128_255, f000_255),
            "silver"  => new(f192_255, f192_255, f192_255),
            "lime"    => new(f000_255, f255_255, f000_255),
            "gray"    => new(f128_255, f128_255, f128_255),
            "olive"   => new(f128_255, f128_255, f000_255),
            "white"   => new(f255_255, f255_255, f255_255),
            "yellow"  => new(f255_255, f255_255, f000_255),
            "maroon"  => new(f128_255, f000_255, f000_255),
            "navy"    => new(f000_255, f000_255, f128_255),
            "red"     => new(f255_255, f000_255, f000_255),
            "blue"    => new(f000_255, f000_255, f255_255),
            "purple"  => new(f128_255, f000_255, f128_255),
            "teal"    => new(f000_255, f128_255, f128_255),
            "fuchsia" => new(f255_255, f000_255, f255_255),
            "aqua"    => new(f000_255, f255_255, f255_255),
            _         => throw new ArgumentException($"Unsupported color value: {value}")
        };
    }
}
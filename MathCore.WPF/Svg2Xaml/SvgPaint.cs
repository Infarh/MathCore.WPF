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

using System.Globalization;
using System.Windows.Media;

namespace MathCore.WPF.SVG;

//****************************************************************************
abstract class SvgPaint
{

    //==========================================================================
    public static SvgPaint? Parse(string value)
    {
        if(value is null)
            throw new ArgumentNullException(nameof(value));
      
        value = value.Trim();
        if(value == "")
            throw new ArgumentException("value must not be empty", nameof(value));

        if(value.StartsWith("url"))
        {
            var url = value[3..].Trim();
            if(url.StartsWith("(") && url.EndsWith(")"))
            {
                url = url[1..^1].Trim();
                if(url.StartsWith("#"))
                    return new SvgUrlPaint(url[1..].Trim());
            }
        }

        if(value.StartsWith("#"))
        {
            var color = value[1..].Trim();
            switch (color)
            {
                case [var sr, var sg, var sb]:
                {
                    var r = byte.Parse(string.Format("{0}{0}", sr), NumberStyles.HexNumber);
                    var g = byte.Parse(string.Format("{0}{0}", sg), NumberStyles.HexNumber);
                    var b = byte.Parse(string.Format("{0}{0}", sb), NumberStyles.HexNumber);
                    return new SvgColorPaint(new SvgColor(r, g, b));
                }
                case  { Length: 6 }:
                {
                    var r = byte.Parse(color[..2], NumberStyles.HexNumber);
                    var g = byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                    var b = byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                    return new SvgColorPaint(new SvgColor(r, g, b));
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
                        r             = float.Parse(components[0], CultureInfo.InvariantCulture.NumberFormat) / 100;
                    }
                    else
                        r = (float)(byte.Parse(components[0]) / 255.0);

                    components[1] = components[1].Trim();
                    if(components[1].EndsWith("%"))
                    {
                        components[1] = components[1][..^1].Trim();
                        g             = float.Parse(components[1], CultureInfo.InvariantCulture.NumberFormat) / 100;
                    }
                    else
                        g = (float)(byte.Parse(components[1]) / 255.0);

                    components[2] = components[1].Trim();
                    if(components[2].EndsWith("%"))
                    {
                        components[2] = components[2][..^1].Trim();
                        b             = float.Parse(components[2], CultureInfo.InvariantCulture.NumberFormat) / 100;
                    }
                    else
                        b = (float)(byte.Parse(components[2]) / 255.0);

                    return new SvgColorPaint(new SvgColor(r, g, b));
                }
            }
        }

        if(value == "none")
            return null;

        return value switch
        {
            "black"   => new SvgColorPaint(new((float)(0 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0))),
            "green"   => new SvgColorPaint(new((float)(0 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0))),
            "silver"  => new SvgColorPaint(new((float)(192 / 255.0), (float)(192 / 255.0), (float)(192 / 255.0))),
            "lime"    => new SvgColorPaint(new((float)(0 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0))),
            "gray"    => new SvgColorPaint(new((float)(128 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0))),
            "olive"   => new SvgColorPaint(new((float)(128 / 255.0), (float)(128 / 255.0), (float)(0 / 255.0))),
            "white"   => new SvgColorPaint(new((float)(255 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0))),
            "yellow"  => new SvgColorPaint(new((float)(255 / 255.0), (float)(255 / 255.0), (float)(0 / 255.0))),
            "maroon"  => new SvgColorPaint(new((float)(128 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0))),
            "navy"    => new SvgColorPaint(new((float)(0 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0))),
            "red"     => new SvgColorPaint(new((float)(255 / 255.0), (float)(0 / 255.0), (float)(0 / 255.0))),
            "blue"    => new SvgColorPaint(new((float)(0 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0))),
            "purple"  => new SvgColorPaint(new((float)(128 / 255.0), (float)(0 / 255.0), (float)(128 / 255.0))),
            "teal"    => new SvgColorPaint(new((float)(0 / 255.0), (float)(128 / 255.0), (float)(128 / 255.0))),
            "fuchsia" => new SvgColorPaint(new((float)(255 / 255.0), (float)(0 / 255.0), (float)(255 / 255.0))),
            "aqua"    => new SvgColorPaint(new((float)(0 / 255.0), (float)(255 / 255.0), (float)(255 / 255.0))),
            _         => throw new ArgumentException($"Unsupported paint value: {value}")
        };
    }

    //==========================================================================
    public abstract Brush? ToBrush(SvgBaseElement element);

} // class SvgPaint
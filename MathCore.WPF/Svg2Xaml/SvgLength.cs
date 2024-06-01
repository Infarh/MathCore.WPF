////////////////////////////////////////////////////////////////////////////////
//
//  SvgLength.cs - This file is part of Svg2Xaml.
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

namespace MathCore.WPF.SVG;

internal class SvgLength(double value, string? unit = null)
{

    public readonly double Value = value;

    public readonly string Unit = unit;

    public static SvgLength Parse(string value)
    {
        value = value.NotNull().Trim();
        if(value == "")
            throw new ArgumentException("value must not be empty", nameof(value));

        if(value == "inherit")
            return new(double.NaN);

        string unit = null;

        foreach(var unit_identifier in new[] {"in", "cm", "mm", "pt", "pc", "px", "%" })
            if(value.EndsWith(unit_identifier))
            {
                unit  = unit_identifier;
                value = value[..^unit_identifier.Length].Trim();
                break;
            }

        return new(double.Parse(value, CultureInfo.InvariantCulture.NumberFormat), unit);
    }

    public double ToDouble() => Value;
}
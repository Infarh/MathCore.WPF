////////////////////////////////////////////////////////////////////////////////
//
//  SvgMatrixTransform.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 25245 $
//  $LastChangedDate: 2009-06-19 13:33:47 +0200 (Fri, 19 Jun 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////

using System.Globalization;
using System.Windows.Media;

namespace MathCore.WPF.SVG;

//****************************************************************************
internal class SvgMatrixTransform(double m11, double m12, double m21, double m22, double OffsetX, double OffsetY)
    : SvgTransform
{
    //==========================================================================
    public readonly double M11 = m11;
    public readonly double M12 = m12;
    public readonly double M21 = m21;
    public readonly double M22 = m22;
    public readonly double OffsetX = OffsetX;
    public readonly double OffsetY = OffsetY;

    //==========================================================================

    //==========================================================================
    public override Transform ToTransform() => new MatrixTransform(M11, M12, M21, M22, OffsetX, OffsetY);

    //==========================================================================
    public static new SvgMatrixTransform Parse(string transform)
    {
        var tokens = transform.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if(tokens.Length == 6)
            return new(
                double.Parse(tokens[0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(tokens[1].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(tokens[2].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                double.Parse(tokens[3].Trim(), CultureInfo.InvariantCulture.NumberFormat), 
                double.Parse(tokens[4].Trim(), CultureInfo.InvariantCulture.NumberFormat), 
                double.Parse(tokens[5].Trim(), CultureInfo.InvariantCulture.NumberFormat));

        throw new ArgumentException();
    }

} // class SvgMatrixTransform
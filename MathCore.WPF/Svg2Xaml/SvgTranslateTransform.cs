////////////////////////////////////////////////////////////////////////////////
//
//  SvgTranslateTransform.cs - This file is part of Svg2Xaml.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Media;

namespace MathCore.WPF.SVG
{

  //****************************************************************************
  class SvgTranslateTransform
    : SvgTransform
  {
    public readonly double X;
    public readonly double Y;
    
    //==========================================================================
    public SvgTranslateTransform(double x, double y)
    {
      X = x;
      Y = y;
    }

    //==========================================================================
    public override Transform ToTransform() => new TranslateTransform(X, Y);

      //==========================================================================
    public static new SvgTranslateTransform Parse(string transform)
    {
      string[] tokens = transform.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
      if(tokens.Length != 2)
        throw new FormatException("A translate transformation must have two values");

      return new SvgTranslateTransform(double.Parse(tokens[0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                       double.Parse(tokens[1].Trim(), CultureInfo.InvariantCulture.NumberFormat));
    }

  } // class SvgTranslateTransform

}

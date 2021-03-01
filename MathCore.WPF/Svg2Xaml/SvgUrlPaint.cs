////////////////////////////////////////////////////////////////////////////////
//
//  SvgUrlPaint.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media;

namespace MathCore.WPF.SVG
{

  //****************************************************************************
  class SvgUrlPaint
    : SvgPaint
  {
    public readonly string Url;
    
    //==========================================================================
    public SvgUrlPaint(string url) => Url = url;

    //==========================================================================
    public override Brush ToBrush(SvgBaseElement element)
    {
      if (!element.Document.Elements.ContainsKey(Url))
        return null;

      var reference = element.Document.Elements[Url];
      if (reference is SvgGradientBaseElement)
        return (reference as SvgGradientBaseElement).ToBrush();
      else if (reference is SvgPatternElement)
        return (reference as SvgPatternElement).ToBrush();

      throw new NotImplementedException();
    }

  } // class SvgUrlPaint

}

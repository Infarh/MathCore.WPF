////////////////////////////////////////////////////////////////////////////////
//
//  SvgURL.cs - This file is part of Svg2Xaml.
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

namespace MathCore.WPF.SVG
{

  //****************************************************************************
  class SvgURL
  {

    //==========================================================================
    public readonly string Id;
    
    //==========================================================================
    public SvgURL(string id)
    {
      Id = id;
    }

    //==========================================================================
    public static SvgURL Parse(string value)
    {
      if(value == null)
        throw new ArgumentNullException("value");

      value = value.Trim();

      if(value == "none")
        return null;

      if(value == "")
        throw new ArgumentException("value must not be empty.");

      if(value.StartsWith("url"))
      {
        value = value.Substring(3).Trim();
        if(value.StartsWith("(") && value.EndsWith(")"))
        {
          value = value.Substring(1, value.Length - 2).Trim();
          if(value.StartsWith("#"))
            return new SvgURL(value.Substring(1));
        }
      }

      throw new ArgumentException($"Unsupported URL value: {value}");
    }


  } // class SvgURL

}

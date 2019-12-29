////////////////////////////////////////////////////////////////////////////////
//
//  SvgTransform.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18379 $
//  $LastChangedDate: 2009-03-17 17:43:58 +0100 (Tue, 17 Mar 2009) $
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
  abstract class SvgTransform
  {

    //==========================================================================
    public static SvgTransform Parse(string value)
    {
      if(value == null)
        throw new ArgumentNullException("value");
      
      value = value.Trim();
      if(value == "")
        throw new ArgumentException("value must not be empty", "value");

      List<SvgTransform> transforms = new List<SvgTransform>();

      string transform = value;
      while(transform.Length > 0)
      {

        if(transform.StartsWith("translate"))
        {
          transform = transform.Substring(9).TrimStart();
          if(transform.StartsWith("("))
          {
            transform = transform.Substring(1);
            int index = transform.IndexOf(")");
            if(index >= 0)
            {
              transforms.Add(SvgTranslateTransform.Parse(transform.Substring(0, index).Trim()));
              transform = transform.Substring(index + 1).TrimStart();
              continue;
            }
          }
        }
          
        if(transform.StartsWith("matrix"))
        {
          transform = transform.Substring(6).TrimStart();
          if(transform.StartsWith("("))
          {
            transform = transform.Substring(1);
            int index = transform.IndexOf(")");
            if(index >= 0)
            {
              transforms.Add(SvgMatrixTransform.Parse(transform.Substring(0, index).Trim()));
              transform = transform.Substring(index + 1).TrimStart();
              continue;
            }
          }
        }

        if(transform.StartsWith("scale"))
        {
          transform = transform.Substring(5).TrimStart();
          if(transform.StartsWith("("))
          {
            transform = transform.Substring(1);
            int index = transform.IndexOf(")");
            if(index >= 0)
            {
              transforms.Add(SvgScaleTransform.Parse(transform.Substring(0, index).Trim()));
              transform = transform.Substring(index + 1).TrimStart();
              continue;
            }
          }
        }

        if(transform.StartsWith("skew"))
        {
          transform = transform.Substring(5).TrimStart();
          if(transform.StartsWith("("))
          {
            transform = transform.Substring(1);
            int index = transform.IndexOf(")");
            if(index >= 0)
            {
              transforms.Add(SvgSkewTransform.Parse(transform.Substring(0, index).Trim()));
              transform = transform.Substring(index + 1).TrimStart();
              continue;
            }
          }
        }

        if(transform.StartsWith("rotate"))
        {
          transform = transform.Substring(6).TrimStart();
          if(transform.StartsWith("("))
          {
            transform = transform.Substring(1);
            int index = transform.IndexOf(")");
            if(index >= 0)
            {
              transforms.Add(SvgScaleTransform.Parse(transform.Substring(0, index).Trim()));
              transform = transform.Substring(index + 1).TrimStart();
              continue;
            }
          }
        }

      }

      if(transforms.Count == 1)
        return transforms[0];
      else if(transforms.Count > 1)
        return new SvgTransformGroup(transforms.ToArray());

      throw new ArgumentException($"Unsupported transform value: {value}");
    }

    //==========================================================================
    public abstract Transform ToTransform();

  } // class Transform

}

////////////////////////////////////////////////////////////////////////////////
//
//  SvgPathElement.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;path&gt; element.
  /// </summary>
  class SvgPathElement
    : SvgDrawableBaseElement
  {
    //==========================================================================
    public readonly string Data;
    
    //==========================================================================
    public SvgPathElement(SvgDocument document, SvgBaseElement parent, XElement pathElement)
      : base(document, parent, pathElement)
    {
      XAttribute d_attribute = pathElement.Attribute("d");
      if(d_attribute != null)
        Data = d_attribute.Value;
      else
        Data = null;
    }

    //==========================================================================
    public override Geometry GetBaseGeometry() => Geometry.Parse(Data).Clone();
  } // class SvgPathElement

}

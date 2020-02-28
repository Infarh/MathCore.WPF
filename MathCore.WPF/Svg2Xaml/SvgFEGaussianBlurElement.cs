////////////////////////////////////////////////////////////////////////////////
//
//  SvgFEGaussianBlurElement.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents an &lt;feGaussianBlur&gt; element.
  /// </summary>
  class SvgFEGaussianBlurElement
    : SvgFilterEffectBaseElement
  {

    //==========================================================================
    public readonly SvgLength StdDeviation = new SvgLength(1.0);

    //==========================================================================
    public SvgFEGaussianBlurElement(SvgDocument document, SvgBaseElement parent, XElement feGaussianBlurElement)
      : base(document, parent, feGaussianBlurElement)
    {
      var std_deviation_attribute = feGaussianBlurElement.Attribute("stdDeviation");
      if(std_deviation_attribute != null)
        StdDeviation = SvgCoordinate.Parse(std_deviation_attribute.Value);

    }

    //==========================================================================
    public override BitmapEffect ToBitmapEffect()
    {
      var blur_bitmap_effect = new BlurBitmapEffect();
      blur_bitmap_effect.Radius = StdDeviation.ToDouble();
      blur_bitmap_effect.KernelType = KernelType.Box;
      return blur_bitmap_effect;
    }

  } // class SvgFEGaussianBlurElement

}

////////////////////////////////////////////////////////////////////////////////
//
//  SvgFilterElement.cs - This file is part of Svg2Xaml.
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

using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents a &lt;filter&gt; element.</summary>
internal class SvgFilterElement
    : SvgBaseElement
{
    //==========================================================================
    public readonly List<SvgFilterEffectBaseElement> FilterEffects = [];

    //==========================================================================
    public SvgFilterElement(SvgDocument document, SvgBaseElement parent, XElement FilterElement)
        : base(document, parent, FilterElement)
    {
        foreach(var element in from element in FilterElement.Elements()
                               where element.Name.NamespaceName == "http://www.w3.org/2000/svg"
                               select element)
            switch(element.Name.LocalName)
            {

                case "feGaussianBlur":
                    FilterEffects.Add(new SvgFeGaussianBlurElement(document, this, element));
                    break;

                case "feBlend":
                    FilterEffects.Add(new SvgFeBlendElement(document, this, element));
                    break;

                case "feColorMatrix":
                    FilterEffects.Add(new SvgFeColorMatrixElement(document, this, element));
                    break;

                default:
                    throw new NotImplementedException($"Unhandled element: {element}");
            }

    }

    //==========================================================================
    public BitmapEffect? ToBitmapEffect()
    {
        if(Document.Options.IgnoreEffects)
            return null;

        var bitmap_effect_group = new BitmapEffectGroup();

        foreach(var filter_effect in FilterEffects)
            if(filter_effect.ToBitmapEffect() is { } bitmap_effect)
                bitmap_effect_group.Children.Add(bitmap_effect);

        return bitmap_effect_group.Children.Count == 0 ? null : bitmap_effect_group;
    }

} // class SvgFilterElement
////////////////////////////////////////////////////////////////////////////////
//
//  SvgClipPathElement.cs - This file is part of Svg2Xaml.
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

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents a &lt;clipPath&gt; element.</summary>
internal class SvgClipPathElement(SvgDocument document, SvgBaseElement parent, XElement ClipPathElement)
    : SvgContainerBaseElement(document, parent, ClipPathElement)
{
    public Geometry GetClipGeometry()
    {
        var geometry_group = new GeometryGroup();

        foreach(var child_element in Children)
        {
            var element = child_element;
            if(element is SvgUseElement use_element)
                element = use_element.GetElement();


            switch (element)
            {
                case SvgDrawableBaseElement base_element:
                {
                    var geometry = base_element.GetGeometry();
                    if(geometry != null)
                        geometry_group.Children.Add(geometry);
                    break;
                }
                case SvgDrawableContainerBaseElement container_base_element:
                {
                    var geometry = container_base_element.GetGeometry();
                    if(geometry != null)
                        geometry_group.Children.Add(geometry);
                    break;
                }
            }
        }

        return geometry_group;
    }

}
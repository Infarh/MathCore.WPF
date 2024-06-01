////////////////////////////////////////////////////////////////////////////////
//
//  SvgFlowRegionElement.cs - This file is part of Svg2Xaml.
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

/// <summary>  Represents a &lt;flowRegíon&gt; element.</summary>
internal class SvgFlowRegionElement(SvgDocument document, SvgBaseElement parent, XElement FlowRegionElement)
    : SvgDrawableContainerBaseElement(document, parent, FlowRegionElement)
{
    public Geometry GetClipGeometry()
    {
        var geometry_group = new GeometryGroup();

        foreach(var element in Children)
            if(element is SvgDrawableBaseElement base_element)
            {
                var geometry = base_element.GetBaseGeometry();
                if(geometry != null)
                    geometry_group.Children.Add(geometry);
            }
            else
                throw new NotImplementedException();

        return geometry_group;
    }

}
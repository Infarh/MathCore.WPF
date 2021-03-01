////////////////////////////////////////////////////////////////////////////////
//
//  SvgFlowRootElement.cs - This file is part of Svg2Xaml.
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
using System;
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG
{

    //****************************************************************************
    /// <summary>
    ///   Represents a &lt;flowRoot&gt; element.
    /// </summary>
    class SvgFlowRootElement
      : SvgDrawableContainerBaseElement
    {
        //==========================================================================
        public readonly SvgFlowRegionElement? FlowRegion;

        //==========================================================================
        public SvgFlowRootElement(SvgDocument document, SvgBaseElement parent, XElement FlowRootElement)
          : base(document, parent, FlowRootElement)
        {
            for (var i = 0; i < Children.Count; ++i)
            {
                FlowRegion = Children[i] as SvgFlowRegionElement;
                if (FlowRegion is null) continue;
                Children.RemoveAt(i);
                break;
            }

            if (FlowRegion is null)
                throw new NotImplementedException();
        }

        //==========================================================================
        public override Drawing Draw()
        {
            var drawing_group = new DrawingGroup { Opacity = Opacity.ToDouble() };

            if (Transform != null)
                drawing_group.Transform = Transform.ToTransform();

            foreach (var element in Children)
            {
                var drawing = element switch
                {
                    SvgDrawableBaseElement base_element => base_element.Draw(),
                    SvgDrawableContainerBaseElement base_element => base_element.Draw(),
                    _ => null
                };

                if (drawing != null)
                    drawing_group.Children.Add(drawing);
            }

            if (Filter != null)
                if (Document.Elements[Filter.Id] is SvgFilterElement filter_element)
                    drawing_group.BitmapEffect = filter_element.ToBitmapEffect();

            if (ClipPath != null)
                if (Document.Elements[ClipPath.Id] is SvgClipPathElement clip_path_element)
                    drawing_group.ClipGeometry = clip_path_element.GetClipGeometry();

            if (Mask != null)
                if (Document.Elements[Mask.Id] is SvgMaskElement mask_element)
                {
                    drawing_group.OpacityMask = mask_element.GetOpacityMask();

                    var geometry_group = new GeometryGroup();

                    if (drawing_group.ClipGeometry != null)
                        geometry_group.Children.Add(drawing_group.ClipGeometry);

                    geometry_group.Children.Add(mask_element.GetClipGeometry());
                    drawing_group.ClipGeometry = geometry_group;

                }

            return drawing_group;
        }
    } // class SvgFlowRootElement
}

////////////////////////////////////////////////////////////////////////////////
//
//  SvgDrawableContainerBaseElement.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18622 $
//  $LastChangedDate: 2009-03-18 17:37:38 +0100 (Wed, 18 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////
using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
internal class SvgDrawableContainerBaseElement
    : SvgContainerBaseElement
{

    //==========================================================================
    public readonly SvgLength Opacity = new(1.0);
    public readonly SvgTransform? Transform;
    public readonly SvgUrl? ClipPath;
    public readonly SvgUrl? Filter;
    public readonly SvgUrl? Mask;
    public readonly SvgDisplay Display = SvgDisplay.Inline;

    //==========================================================================
    public SvgDrawableContainerBaseElement(SvgDocument document, SvgBaseElement parent, XElement DrawableContainerElement)
        : base(document, parent, DrawableContainerElement)
    {
        var opacity_attribute = DrawableContainerElement.Attribute("opacity");
        if (opacity_attribute != null)
            Opacity = SvgLength.Parse(opacity_attribute.Value);

        var transform_attribute = DrawableContainerElement.Attribute("transform");
        if (transform_attribute != null)
            Transform = SvgTransform.Parse(transform_attribute.Value);

        var clip_attribute = DrawableContainerElement.Attribute("clip-path");
        if (clip_attribute != null)
            ClipPath = SvgUrl.Parse(clip_attribute.Value);

        var filter_attribute = DrawableContainerElement.Attribute("filter");
        if (filter_attribute != null)
            Filter = SvgUrl.Parse(filter_attribute.Value);

        var mask_attribute = DrawableContainerElement.Attribute("mask");
        if (mask_attribute != null)
            Mask = SvgUrl.Parse(mask_attribute.Value);

        var display_attribute = DrawableContainerElement.Attribute("display");
        if (display_attribute != null)
            Display = display_attribute.Value switch
            {
                "inline"             => SvgDisplay.Inline,
                "block"              => SvgDisplay.Block,
                "list-item"          => SvgDisplay.ListItem,
                "run-in"             => SvgDisplay.RunIn,
                "compact"            => SvgDisplay.Compact,
                "marker"             => SvgDisplay.Marker,
                "table"              => SvgDisplay.Table,
                "inline-table"       => SvgDisplay.InlineTable,
                "table-row-group"    => SvgDisplay.TableRowGroup,
                "table-header-group" => SvgDisplay.TableHeaderGroup,
                "table-footer-group" => SvgDisplay.TableFooterGroup,
                "table-row"          => SvgDisplay.TableRow,
                "table-column-group" => SvgDisplay.TableColumnGroup,
                "table-column"       => SvgDisplay.TableColumn,
                "table-cell"         => SvgDisplay.TableCell,
                "table-caption"      => SvgDisplay.TableCaption,
                "none"               => SvgDisplay.None,
                _                    => throw new NotImplementedException(),
            };
    }

    //==========================================================================
    public virtual Geometry? GetGeometry()
    {
        var geometry_group = new GeometryGroup();

        foreach (var element in Children)
            switch (element)
            {
                case SvgDrawableBaseElement base_element: geometry_group.Children.Add(base_element.GetGeometry());
                    break;
                case SvgDrawableContainerBaseElement base_element: geometry_group.Children.Add(base_element.GetGeometry());
                    break;
            }

        if (Transform is not null)
            geometry_group.Transform = Transform.ToTransform();

        if (ClipPath is null) return geometry_group;
        if (Document.Elements[ClipPath.Id] is SvgClipPathElement clip_path_element)
            return Geometry.Combine(geometry_group, clip_path_element.GetClipGeometry(), GeometryCombineMode.Exclude, null);

        return geometry_group;
    }

    //==========================================================================
    public virtual Drawing Draw()
    {
        var drawing_group = new DrawingGroup { Opacity = Opacity.ToDouble() };

        if (Transform != null)
            drawing_group.Transform = Transform.ToTransform();

        foreach (var child_element in Children)
        {
            var element = child_element;
            if (element is SvgUseElement use_element)
                element = use_element.GetElement();

            Drawing? drawing = null;

            switch (element)
            {
                case SvgDrawableBaseElement drawable_base_element:
                {
                    if (drawable_base_element.Display != SvgDisplay.None)
                        drawing = drawable_base_element.Draw();
                    break;
                }
                case SvgDrawableContainerBaseElement drawable_container_base_element:
                {
                    if (drawable_container_base_element.Display != SvgDisplay.None)
                        drawing = drawable_container_base_element.Draw();
                    break;
                }
            }

            if (drawing != null)
                drawing_group.Children.Add(drawing);
        }

        if (Filter != null)
            if (Document.Elements[Filter.Id] is SvgFilterElement filter_element)
                drawing_group.BitmapEffect = filter_element.ToBitmapEffect();

        if (ClipPath != null)
            if (Document.Elements[ClipPath.Id] is SvgClipPathElement clip_path_element)
                drawing_group.ClipGeometry = clip_path_element.GetClipGeometry();

        if (Mask == null) return drawing_group;

        if (Document.Elements[Mask.Id] is not SvgMaskElement mask_element) 
            return drawing_group;

        var opacity_mask = mask_element.GetOpacityMask();
        /*
                if(Transform != null)
                  opacity_mask.Transform = Transform.ToTransform();
                */
        drawing_group.OpacityMask = opacity_mask;

        return drawing_group;
    }

} // class SvgDrawableContainerBaseElement
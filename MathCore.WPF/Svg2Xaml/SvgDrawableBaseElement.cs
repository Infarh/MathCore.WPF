////////////////////////////////////////////////////////////////////////////////
//
//  SvgDrawableBaseElement.cs - This file is part of Svg2Xaml.
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml.Linq;

// ReSharper disable once CheckNamespace
namespace MathCore.WPF.SVG
{

    //****************************************************************************
    abstract class SvgDrawableBaseElement : SvgBaseElement
    {

        //==========================================================================
        public readonly SvgLength Opacity = new SvgLength(1.0);
        public readonly SvgLength FillOpacity = new SvgLength(1.0);
        public readonly SvgLength StrokeOpacity = new SvgLength(1.0);
        public readonly SvgTransform Transform;
        public readonly SvgPaint Fill = new SvgColorPaint(new SvgColor(0, 0, 0));
        public readonly SvgPaint Stroke; /* new SvgColorPaint(new SvgColor(0, 0, 0)); */
        public readonly SvgLength StrokeWidth = new SvgLength(1);
        public readonly SvgStrokeLinecap StrokeLinecap = SvgStrokeLinecap.Butt;
        public readonly SvgStrokeLinejoin StrokeLinejoin = SvgStrokeLinejoin.Miter;
        public readonly double StrokeMiterlimit = 4;     // Double.None = inherit
        public readonly SvgLength StrokeDashoffset = new SvgLength(0);
        public readonly SvgLength?[] StrokeDasharray; // null = none, Length[0] = inherit
        public readonly string ClipPath;
        public readonly string Filter;
        public readonly string Mask;
        public readonly SvgDisplay Display = SvgDisplay.Inline;
        public readonly SvgFillRule FillRule = SvgFillRule.Nonzero;

        //==========================================================================
        public SvgDrawableBaseElement(SvgDocument document, SvgBaseElement parent, XElement drawableBaseElement)
          : base(document, parent, drawableBaseElement)
        {
            var opacity_attribute = drawableBaseElement.Attribute("opacity");
            if (opacity_attribute != null)
                Opacity = SvgLength.Parse(opacity_attribute.Value);

            var fill_opacity_attribute = drawableBaseElement.Attribute("fill-opacity");
            if (fill_opacity_attribute != null)
                FillOpacity = SvgLength.Parse(fill_opacity_attribute.Value);

            var stroke_opacity_attribute = drawableBaseElement.Attribute("stroke-opacity");
            if (stroke_opacity_attribute != null)
                StrokeOpacity = SvgLength.Parse(stroke_opacity_attribute.Value);

            var transform_attribute = drawableBaseElement.Attribute("transform");
            if (transform_attribute != null)
                Transform = SvgTransform.Parse(transform_attribute.Value);

            var fill_attribute = drawableBaseElement.Attribute("fill");
            if (fill_attribute != null)
                Fill = SvgPaint.Parse(fill_attribute.Value);

            var stroke_attribute = drawableBaseElement.Attribute("stroke");
            if (stroke_attribute != null)
                Stroke = SvgPaint.Parse(stroke_attribute.Value);

            var stroke_width_attribute = drawableBaseElement.Attribute("stroke-width");
            if (stroke_width_attribute != null)
                StrokeWidth = SvgLength.Parse(stroke_width_attribute.Value);

            var stroke_linecap_attribute = drawableBaseElement.Attribute("stroke-linecap");
            if (stroke_linecap_attribute != null)
                StrokeLinecap = stroke_linecap_attribute.Value switch
                {
                    "butt" => SvgStrokeLinecap.Butt,
                    "round" => SvgStrokeLinecap.Round,
                    "square" => SvgStrokeLinecap.Square,
                    "inherit" => SvgStrokeLinecap.Inherit,
                    _ => throw new NotImplementedException()
                };

            var stroke_linejoin_attribute = drawableBaseElement.Attribute("stroke-linejoin");
            if (stroke_linejoin_attribute != null)
                StrokeLinejoin = stroke_linejoin_attribute.Value switch
                {
                    "miter" => SvgStrokeLinejoin.Miter,
                    "round" => SvgStrokeLinejoin.Round,
                    "bevel" => SvgStrokeLinejoin.Bevel,
                    "inherit" => SvgStrokeLinejoin.Inherit,
                    _ => throw new NotSupportedException()
                };

            var stroke_miterlimit_attribute = drawableBaseElement.Attribute("stroke-miterlimit");
            if (stroke_miterlimit_attribute != null)
            {
                if (stroke_miterlimit_attribute.Value == "inherit")
                    StrokeMiterlimit = double.NaN;
                else
                {
                    var miterlimit = double.Parse(stroke_miterlimit_attribute.Value, CultureInfo.InvariantCulture.NumberFormat);
                    //if(miterlimit < 1)
                    //throw new NotSupportedException("A miterlimit less than 1 is not supported.");
                    StrokeMiterlimit = miterlimit;
                }
            }

            var stroke_dasharray_attribute = drawableBaseElement.Attribute("stroke-dasharray");
            if (stroke_dasharray_attribute != null)
            {
                if (stroke_dasharray_attribute.Value == "none")
                    StrokeDasharray = null;
                else if (stroke_dasharray_attribute.Value == "inherit")
                    StrokeDasharray = Array.Empty<SvgLength>();
                else
                {
                    var lengths = stroke_dasharray_attribute.Value.Split(',').Select(SvgLength.Parse).ToList();

                    if (lengths.Count % 2 == 1)
                    {
                        StrokeDasharray = new SvgLength[lengths.Count * 2];
                        for (var i = 0; i < lengths.Count - 1; ++i)
                        {
                            StrokeDasharray[i] = lengths[i];
                            StrokeDasharray[i + lengths.Count] = lengths[i];
                        }
                    }
                    else
                        StrokeDasharray = lengths.ToArray();

                }
            }

            var stroke_dashoffset_attribute = drawableBaseElement.Attribute("stroke-dashoffset");
            if (stroke_dashoffset_attribute != null)
                StrokeDashoffset = SvgLength.Parse(stroke_dashoffset_attribute.Value);

            var clip_attribute = drawableBaseElement.Attribute("clip-path");
            var clip_path = clip_attribute?.Value.Trim();
            if (clip_path?.StartsWith("url") == true)
            {
                clip_path = clip_path.Substring(3).Trim();
                if (clip_path.StartsWith("(") && clip_path.EndsWith(")"))
                {
                    clip_path = clip_path.Substring(1, clip_path.Length - 2).Trim();
                    if (clip_path.StartsWith("#"))
                        ClipPath = clip_path.Substring(1);
                }
            }

            var filter_attribute = drawableBaseElement.Attribute("filter");
            var filter = filter_attribute?.Value.Trim();
            if (filter?.StartsWith("url") == true)
            {
                filter = filter.Substring(3).Trim();
                if (filter.StartsWith("(") && filter.EndsWith(")"))
                {
                    filter = filter.Substring(1, filter.Length - 2).Trim();
                    if (filter.StartsWith("#"))
                        Filter = filter.Substring(1);
                }
            }

            var mask_attribute = drawableBaseElement.Attribute("mask");
            var mask = mask_attribute?.Value.Trim();
            if (mask?.StartsWith("url") == true)
            {
                mask = mask.Substring(3).Trim();
                if (mask.StartsWith("(") && mask.EndsWith(")"))
                {
                    mask = mask.Substring(1, mask.Length - 2).Trim();
                    if (mask.StartsWith("#"))
                        Mask = mask.Substring(1);
                }
            }

            var display_attribute = drawableBaseElement.Attribute("display");
            if (display_attribute != null)
                switch (display_attribute.Value)
                {
                    case "inline":
                        Display = SvgDisplay.Inline;
                        break;

                    case "block":
                        Display = SvgDisplay.Block;
                        break;

                    case "list-item":
                        Display = SvgDisplay.ListItem;
                        break;

                    case "run-in":
                        Display = SvgDisplay.RunIn;
                        break;

                    case "compact":
                        Display = SvgDisplay.Compact;
                        break;

                    case "marker":
                        Display = SvgDisplay.Marker;
                        break;

                    case "table":
                        Display = SvgDisplay.Table;
                        break;

                    case "inline-table":
                        Display = SvgDisplay.InlineTable;
                        break;

                    case "table-row-group":
                        Display = SvgDisplay.TableRowGroup;
                        break;

                    case "table-header-group":
                        Display = SvgDisplay.TableHeaderGroup;
                        break;

                    case "table-footer-group":
                        Display = SvgDisplay.TableFooterGroup;
                        break;

                    case "table-row":
                        Display = SvgDisplay.TableRow;
                        break;

                    case "table-column-group":
                        Display = SvgDisplay.TableColumnGroup;
                        break;

                    case "table-column":
                        Display = SvgDisplay.TableColumn;
                        break;

                    case "table-cell":
                        Display = SvgDisplay.TableCell;
                        break;

                    case "table-caption":
                        Display = SvgDisplay.TableCaption;
                        break;

                    case "none":
                        Display = SvgDisplay.None;
                        break;

                    default:
                        throw new NotImplementedException();
                }

            var fill_rule_attribute = drawableBaseElement.Attribute("fill-rule");
            if (fill_rule_attribute != null)
                FillRule = fill_rule_attribute.Value switch
                {
                    "nonzero" => SvgFillRule.Nonzero,
                    "evenodd" => SvgFillRule.Evenodd,
                    "inherit" => SvgFillRule.Inherit,
                    _ => throw new NotImplementedException()
                };

            // color, color-interpolation, color-rendering

            // viewBox attribute
            // preserveAspectRatio attribute

            // overflow


            foreach (var element in from element in drawableBaseElement.Elements()
                                    where element.Name.NamespaceName == "http://www.w3.org/2000/svg"
                                    select element)
                switch (element.Name.LocalName)
                {
                    default:
                        throw new NotImplementedException($"Unhandled element: {element}");
                }
        }

        //==========================================================================
        public abstract Geometry GetBaseGeometry();

        //==========================================================================
        public virtual Geometry? GetGeometry()
        {
            var geometry = GetBaseGeometry();
            if (geometry is null)
                return null;

            if (Transform != null)
                geometry.Transform = Transform.ToTransform();

            if (ClipPath is null) return geometry;
            var clip_path_element = Document.Elements[ClipPath] as SvgClipPathElement;
            var clip_geometry = clip_path_element?.GetClipGeometry();
            return clip_geometry != null
                ? Geometry.Combine(geometry, clip_geometry, GeometryCombineMode.Intersect, null)
                : geometry;
        }

        //==========================================================================
        public Pen? GetPen()
        {
            if (Stroke is null)
                return null;

            if (StrokeWidth.ToDouble() <= 0.0)
                return null;

            var brush = Stroke.ToBrush(this);
            brush.Opacity = Opacity.ToDouble() * StrokeOpacity.ToDouble();

            var pen = new Pen(brush, StrokeWidth.ToDouble());

            return pen;
        }

        //==========================================================================
        public Brush? GetBrush()
        {
            var brush = Fill?.ToBrush(this);
            if (brush is null)
                return null;

            brush.Opacity = Opacity.ToDouble() * FillOpacity.ToDouble();
            return brush;
        }

        //==========================================================================
        public virtual Drawing? GetBaseDrawing()
        {
            var geometry = GetGeometry();

            if (geometry?.IsEmpty() != false)
                return null;
            if (geometry.Bounds.Width <= 0.0)
                return null;
            if (geometry.Bounds.Height <= 0.0)
                return null;

            var brush = GetBrush();
            var pen = GetPen();

            if ((brush is null) && (pen is null))
                return null;


            // Apply fill-rule...
            var path_geometry = Geometry.Combine(geometry, Geometry.Empty, GeometryCombineMode.Exclude, null);
            path_geometry.FillRule = FillRule switch
            {
                SvgFillRule.Evenodd => System.Windows.Media.FillRule.EvenOdd,
                SvgFillRule.Nonzero => System.Windows.Media.FillRule.Nonzero,
                _ => path_geometry.FillRule
            };
            geometry = path_geometry;

            var geometry_drawing = new GeometryDrawing(brush, pen, geometry);

            return geometry_drawing;
        }

        //==========================================================================
        public virtual Drawing? Draw()
        {
            var drawing = GetBaseDrawing();
            if (drawing is null)
                return null;

            BitmapEffect? bitmap_effect = null;
            if (Filter != null)
                if (Document.Elements.ContainsKey(Filter))
                    if (Document.Elements[Filter] is SvgFilterElement filter_element)
                        bitmap_effect = filter_element.ToBitmapEffect();

            Brush? opacity_mask = null;
            if (Mask != null)
                if (Document.Elements[Mask] is SvgMaskElement mask_element)
                {
                    opacity_mask = mask_element.GetOpacityMask();
                    if (opacity_mask != null)
                        if (Transform != null)
                            opacity_mask.Transform = Transform.ToTransform();
                }

            if ((opacity_mask is null) && (bitmap_effect is null))
                return drawing;


            var drawing_group = new DrawingGroup { BitmapEffect = bitmap_effect, OpacityMask = opacity_mask };
            drawing_group.Children.Add(drawing);

            return drawing_group;
        }

    } // class SvgDrawableBaseElement
}
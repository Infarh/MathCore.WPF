////////////////////////////////////////////////////////////////////////////////
//
//  SvgMaskElement.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media.Imaging;
using System.Windows;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents a &lt;mask&gt; element.</summary>
class SvgMaskElement
    : SvgContainerBaseElement
{

    //==========================================================================
    public readonly SvgMaskUnits MaskUnits = SvgMaskUnits.ObjectBoundingBox;

    //==========================================================================
    public SvgMaskElement(SvgDocument document, SvgBaseElement parent, XElement MaskElement)
        : base(document, parent, MaskElement)
    {
        var mask_units_attribute = MaskElement.Attribute("maskUnits");
        if (mask_units_attribute != null)
            switch (mask_units_attribute.Value)
            {
                case "objectBoundingBox":
                    MaskUnits = SvgMaskUnits.ObjectBoundingBox;
                    break;

                case "userSpaceOnUse":
                    MaskUnits = SvgMaskUnits.UserSpaceOnUse;
                    break;

                default:
                    throw new NotImplementedException($"maskUnits value '{mask_units_attribute.Value}' is no supported");
            }
    }

    //==========================================================================
    public Geometry GetClipGeometry()
    {
        var geometry_group = new GeometryGroup();

        foreach (var child_element in Children)
        {
            var element = child_element;
            if (element is SvgUseElement use_element)
                element = use_element.GetElement();

            if (element is SvgDrawableBaseElement base_element)
            {
                var geometry = base_element.GetBaseGeometry();
                if (geometry != null)
                {
                    if (base_element.Transform != null) geometry.Transform = base_element.Transform.ToTransform();
                    geometry_group.Children.Add(geometry);
                }
            }
            else
                throw new NotImplementedException();
        }

        return geometry_group;
    }

    //==========================================================================
    private static Color ConvertColor(Color color)
    {
        var max = Math.Max(Math.Max(color.ScR, color.ScG), color.ScB);
        var min = Math.Min(Math.Min(color.ScR, color.ScG), color.ScB);

        return Color.FromScRgb((min + max) / 2, 0, 0, 0);
    }

    //==========================================================================
    private static void ConvertColors(Brush brush)
    {
        if (brush != null)
        {

            if (brush is SolidColorBrush color_brush)
                color_brush.Color = ConvertColor(color_brush.Color);
            else if (brush is GradientBrush gradient_brush)
                foreach (var gradient_stop in gradient_brush.GradientStops)
                    gradient_stop.Color = ConvertColor(gradient_stop.Color);
            else if (brush is DrawingBrush drawing_brush)
                ConvertColors(drawing_brush.Drawing);
            else
                throw new NotSupportedException();

        }
    }

    //==========================================================================
    private static void ConvertColors(Pen pen)
    {
        if (pen != null)
            ConvertColors(pen.Brush);
    }

    //==========================================================================
    private static void ConvertColors(Drawing drawing)
    {
        switch (drawing)
        {
            case DrawingGroup drawing_group:
            {
                foreach (var child_drawing in drawing_group.Children)
                    ConvertColors(child_drawing);
                break;
            }
            case GeometryDrawing geometry_drawing:
                ConvertColors(geometry_drawing.Brush);
                ConvertColors(geometry_drawing.Pen);
                break;
            case ImageDrawing { ImageSource: BitmapSource } image_drawing:
                if (image_drawing.ImageSource is BitmapSource bitmap_source)
                {
                    var bitmap = new WriteableBitmap(
                        bitmap_source.PixelWidth, bitmap_source.PixelHeight, bitmap_source.DpiX, bitmap_source.DpiY, PixelFormats.Bgra32, null);

                    var pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
                    bitmap_source.CopyPixels(pixels, (bitmap_source.PixelWidth * bitmap_source.Format.BitsPerPixel + 7) / 8, 0);

                    for (var i = 0; i < pixels.Length; i += 4)
                    {
                        var r = pixels[i + 0];
                        var g = pixels[i + 1];
                        var b = pixels[i + 2];

                        var max = Math.Max(Math.Max(r, g), b);
                        var min = Math.Min(Math.Min(r, g), b);

                        var a = (byte)(((int)min + (int)max) / 2);

                        pixels[i + 3] = a;
                    }

                    bitmap.WritePixels(
                        new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels,
                        (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8, 0);

                    image_drawing.ImageSource = bitmap;
                }

                break;
            case ImageDrawing { ImageSource: DrawingImage } image_drawing:
                ConvertColors((image_drawing.ImageSource as DrawingImage).Drawing);
                break;
            case ImageDrawing: throw new NotSupportedException();
            default:           throw new NotSupportedException();
        }
    }

    //==========================================================================
    public DrawingBrush? GetOpacityMask()
    {
        var drawing_group = new DrawingGroup();

        foreach (var child_element in Children)
        {
            var element = child_element;
            if (element is SvgUseElement use_element)
                element = use_element.GetElement();

            var drawing = element switch
            {
                SvgDrawableBaseElement drawable_base_element => drawable_base_element.Draw(),
                SvgDrawableContainerBaseElement base_element => base_element.Draw(),
                _                                            => null
            };

            if (drawing != null)
                drawing_group.Children.Add(drawing);
        }

        if (drawing_group.Children.Count == 0)
            return null;

        foreach (var drawing in drawing_group.Children)
            ConvertColors(drawing);


        var brush = new DrawingBrush(drawing_group);

        if (MaskUnits != SvgMaskUnits.UserSpaceOnUse) return brush;
        brush.ViewportUnits = BrushMappingMode.Absolute;
        brush.Viewport      = drawing_group.Bounds;

        return brush;
    }
} // class SvgMaskElement
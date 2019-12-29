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
using System;
using System.Windows.Media;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows;

namespace MathCore.WPF.SVG
{
  
  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;mask&gt; element.
  /// </summary>
  class SvgMaskElement
    : SvgContainerBaseElement
  {

    //==========================================================================
    public readonly SvgMaskUnits MaskUnits = SvgMaskUnits.ObjectBoundingBox;

    //==========================================================================
    public SvgMaskElement(SvgDocument document, SvgBaseElement parent, XElement maskElement)
      : base(document, parent, maskElement)
    {
      XAttribute mask_units_attribute = maskElement.Attribute("maskUnits");
      if(mask_units_attribute != null)
        switch(mask_units_attribute.Value)
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
      GeometryGroup geometry_group = new GeometryGroup();

      foreach(SvgBaseElement child_element in Children)
      {
        SvgBaseElement element = child_element;
        if(element is SvgUseElement)
          element = (element as SvgUseElement).GetElement();

        if(element is SvgDrawableBaseElement)
        {
          Geometry geometry = (element as SvgDrawableBaseElement).GetBaseGeometry();
          if(geometry != null)
          {
            if((element as SvgDrawableBaseElement).Transform != null)
            {
              geometry.Transform = (element as SvgDrawableBaseElement).Transform.ToTransform();
            }
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
      float max = Math.Max(Math.Max(color.ScR, color.ScG), color.ScB);
      float min = Math.Min(Math.Min(color.ScR, color.ScG), color.ScB);

      return Color.FromScRgb((min + max) / 2, 0, 0, 0);
    }

    //==========================================================================
    private static void ConvertColors(Brush brush)
    {
      if(brush != null)
      {

        if(brush is SolidColorBrush)
        {
          (brush as SolidColorBrush).Color = ConvertColor((brush as SolidColorBrush).Color);
        }
        else if(brush is GradientBrush)
        {
          foreach(GradientStop gradient_stop in (brush as GradientBrush).GradientStops)
            gradient_stop.Color = ConvertColor(gradient_stop.Color);
        }
        else if(brush is DrawingBrush)
        {
          ConvertColors((brush as DrawingBrush).Drawing);
        }
        else
          throw new NotSupportedException();

      }
    }

    //==========================================================================
    private static void ConvertColors(Pen pen)
    {
      if(pen != null)
        ConvertColors(pen.Brush);
    }

    //==========================================================================
    private static void ConvertColors(Drawing drawing)
    {
      if(drawing is DrawingGroup)
      {
        foreach(Drawing child_drawing in (drawing as DrawingGroup).Children)
          ConvertColors(child_drawing);
      }
      else if(drawing is GeometryDrawing)
      {
        ConvertColors((drawing as GeometryDrawing).Brush);
        ConvertColors((drawing as GeometryDrawing).Pen);
      }
      else if(drawing is ImageDrawing)
      {
        if((drawing as ImageDrawing).ImageSource is BitmapSource)
        {
          BitmapSource bitmap_source = (drawing as ImageDrawing).ImageSource as BitmapSource;
          if(bitmap_source != null)
          {
            WriteableBitmap bitmap = new WriteableBitmap(bitmap_source.PixelWidth, bitmap_source.PixelHeight, bitmap_source.DpiX, bitmap_source.DpiY, PixelFormats.Bgra32, null);

            byte[] pixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
            bitmap_source.CopyPixels(pixels, (bitmap_source.PixelWidth * bitmap_source.Format.BitsPerPixel + 7) / 8, 0);

            for(int i = 0; i < pixels.Length; i+=4)
            {
              byte r = pixels[i + 0];
              byte g = pixels[i + 1];
              byte b = pixels[i + 2];

              byte max = Math.Max(Math.Max(r, g), b);
              byte min = Math.Min(Math.Min(r, g), b);

              byte a = (byte)(((int)min + (int)max) / 2);

              pixels[i + 3] = a;
            }   
            


            bitmap.WritePixels(new Int32Rect(0,0,bitmap.PixelWidth, bitmap.PixelHeight), pixels,
                                      (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8, 0);



            (drawing as ImageDrawing).ImageSource = bitmap;
          }
        }
        else if((drawing as ImageDrawing).ImageSource is DrawingImage)
        {
          ConvertColors(((drawing as ImageDrawing).ImageSource as DrawingImage).Drawing);
        }
        else
          throw new NotSupportedException();
      }
      else
        throw new NotSupportedException();
    }

    //==========================================================================
    public DrawingBrush GetOpacityMask()
    {
      DrawingGroup drawing_group = new DrawingGroup();

      foreach(SvgBaseElement child_element in Children)
      {
        SvgBaseElement element = child_element;
        if(element is SvgUseElement)
          element = (element as SvgUseElement).GetElement();

        Drawing drawing = null;

        if(element is SvgDrawableBaseElement)
          drawing = (element as SvgDrawableBaseElement).Draw();
        else if(element is SvgDrawableContainerBaseElement)
          drawing = (element as SvgDrawableContainerBaseElement).Draw();

        if(drawing != null)
          drawing_group.Children.Add(drawing);
      }

      if(drawing_group.Children.Count == 0)
        return null;

      foreach(Drawing drawing in drawing_group.Children)
        ConvertColors(drawing);


      DrawingBrush brush = new DrawingBrush(drawing_group);

      if(MaskUnits == SvgMaskUnits.UserSpaceOnUse)
      {
        brush.ViewportUnits = BrushMappingMode.Absolute;
        brush.Viewport = drawing_group.Bounds;
      }

      return brush;
    }



  } // class SvgMaskElement

}

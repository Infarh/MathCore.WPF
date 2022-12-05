////////////////////////////////////////////////////////////////////////////////
//
//  SvgImageElement.cs - This file is part of Svg2Xaml.
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

using System.Windows.Media;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Represents an &lt;image&gt; element.</summary>
internal class SvgImageElement : SvgDrawableBaseElement
{
    //==========================================================================
    public readonly SvgCoordinate Y = new(0.0);
    public readonly SvgCoordinate X = new(0.0);
    public readonly SvgLength Width = new(0.0);
    public readonly SvgLength Height = new(0.0);

    //==========================================================================
    public readonly string DataType;
    public readonly byte[]? Data;

    //==========================================================================
    public SvgImageElement(SvgDocument document, SvgBaseElement parent, XElement ImageElement)
        : base(document, parent, ImageElement)
    {
        if(ImageElement.Attribute("x") is { } x_attribute)
            X = SvgCoordinate.Parse(x_attribute.Value);

        if(ImageElement.Attribute("y") is { } y_attribute)
            Y = SvgCoordinate.Parse(y_attribute.Value);

        if(ImageElement.Attribute("width") is { } width_attribute)
            Width = SvgLength.Parse(width_attribute.Value);

        if(ImageElement.Attribute("height") is { } height_attribute)
            Height = SvgLength.Parse(height_attribute.Value);

        if (ImageElement.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")) is not { } href_attribute) 
            return;

        var reference = href_attribute.Value.TrimStart();
        if (!reference.StartsWith("data:")) return;

        reference = reference[5..].TrimStart();
        var index = reference.IndexOf(";", StringComparison.Ordinal);
        if (index <= -1) 
            return;

        var type = reference[..index].Trim();
        reference = reference[(index + 1)..];

        index = reference.IndexOf(",", StringComparison.Ordinal);
        var encoding = reference[..index].Trim();
        reference = reference[(index + 1)..].TrimStart();

        Data = encoding switch
        {
            "base64" => Convert.FromBase64String(reference),
            _        => throw new NotSupportedException($"Unsupported encoding: {encoding}")
        };

        var type_tokens = type.Split('/');
        if(type_tokens.Length != 2)
            throw new NotSupportedException($"Unsupported type: {type}");

        type_tokens[0] = type_tokens[0].Trim();
        if(type_tokens[0] != "image")
            throw new NotSupportedException($"Unsupported type: {type}");

        DataType = type_tokens[1].Trim() switch
        {
            "jpeg" => "jpeg",
            "png"  => "png",
            _      => throw new NotSupportedException($"Unsupported type: {type}")
        };
    }

    //==========================================================================
    public override Drawing? GetBaseDrawing()
    {
        if(Data is null)
            return null;

        var temp_file = Path.GetTempFileName();
        using(var file_stream = new FileStream(temp_file, FileMode.Create, FileAccess.Write))
        using(var writer = new BinaryWriter(file_stream))
            writer.Write(Data);

        return new ImageDrawing(new BitmapImage(new Uri(temp_file)), new Rect(
            new Point(X.ToDouble(), Y.ToDouble()),
            new Size(Width.ToDouble(), Height.ToDouble())
        ));
    }

    //==========================================================================
    public override Geometry GetBaseGeometry() => new RectangleGeometry(new(
        new(X.ToDouble(), Y.ToDouble()),
        new Size(Width.ToDouble(), Height.ToDouble())
    ));
} // class SvgImageElement
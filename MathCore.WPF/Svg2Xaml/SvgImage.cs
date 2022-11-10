////////////////////////////////////////////////////////////////////////////////
//
//  SvgImage.cs - This file is part of Svg2Xaml.
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

using System.Windows.Markup;
using System.Windows;
using System.IO;
using System.Windows.Media;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  A <see cref="MarkupExtension"/> for loading SVG images.</summary>
[MarkupExtensionReturnType(typeof(DrawingImage))]
public class SvgImage : MarkupExtension
{
    //==========================================================================

    private Uri _Uri;
    private bool _MIgnoreEffects;

    //==========================================================================
    /// <summary>  Initializes a new <see cref="SvgImage"/> instance.</summary>
    public SvgImage()
    {
        // ...
    }

    //==========================================================================
    /// <summary>  Initializes a new <see cref="SvgImage"/> instance.</summary>
    /// <param name="uri">
    ///   The location of the SVG document.
    /// </param>
    public SvgImage(Uri uri) => _Uri = uri;

    //==========================================================================
    /// <summary>
    ///   Overrides <see cref="MarkupExtension.ProvideValue"/> and returns the 
    ///   <see cref="DrawingImage"/> the SVG document is rendered into.
    /// </summary>
    /// <param name="ServiceProvider">
    ///   Object that can provide services for the markup extension; 
    ///   <paramref name="ServiceProvider"/> is not used.
    /// </param>
    /// <returns>
    ///   The <see cref="DrawingImage"/> the SVG image is rendered into or 
    ///   <c>null</c> in case there has been an error while parsing or 
    ///   rendering.
    /// </returns>
    public override object ProvideValue(IServiceProvider ServiceProvider)
    {
        Stream stream = null;
        try
        {

            try { stream = Application.GetResourceStream(_Uri)?.Stream; } catch (IOException) { }
            if(stream is null && File.Exists(_Uri.ToString()))
                stream = new FileStream(_Uri.ToString(), FileMode.Open);
            else
                return null;
            return SvgReader.Load(stream, new SvgReaderOptions { IgnoreEffects = _MIgnoreEffects });
        }
        finally
        {
            stream?.Dispose();
        }
    }

    //==========================================================================
    /// <summary>  Gets or sets the location of the SVG image.</summary>
    public Uri Uri { get => _Uri; set => _Uri = value; }

    //==========================================================================
    /// <summary>
    ///   Gets or sets whether SVG filter effects should be transformed into
    ///   WPF bitmap effects.
    /// </summary>
    public bool IgnoreEffects { get => _MIgnoreEffects; set => _MIgnoreEffects = value; }

} // class SvgImage
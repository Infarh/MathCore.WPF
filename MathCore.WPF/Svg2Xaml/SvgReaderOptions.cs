////////////////////////////////////////////////////////////////////////////////
//
//  SvgReaderOptions.cs - This file is part of Svg2Xaml.
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

using System.Windows.Media.Effects;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>
///   Defines a set of options to customize rendering repspectively reading 
///   of SVG documents.
/// </summary>
public class SvgReaderOptions
{

    //==========================================================================
    private bool _MIgnoreEffects;

    //==========================================================================
    /// <summary>  Initializes a new <see cref="SvgReaderOptions"/> instance.</summary>
    public SvgReaderOptions()
    {
        // ...
    }

    //==========================================================================
    /// <summary>  Initializes a new <see cref="SvgReaderOptions"/> instance.</summary>
    /// <param name="IgnoreEffects">
    ///   Specifies whether filter effects should be applied using WPF bitmap 
    ///   effects.
    /// </param>
    public SvgReaderOptions(bool IgnoreEffects) => _MIgnoreEffects = IgnoreEffects;

    //==========================================================================
    /// <summary>
    ///   Gets or sets whether SVG effects should either be ignored or 
    ///   converted to <see cref="BitmapEffect">bitmap effects</see>.
    /// </summary>
    public bool IgnoreEffects 
    {
        get => _MIgnoreEffects;

        set => _MIgnoreEffects = value;
    }

} // class SvgReaderOptions
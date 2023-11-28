////////////////////////////////////////////////////////////////////////////////
//
//  SvgDocument.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18341 $
//  $LastChangedDate: 2009-03-17 13:03:30 +0100 (Tue, 17 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////

using System.Windows.Media;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
sealed class SvgDocument
{
    //==========================================================================
    public readonly Dictionary<string, SvgBaseElement> Elements = [];

    //==========================================================================
    public readonly SvgSvgElement    Root;
    public readonly SvgReaderOptions Options;

    //==========================================================================
    public SvgDocument(XElement root, SvgReaderOptions options)
    {
        Root    = new SvgSvgElement(this, null, root);
        Options = options;
    }

    //==========================================================================
    public DrawingImage Draw() => new(Root.Draw());
} // class SvgDocument
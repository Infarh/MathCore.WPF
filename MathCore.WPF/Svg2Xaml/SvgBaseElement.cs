////////////////////////////////////////////////////////////////////////////////
//
//  SvgBaseElement.cs - This file is part of Svg2Xaml.
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
using System.Xml;
using System.Xml.Linq;

namespace MathCore.WPF.SVG;

//****************************************************************************
/// <summary>  Base class for all other SVG elements.</summary>
class SvgBaseElement
{

    //==========================================================================
    public readonly SvgDocument Document;

    //==========================================================================
    public readonly string? Reference;

    //==========================================================================
    public SvgSvgElement Root => Document.Root;

    //==========================================================================
    public readonly SvgBaseElement Parent;

    //==========================================================================
    public readonly string Id;

    //==========================================================================
    public readonly XElement Element;

    //==========================================================================
    protected SvgBaseElement(SvgDocument document, SvgBaseElement parent, XElement element)
    {
        Document = document;
        Parent   = parent;

        // Create attributes from styles...
        var style_attribute = element.Attribute("style");
        if(style_attribute != null)
        {
            foreach(var property in style_attribute.Value.Split(';'))
            {
                var tokens = property.Split(':');
                if (tokens.Length != 2) continue;

                try
                {
                    element.SetAttributeValue(tokens[0], tokens[1]);
                }
                catch(XmlException)
                { }
            }
            style_attribute.Remove();
        }

        var id_attribute = element.Attribute("id");
        if(id_attribute != null)
            Document.Elements[Id = id_attribute.Value] = this;

        if(element.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink")) is { Value: [ '#', _ ] reference })
            Reference = reference[1..];

        Element = element;
    }

} // class SvgBaseElement
////////////////////////////////////////////////////////////////////////////////
//
//  SvgGradientBaseElement.cs - This file is part of Svg2Xaml.
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

namespace MathCore.WPF.SVG;

//****************************************************************************
abstract class SvgGradientBaseElement
    : SvgBaseElement
{
    //==========================================================================
    public readonly List<SvgStopElement> Stops = [];

    //==========================================================================
    public readonly SvgGradientUnits GradientUnits = SvgGradientUnits.ObjectBoundingBox;
    public readonly SvgSpreadMethod SpreadMethod = SvgSpreadMethod.Pad;
    public readonly SvgTransform? Transform;
    
    //==========================================================================
    public SvgGradientBaseElement(SvgDocument document, SvgBaseElement parent, XElement GradientElement)
        : base(document, parent, GradientElement)
    {

        var gradient_units_attribute = GradientElement.Attribute("gradientUnits");
        if(gradient_units_attribute != null)
            switch(gradient_units_attribute.Value)
            {
                case "objectBoundingBox":
                    GradientUnits = SvgGradientUnits.ObjectBoundingBox;
                    break;

                case "userSpaceOnUse":
                    GradientUnits = SvgGradientUnits.UserSpaceOnUse;
                    break;

                default:
                    throw new NotImplementedException($"gradientUnits value '{gradient_units_attribute.Value}' is no supported");
            }

        var gradient_transform_attribute = GradientElement.Attribute("gradientTransform");
        if(gradient_transform_attribute != null)
            Transform = SvgTransform.Parse(gradient_transform_attribute.Value);

        var spread_method_attribute = GradientElement.Attribute("spreadMethod");
        if(spread_method_attribute != null)
            switch(spread_method_attribute.Value)
            {
                case "pad":
                    SpreadMethod = SvgSpreadMethod.Pad;
                    break;

                case "reflect":
                    SpreadMethod = SvgSpreadMethod.Reflect;
                    break;

                case "repeat":
                    SpreadMethod = SvgSpreadMethod.Repeat;
                    break;
            }



        foreach(var element in from element in GradientElement.Elements()
                               where element.Name.NamespaceName == "http://www.w3.org/2000/svg"
                               select element)
            switch(element.Name.LocalName)
            {
                case "stop":
                    Stops.Add(new(Document, this, element));
                    break;

                default:
                    throw new NotImplementedException($"Unhandled element: {element}");
            }
    }

    //==========================================================================
    protected abstract GradientBrush CreateBrush();

    //==========================================================================
    protected virtual GradientBrush SetBrush(GradientBrush brush)
    {
        switch(SpreadMethod)
        {
            case SvgSpreadMethod.Pad:
                brush.SpreadMethod = GradientSpreadMethod.Pad;
                break;

            case SvgSpreadMethod.Reflect:
                brush.SpreadMethod = GradientSpreadMethod.Reflect;
                break;

            case SvgSpreadMethod.Repeat:
                brush.SpreadMethod = GradientSpreadMethod.Repeat;
                break;
        }

        switch(GradientUnits)
        {
            case SvgGradientUnits.ObjectBoundingBox:
                brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                break;

            case SvgGradientUnits.UserSpaceOnUse:
                brush.MappingMode = BrushMappingMode.Absolute;
                break;
        }

        if(Transform != null)
            brush.Transform = Transform.ToTransform();

        foreach(var stop in Stops)
            brush.GradientStops.Add(stop.ToGradientStop());

        return brush;
    }

    //==========================================================================
    public GradientBrush? ToBrush()
    {
        var brush = CreateBrush();

        if (Reference == null) return SetBrush(brush);
        if (!Document.Elements.ContainsKey(Reference))
            return null;

        if(Document.Elements[Reference] is not SvgGradientBaseElement reference)
            throw new NotImplementedException();

        reference.SetBrush(brush);

        return SetBrush(brush);
    }

} // class SvgGradientBaseElement
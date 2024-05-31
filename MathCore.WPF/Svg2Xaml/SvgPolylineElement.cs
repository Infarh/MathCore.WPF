﻿////////////////////////////////////////////////////////////////////////////////
//
//  SvgPolylineElement.cs - This file is part of Svg2Xaml.
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
using System.Globalization;

namespace MathCore.WPF.SVG;

//****************************************************************************
class SvgPolylineElement
    : SvgDrawableBaseElement
{
    //==========================================================================
    public readonly List<SvgPoint> Points = [];

    //==========================================================================
    public SvgPolylineElement(SvgDocument document, SvgBaseElement parent, XElement PolylineElement)
        : base(document, parent, PolylineElement)
    {
        var points_attribute = PolylineElement.Attribute("points");
        if(points_attribute != null)
        {
            var coordinates = new List<double>();

            var points = points_attribute.Value.Split(',', ' ', '\t');
            foreach(var coordinate_value in points)
            {
                var coordinate = coordinate_value.Trim();
                if(coordinate == "")
                    continue;
                coordinates.Add(double.Parse(coordinate, CultureInfo.InvariantCulture.NumberFormat));
            }

            for(var i = 0; i < coordinates.Count - 1; i += 2)
                Points.Add(new(coordinates[i], coordinates[i + 1]));
        }
    }

    //==========================================================================
    public override Geometry? GetBaseGeometry()
    {
        if(Points.Count == 0)
            return null;

        var path_figure = new PathFigure();

        path_figure.StartPoint = Points[0].ToPoint();
        path_figure.IsClosed   = true;
        path_figure.IsFilled   = false;

        for(var i = 1; i < Points.Count; ++i)
            path_figure.Segments.Add(new LineSegment(Points[i].ToPoint(), true));

        var path_geometry = new PathGeometry();
        path_geometry.Figures.Add(path_figure);

        return path_geometry;
    }

} // class SvgPolylineElement
﻿////////////////////////////////////////////////////////////////////////////////
//
//  SvgTransformGroup.cs - This file is part of Svg2Xaml.
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
//  $LastChangedRevision: 18379 $
//  $LastChangedDate: 2009-03-17 17:43:58 +0100 (Tue, 17 Mar 2009) $
//  $LastChangedBy: unknown $
//
////////////////////////////////////////////////////////////////////////////////

using System.Windows.Media;

namespace MathCore.WPF.SVG;

//****************************************************************************
internal class SvgTransformGroup
    : SvgTransform
{
    //==========================================================================
    public readonly List<SvgTransform> Transforms = [];

    //==========================================================================
    public SvgTransformGroup(SvgTransform[] transforms)
    {
        foreach(var transform in transforms)
            Transforms.Add(transform);
    }

    //==========================================================================
    public override Transform ToTransform()
    {
        var transform_group = new TransformGroup();

        foreach(var transform in Transforms)
            transform_group.Children.Add(transform.ToTransform());

        return transform_group;
    }

} // class SvgTransformGroup
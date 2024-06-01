////////////////////////////////////////////////////////////////////////////////
//
//  SvgTransform.cs - This file is part of Svg2Xaml.
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
internal abstract class SvgTransform
{

    //==========================================================================
    public static SvgTransform Parse(string value)
    {
        if(value is null)
            throw new ArgumentNullException(nameof(value));
      
        value = value.Trim();
        if(value == "")
            throw new ArgumentException("value must not be empty", nameof(value));

        var transforms = new List<SvgTransform>();

        var transform = value;
        while(transform.Length > 0)
        {

            if(transform.StartsWith("translate"))
            {
                transform = transform[9..].TrimStart();
                if(transform.StartsWith("("))
                {
                    transform = transform[1..];
                    var index = transform.IndexOf(")", StringComparison.Ordinal);
                    if(index >= 0)
                    {
                        transforms.Add(SvgTranslateTransform.Parse(transform[..index].Trim()));
                        transform = transform[(index + 1)..].TrimStart();
                        continue;
                    }
                }
            }
          
            if(transform.StartsWith("matrix"))
            {
                transform = transform[6..].TrimStart();
                if(transform.StartsWith("("))
                {
                    transform = transform[1..];
                    var index = transform.IndexOf(")", StringComparison.Ordinal);
                    if(index >= 0)
                    {
                        transforms.Add(SvgMatrixTransform.Parse(transform[..index].Trim()));
                        transform = transform[(index + 1)..].TrimStart();
                        continue;
                    }
                }
            }

            if(transform.StartsWith("scale"))
            {
                transform = transform[5..].TrimStart();
                if(transform.StartsWith("("))
                {
                    transform = transform[1..];
                    var index = transform.IndexOf(")", StringComparison.Ordinal);
                    if(index >= 0)
                    {
                        transforms.Add(SvgScaleTransform.Parse(transform[..index].Trim()));
                        transform = transform[(index + 1)..].TrimStart();
                        continue;
                    }
                }
            }

            if(transform.StartsWith("skew"))
            {
                transform = transform[5..].TrimStart();
                if(transform.StartsWith("("))
                {
                    transform = transform[1..];
                    var index = transform.IndexOf(")", StringComparison.Ordinal);
                    if(index >= 0)
                    {
                        transforms.Add(SvgSkewTransform.Parse(transform[..index].Trim()));
                        transform = transform[(index + 1)..].TrimStart();
                        continue;
                    }
                }
            }

            if(transform.StartsWith("rotate"))
            {
                transform = transform[6..].TrimStart();
                if (!transform.StartsWith("(")) continue;

                transform = transform[1..];
                var index = transform.IndexOf(")", StringComparison.Ordinal);
                if (index < 0) continue;

                transforms.Add(SvgScaleTransform.Parse(transform[..index].Trim()));
                transform = transform[(index + 1)..].TrimStart();
            }

        }

        return transforms.Count switch
        {
            1   => transforms[0],
            > 1 => new SvgTransformGroup(transforms.ToArray()),
            _   => throw new ArgumentException($"Unsupported transform value: {value}")
        };
    }

    //==========================================================================
    public abstract Transform ToTransform();

} // class Transform
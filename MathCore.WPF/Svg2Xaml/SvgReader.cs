////////////////////////////////////////////////////////////////////////////////
//
//  SvgReader.cs - This file is part of Svg2Xaml.
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
using System.IO;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace MathCore.WPF.SVG
{

    //****************************************************************************
    /// <summary>
    ///   Provides methods to read (and render) SVG documents.
    /// </summary>
    public static class SvgReader
    {

        //==========================================================================
        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="reader">
        ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <param name="options">
        ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
        ///   rendering the SVG document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(XmlReader reader, SvgReaderOptions options)
        {
            if(options is null)
                options = new SvgReaderOptions();

            XDocument document = XDocument.Load(reader);
            if(document.Root.Name.NamespaceName != "http://www.w3.org/2000/svg")
                throw new XmlException("Root element is not in namespace 'http://www.w3.org/2000/svg'.");
            if(document.Root.Name.LocalName != "svg")
                throw new XmlException("Root element is not an <svg> element.");

            return new SvgDocument(document.Root, options).Draw();
        }

        //==========================================================================
        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="reader">
        ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(XmlReader reader) => Load(reader, null);

        //==========================================================================
        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="stream">
        ///   A <see cref="Stream"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <param name="options">
        ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
        ///   rendering the SVG document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(Stream stream, SvgReaderOptions options)
        {
            using(XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                return Load(reader, options);
        }

        //==========================================================================
        /// <summary>
        ///   Loads an SVG document and renders it into a 
        ///   <see cref="DrawingImage"/>.
        /// </summary>
        /// <param name="stream">
        ///   A <see cref="Stream"/> to read the XML structure of the SVG 
        ///   document.
        /// </param>
        /// <returns>
        ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
        /// </returns>
        public static DrawingImage Load(Stream stream) => Load(stream, null);
    } // class SvgReader

}

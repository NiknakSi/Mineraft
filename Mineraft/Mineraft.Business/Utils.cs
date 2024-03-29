﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Mineraft.Business
{
    /// <summary>
    /// A collection of utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Converts a text string into a List of Glyphs which can be rendered onto the output
        /// </summary>
        /// <param name="chars">Characters to render</param>
        /// <param name="consoleColor">Color to render the chars as if necessary</param>
        public static List<Glyph> StringToLine(string chars, ConsoleColor? consoleColor = null)
        {
            return chars.Select(c => new Glyph(c, consoleColor)).ToList();
        }

        /// <summary>
        /// Converts a horizontal grid index value into its corresponding text value used by the X legend
        /// </summary>
        /// <param name="index">Index of the position to convert</param>
        public static string GetXLegendValue(int index)
        {
            var utfA = 65;
            return char.ConvertFromUtf32(utfA + index);
        }
    }
}

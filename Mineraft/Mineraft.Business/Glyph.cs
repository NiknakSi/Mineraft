using System;

namespace Mineraft.Business
{
    /// <summary>
    /// Represents an individual element that the output renderer can work with
    /// </summary>
    public class Glyph
    {
        /// <summary>
        /// The value for the element
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// A color for the element to use
        /// </summary>
        public ConsoleColor? Color { get; }

        public Glyph(char value, ConsoleColor? color = null)
        {
            Value = value.ToString();
            Color = color;
        }

        public Glyph(string value, ConsoleColor? color = null)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value must contain a non-empty string");

            Value = value;
            Color = color;
        }
    }
}

using System;

namespace Mineraft.Business.Services.Contracts
{
    /// <summary>
    /// Designed to wrap the standard System.Console
    /// </summary>
    public interface IOutputRenderer
    {
        string WindowTitle { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        bool ShowCursor { get; set; }
        ConsoleKey ReadInput { get; }
        ConsoleColor BackgroundColor { get; set; }
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// A simple test to confirm if the environment will support display of emojis
        /// </summary>
        bool EmojiSupport { get; }

        /// <summary>
        /// Returns the string representation of the specified game square status to use in a glyph
        /// </summary>
        /// <param name="square">Square to convert</param>
        string ConvertGameSquareToGlyphValue(Enums.SquareStatus square);

        void Init(string gameTitle);
        void ResetColors();

        /// <summary>
        /// Ensures the console colours are reset to the game defaults
        /// </summary>
        void SetGameDefaultColors();
        void RenderSingle(Glyph value);
        void Flush();
        void RenderGroupWithFlush(string value);
        void SetWindowSize(int width, int height);
        void Clear();
    }
}

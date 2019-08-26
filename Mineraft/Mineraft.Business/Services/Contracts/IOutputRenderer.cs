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

        void ResetColors();
        void SetGameDefaultColors();
        void RenderSingle(Glyph value);
        void Flush();
        void RenderGroupWithFlush(string value);
        void SetWindowSize(int width, int height);
        void Clear();
    }
}

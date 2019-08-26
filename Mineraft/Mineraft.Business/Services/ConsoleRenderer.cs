using System;
using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business.Services
{
    public class ConsoleRenderer : IOutputRenderer
    {
        public string WindowTitle { get => Console.Title; set => Console.Title = value; }
        public bool ShowCursor { get => Console.CursorVisible; set => Console.CursorVisible = value; }
        public int WindowWidth { get => Console.WindowWidth; set => Console.WindowWidth = value; }
        public int WindowHeight { get => Console.WindowHeight; set => Console.WindowHeight = value; }
        public ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }
        public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }

        public ConsoleKey ReadInput => Console.ReadKey(true).Key;

        public void ResetColors() => Console.ResetColor();

        /// <summary>
        /// Ensures the console colours are reset to the game defaults
        /// </summary>
        public void SetGameDefaultColors()
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.Green;
        }

        public void SetWindowSize(int width, int height) => Console.SetWindowSize(width, height);

        public void RenderSingle(Glyph value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Color.HasValue)
                ForegroundColor = value.Color.Value;
            else
                SetGameDefaultColors();

            Console.Write(value.Value);
        }

        public void Flush() => Console.WriteLine();
        public void RenderGroupWithFlush(string value) => Console.WriteLine(value);

        public void Clear() => Console.Clear();
    }
}

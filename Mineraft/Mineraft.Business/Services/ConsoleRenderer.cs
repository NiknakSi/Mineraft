using System;
using System.Threading.Tasks;
using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business.Services
{
    public class ConsoleRenderer : IOutputRenderer
    {
        private const int MINIMUMWINDOWCOLS = 120;
        private const int MINIMUMWINDOWROWS = 35;

        public string WindowTitle { get => Console.Title; set => Console.Title = value; }
        public bool ShowCursor { get => Console.CursorVisible; set => Console.CursorVisible = value; }
        public int WindowWidth { get => Console.WindowWidth; set => Console.WindowWidth = value; }
        public int WindowHeight { get => Console.WindowHeight; set => Console.WindowHeight = value; }
        public ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }
        public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
        public bool EmojiSupport { get { return Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix; } }

        public ConsoleKey ReadInput => Console.ReadKey(true).Key;

        public void ResetColors() => Console.ResetColor();

        public void Init(string gameTitle)
        {
            WindowTitle = gameTitle;
            Clear();

            //configure the window size
            EnsureWindowSize(MINIMUMWINDOWCOLS, MINIMUMWINDOWROWS).Wait();

            ShowCursor = false;
        }

        public string ConvertGameSquareToGlyphValue(Enums.SquareStatus square)
        {
            //note - emojis typically take up 2 characters of space when counted or rendered

            switch (square)
            {
                case Enums.SquareStatus.Mine:
                    return EmojiSupport ? "💣" : "<>";
                case Enums.SquareStatus.Debris:
                    return EmojiSupport ? "💥" : "XX";
                default:
                    return EmojiSupport ? "🌊" : "  ";
            }
        }

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

        /// <summary>
        /// Ensure that the console window is big enough to house the game properly either by
        /// automatically sizing it or waiting for the user to resize it
        /// </summary>
        /// <param name="width">Required nubmer of columns</param>
        /// <param name="height">Required number of rows</param>
        private async Task EnsureWindowSize(int width, int height)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetWindowSize(width, height);

            //on mac/linux we can't automatically set the size so it has to be done by the user
            if (WindowWidth < width || WindowHeight < height)
                RenderGroupWithFlush($"Please enlarge the window to {width}x{height} minimum");

            //wait for the user to resize the window
            while (WindowWidth < width || WindowHeight < height)
                await Task.Delay(500);
        }
    }
}

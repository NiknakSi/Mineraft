using System;
using System.Threading.Tasks;
using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business
{
    /// <summary>
    /// The core of the application which handles user input and coordinates the game engine
    /// </summary>
    public class MineraftApplication
    {
        public const string TITLE = "MINERAFT";
        public const int DIFFICULTYFACTOR = 5; //smaller numbers mean more mines
        public const int STARTLIVES = 5;
        private const int MINIMUMWINDOWCOLS = 120;
        private const int MINIMUMWINDOWROWS = 35;

        private readonly IOutputRenderer _Output;
        private readonly IGameEngine _GameEngine;

        public MineraftApplication(IOutputRenderer output, IGameEngine gameEngine)
        {
            _Output = output;
            _GameEngine = gameEngine;
        }

        /// <summary>
        /// Represents the main game loop
        /// </summary>
        public void Run()
        {
            _Output.WindowTitle = TITLE;

            //configure the window size
            EnsureWindowSize(MINIMUMWINDOWCOLS, MINIMUMWINDOWROWS).Wait();
            
            _Output.ShowCursor = false;

            //reset and init the game engine
            _GameEngine.Reset();

            //listen for some user input
            var keyPress = _Output.ReadInput;

            //escape always quits no matter the game state
            while (keyPress != ConsoleKey.Escape)
            {
                switch (keyPress)
                {
                    //difficulty selection
                    case ConsoleKey.E:
                        _GameEngine.SetDifficulty(Enums.Difficulty.Easy);
                        break;
                    case ConsoleKey.M:
                        _GameEngine.SetDifficulty(Enums.Difficulty.Medium);
                        break;
                    case ConsoleKey.H:
                        _GameEngine.SetDifficulty(Enums.Difficulty.Hard);
                        break;

                    //start position
                    case ConsoleKey.D0:
                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                    case ConsoleKey.D6:
                    case ConsoleKey.D7:
                    case ConsoleKey.D8:
                    case ConsoleKey.D9:
                        _GameEngine.SetDesiredStartPosition((int)keyPress - (int)ConsoleKey.D0);
                        break;
                    case ConsoleKey.Enter:
                        _GameEngine.ConfirmDesiredStartPosition();
                        break;
                    case ConsoleKey.Backspace:
                        _GameEngine.SetDesiredStartPosition(null);
                        break;

                    //player movement
                    case ConsoleKey.UpArrow:
                        _GameEngine.TryMove(Enums.Direction.North);
                        break;
                    case ConsoleKey.RightArrow:
                        _GameEngine.TryMove(Enums.Direction.East);
                        break;
                    case ConsoleKey.DownArrow:
                        _GameEngine.TryMove(Enums.Direction.South);
                        break;
                    case ConsoleKey.LeftArrow:
                        _GameEngine.TryMove(Enums.Direction.West);
                        break;

                    //misc
                    case ConsoleKey.R:
                        _GameEngine.Reset();
                        break;
                    default:
                        //_Output.RenderGroupWithFlush("Press ESC to quit or R to reset");
                        break;
                }

                keyPress = _Output.ReadInput;
            }

            _Output.ResetColors();
            _Output.ShowCursor = true;
        }

        /// <summary>
        /// Ensure that the console window is big enough to house the game properly either by
        /// automatically sizing it or waiting for the user to resize it
        /// </summary>
        /// <param name="width">Required nubmer of columns</param>
        /// <param name="height">Required number of rows</param>
        private async Task EnsureWindowSize(int width, int height)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                _Output.SetWindowSize(width, height);

            //on mac/linux we can't automatically set the size so it has to be done by the user
            if (_Output.WindowWidth < width || _Output.WindowHeight < height)
                _Output.RenderGroupWithFlush($"Please enlarge the window to {width}x{height} minimum");

            //wait for the user to resize the window
            while (_Output.WindowWidth < width || _Output.WindowHeight < height)
                await Task.Delay(500);
        }
    }
}

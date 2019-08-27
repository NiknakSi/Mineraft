using System;
using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business
{
    /// <summary>
    /// The core of the application which handles user input and coordinates the game engine
    /// </summary>
    public class MineraftApplication
    {
        private const string TITLE = "MINERAFT";
        private const double DIFFICULTYFACTOR = 0.1; //bigger number mean more mines
        private const int STARTLIVES = 5;

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
            //init the game engine
            _GameEngine.Init(TITLE, DIFFICULTYFACTOR, STARTLIVES);

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
    }
}

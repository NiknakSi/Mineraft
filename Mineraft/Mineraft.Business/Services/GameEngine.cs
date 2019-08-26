using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business.Services
{
    public class GameEngine : IGameEngine
    {
        private readonly IOutputRenderer _Output;
        private readonly IBoard _Board;
        private readonly IPlayer _Player;

        private string _DesiredStartPosition = "";

        public Enums.GameState State { get; private set; }
        public Enums.Difficulty Difficulty { get; private set; }
        public int LivesRemaining => MineraftApplication.STARTLIVES - _Board.ExplodedMines;
        public int MoveCounter { get; private set; }

        public GameEngine(IOutputRenderer output, IBoard board, IPlayer player)
        {
            _Output = output;
            _Board = board;
            _Player = player;
        }

        public void SetDifficulty(Enums.Difficulty difficulty)
        {
            if (State != Enums.GameState.DifficultySelect)
                return;

            Difficulty = difficulty;
            State = Enums.GameState.StartPositionSelect;

            ConfigureBoard(difficulty);
            RenderToOutput();
        }

        /// <summary>
        /// Initialises the game board for the specified difficulty level
        /// </summary>
        private void ConfigureBoard(Enums.Difficulty difficulty)
        {
            int boardWidth, boardHeight;

            switch (difficulty)
            {
                case Enums.Difficulty.Medium:
                    boardWidth = boardHeight = 16;
                    break;
                case Enums.Difficulty.Hard:
                    boardWidth = boardHeight = 26;
                    break;
                default:
                    boardWidth = boardHeight = 8;
                    break;
            }

            _Board.Init(boardWidth, boardHeight);
        }

        public void SetDesiredStartPosition(int? positionInput)
        {
            if (State != Enums.GameState.StartPositionSelect)
                return;

            //use nulls to handle backspace in case of user input errors
            if (!positionInput.HasValue)
            {
                if (!string.IsNullOrEmpty(_DesiredStartPosition))
                    _DesiredStartPosition = _DesiredStartPosition.Remove(_DesiredStartPosition.Length - 1);
            }
            else
                _DesiredStartPosition += positionInput.Value.ToString();

            RenderToOutput();
        }

        public void ConfirmDesiredStartPosition()
        {
            if (State != Enums.GameState.StartPositionSelect)
                return;
            if (!int.TryParse(_DesiredStartPosition, out int position) || position <= 0 || position > _Board.Height)
                return;

            State = Enums.GameState.FreeMovement;
            _Player.UpdatePosition(-1, position - 1);
            _Player.OnTheBoard = TryMove(Enums.Direction.East);
        }

        public void Reset()
        {
            _Output.SetGameDefaultColors();
            State = Enums.GameState.DifficultySelect;
            _DesiredStartPosition = string.Empty;
            MoveCounter = 0;

            ConfigureBoard(Difficulty);
            _Player.UpdatePosition(-1, -1);
            _Player.OnTheBoard = false;
            RenderToOutput();
        }

        public bool TryMove(Enums.Direction direction)
        {
            if (State != Enums.GameState.FreeMovement)
                return false;

            bool makePermittedMove(int x, int y)
            {
                //if the square is already debris and the player is on the board then don't count this as a move, just get out early
                if (_Board.GetSquare(x, y) == Enums.SquareStatus.Debris && _Player.OnTheBoard)
                    return true; //permitted but not counted

                //attempt to move into the desired square
                var status = _Board.OccupySquare(x, y);
                bool result;

                if (status == Enums.SquareStatus.Safe)
                {
                    if (status == Enums.SquareStatus.Safe)
                        _Player.UpdatePosition(x, y);

                    result = true;
                }
                else if (status == Enums.SquareStatus.Debris)
                    result = _Player.OnTheBoard; //prevents a valid move occuring when the player hits debris from the start position
                else
                    result = false;

                if (status != Enums.SquareStatus.Debris)
                    MoveCounter += 1;

                return result;
            }

            bool permittedMove = false;

            //calculate and move to the new coordinates based on the direction of travel
            switch (direction)
            {
                case Enums.Direction.North:
                    if (_Player.PositionY < _Board.Height - 1)
                        permittedMove = makePermittedMove(_Player.PositionX, _Player.PositionY + 1);
                    break;
                case Enums.Direction.East:
                    if (_Player.PositionX < _Board.Width - 1)
                        permittedMove = makePermittedMove(_Player.PositionX + 1, _Player.PositionY);
                    break;
                case Enums.Direction.South:
                    if (_Player.PositionY > 0)
                        permittedMove = makePermittedMove(_Player.PositionX, _Player.PositionY - 1);
                    break;
                case Enums.Direction.West:
                    if (_Player.PositionX > 0)
                        permittedMove = makePermittedMove(_Player.PositionX - 1, _Player.PositionY);
                    break;
            }

            //deal with failed move situations
            if (!permittedMove && (LivesRemaining <= 0 || !_Player.OnTheBoard))
            {
                if (LivesRemaining <= 0)
                    State = Enums.GameState.Loser;
                if (!_Player.OnTheBoard)
                    State = Enums.GameState.StartPositionSelect;

                _DesiredStartPosition = string.Empty;
                _Player.UpdatePosition(-1, -1);
            }

            //maybe the player made it all the way across
            if (_Player.PositionX == _Board.Width - 1)
                State = Enums.GameState.Winner;

            RenderToOutput();
            return permittedMove;
        }

        public void RenderToOutput()
        {
            var gameLines = new List<List<Glyph>>();

            var gameTitle = $"{Utils.SquareToGraphic(Enums.SquareStatus.Mine)} Welcome to {MineraftApplication.TITLE} {Utils.SquareToGraphic(Enums.SquareStatus.Mine)}";
            var padding = (_Output.WindowWidth - gameTitle.Length) / 2;

            //insert the main heading
            gameLines.Insert(0, new List<Glyph>());
            gameLines.Insert(1, Utils.StringToLine(new StringBuilder().Append(' ', padding).Append(gameTitle).ToString(), ConsoleColor.Red));
            gameLines.Insert(2, Utils.StringToLine(new StringBuilder().Append(' ', padding - 1).Append('═', gameTitle.Length + 2).ToString(), ConsoleColor.Red));
            gameLines.Insert(3, new List<Glyph>());

            //generate the board
            var boardLines = _Board.Render(State == Enums.GameState.Loser || State == Enums.GameState.Winner);

            //generate the status/info lines to sit beside the board
            var info = new List<string>()
            {
                "Navigate from West to East using the arrow keys",
                "Watch out for the mines!",
                "",
                State == Enums.GameState.DifficultySelect ? "Select difficulty: [E]asy / [M]edium / [H]ard"
                    : State == Enums.GameState.StartPositionSelect ? "Enter start row number and press ENTER:"
                    : State == Enums.GameState.Loser || State == Enums.GameState.Winner ? "Press R to reset, ESC to quit"
                    : "",
                "",
                State == Enums.GameState.StartPositionSelect ? _DesiredStartPosition : "",
                State == Enums.GameState.Loser ? "GAME OVER"
                    : State == Enums.GameState.Winner ? $"CONGRATULATIONS, YOU MADE IT!"
                    : State == Enums.GameState.FreeMovement ? $"Position: {Utils.GetXLegendValue(_Player.PositionX)}{_Player.PositionY + 1}" : "",
                State != Enums.GameState.Loser && State != Enums.GameState.Winner ? $"Lives remaining: {LivesRemaining}" : "",
                State != Enums.GameState.DifficultySelect ? $"Moves made: {MoveCounter}" : ""
            };

            //board should be positioned to the right of the screen so the lhs has space for status and instructions
            var boardWidth = boardLines.First().Count;
            padding = (_Output.WindowWidth - boardWidth) - 10; //10 cols of safety margin

            //create the default padding used for board lines without any info/status text
            var defaultPadder = new StringBuilder().Append(' ', padding).ToString();
            var infoStartLine = (boardLines.Count - info.Count) / 2; //positions the info/status block roughly in the middle of the board height

            //merge our status/info lines with the board lines
            for (int i = 0; i < boardLines.Count - 1; i++)
            {
                var linePrepend = defaultPadder;

                //check we're in an appropriate place in the board lines array and build the status/info padded string
                if (i >= infoStartLine && i - infoStartLine < info.Count)
                    linePrepend = BuildBoardLinePrependString(info[i - infoStartLine], padding);

                //insert the status/info line into the list of chars for this board line
                boardLines[i].InsertRange(0, Utils.StringToLine(linePrepend));
            }

            //add the board into the overall output array
            gameLines.AddRange(boardLines);


            //clear the display and print out each line of the game
            _Output.Clear();

            foreach (var line in gameLines)
            {
                foreach (var chr in line)
                    _Output.RenderSingle(chr);

                _Output.Flush();
            }

            _Output.SetGameDefaultColors();
        }

        /// <summary>
        /// Takes a string and injects it into a quantity of whitespace
        /// </summary>
        /// <param name="text">Display text</param>
        /// <param name="overallLength">Overall length of the string once finished and padded with whitespace</param>
        /// <returns></returns>
        private string BuildBoardLinePrependString(string text, int overallLength)
        {
            var padding = (overallLength - text.Length) / 2;
            var prependString = new StringBuilder().Append(' ', padding).Append(text).Append(' ', padding).ToString();

            //in case we're off by a character due to int rounding
            while (prependString.Length > overallLength)
                prependString = prependString.Remove(overallLength);
            while (prependString.Length < overallLength)
                prependString += " ";

            return prependString;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Mineraft.Business.Services.Contracts;

namespace Mineraft.Business.Services
{
    public class Board : IBoard
    {
        private readonly IOutputRenderer _Output;
        private readonly IPlayer _Player;

        private Enums.SquareStatus[,] _Squares;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int ExplodedMines
        {
            get
            {
                //any square that has a status of debris represents an exploded mine
                return _Squares.Cast<Enums.SquareStatus>()
                    .Count(s => s == Enums.SquareStatus.Debris);
            }
        }

        public Board(IOutputRenderer output, IPlayer player)
        {
            _Output = output;
            _Player = player;
        }

        public void Init(int width, int height, double difficultyFactor)
        {
            if (width < 1 || width > 26)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be between between 1 and 26 inclusive");
            if (height < 1 || height > 26)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be between between 1 and 26 inclusive");
            if (difficultyFactor < 0 || difficultyFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(difficultyFactor), "DifficultyFactor must be a decimal between 0 and 1 inclusive");

            Width = width;
            Height = height;

            _Squares = new Enums.SquareStatus[width, height];

            var random = new Random();

            //add squares to the board, some of which are mines
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    _Squares[x, y] = random.NextDouble() < difficultyFactor
                        ? Enums.SquareStatus.Mine : Enums.SquareStatus.Safe;
        }

        public List<List<Glyph>> Render(bool showMines = false)
        {
            var boardLines = new List<List<Glyph>>();

            //construct the top and bottom legends and borders
            var yLegendPad = Height > 9 ? "   " : "  "; //when the height reaches double digits more padding is needed
            var xLegendLine = Utils.StringToLine(yLegendPad + "  ");
            var xLegendBorderCenter = new List<Glyph>();
            var xLegendBorderTop = Utils.StringToLine(yLegendPad + "┌──┐", ConsoleColor.Blue);
            var xLegendBorderBottom = Utils.StringToLine(yLegendPad + "└──┘", ConsoleColor.Blue);

            for (int i = 0; i < Width; i++)
            {
                //generate horizontal A/B/C etc legend markers and border, changing colour if player is on this column
                xLegendLine.AddRange(Utils.StringToLine(Utils.GetXLegendValue(i) + " ", _Player.PositionX == i ? ConsoleColor.Red : ConsoleColor.White));
                xLegendBorderCenter.AddRange(Utils.StringToLine("──", ConsoleColor.Blue));
            }

            //2 gets us into the ┌──┐ space
            xLegendBorderTop.InsertRange(yLegendPad.Length + 2, xLegendBorderCenter);
            xLegendBorderBottom.InsertRange(yLegendPad.Length + 2, xLegendBorderCenter);

            //add the top legend lines
            boardLines.Add(xLegendLine);
            boardLines.Add(xLegendBorderTop);

            //generate mine field lines remembering that we loop the rows first since we're printing glyphs left to right one line at a time
            for (int y = Height - 1; y >= 0; y--)
            {
                int lineNum = y + 1;
                string legendPad = Height > 9 && lineNum <= 9 ? " " : ""; //correct padding depending on if this is a double digit row

                //create a new line starting with the the legend, switching colours if the player is on this row
                var line = new List<Glyph>()
                {
                    new Glyph(legendPad + lineNum.ToString(), _Player.PositionY == y ? ConsoleColor.Red : ConsoleColor.White),
                    new Glyph(' '),
                    new Glyph('│', ConsoleColor.Blue),
                    new Glyph(' '),
                };

                //loop the columns
                for (int x = 0; x < Width; x++)
                {
                    //display the player icon or mine field square as necessary
                    if (_Player.PositionX == x && _Player.PositionY == y)
                        line.Add(new Glyph(_Output.EmojiSupport ? "⛵️" : "|>"));
                    else
                    {
                        //show a mine location only if required (e.g. game is over)
                        var square = GetSquare(x, y);
                        if (square == Enums.SquareStatus.Mine && !showMines)
                            square = Enums.SquareStatus.Safe;

                        line.Add(new Glyph(_Output.ConvertGameSquareToGlyphValue(square)));
                    }
                }

                //finish the line with the legend
                line.Add(new Glyph(' '));
                line.Add(new Glyph('│', ConsoleColor.Blue));
                line.Add(new Glyph(' '));
                line.Add(new Glyph(lineNum.ToString(), _Player.PositionY == y ? ConsoleColor.Red : ConsoleColor.White));

                boardLines.Add(line);
            }

            //add the bottom legend to the board
            boardLines.Add(xLegendBorderBottom);
            boardLines.Add(xLegendLine);

            return boardLines;
        }
        
        public Enums.SquareStatus OccupySquare(int x, int y)
        {
            if (x < 0 || x > Width - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "X must be a valid board column index");
            if (y < 0 || y > Height - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "Y must be a valid board row index");

            var status = GetSquare(x, y);

            //if the square is a mine update it to debris so we know the player hit it
            if (status == Enums.SquareStatus.Mine)
                _Squares[x, y] = Enums.SquareStatus.Debris;

            return status;
        }

        public Enums.SquareStatus GetSquare(int x, int y)
        {
            if (x < 0 || x > Width - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "X must be a valid board column index");
            if (y < 0 || y > Height - 1)
                throw new ArgumentOutOfRangeException(nameof(x), "Y must be a valid board row index");

            return _Squares[x, y];
        }
    }
}

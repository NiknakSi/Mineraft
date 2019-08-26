using System.Collections.Generic;

namespace Mineraft.Business.Services.Contracts
{
    /// <summary>
    /// Describes the game board where the mines live and the player navigates
    /// </summary>
    public interface IBoard
    {
        /// <summary>
        /// Number of grid squares for the board width
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Number of grid squares for the board width
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Number of mines that have been hit so far
        /// </summary>
        int ExplodedMines { get; }

        /// <summary>
        /// Builds a new gameboard for the specified size. Mines will be randomly placed.
        /// </summary>
        /// <param name="width">Number of grid squares for the board width</param>
        /// <param name="height">Number of grid squares for the board width</param>
        void Init(int width, int height);

        /// <summary>
        /// Genrates the lines which represent the board
        /// </summary>
        /// <param name="showMines">Enable to show mines on the board</param>
        List<List<Glyph>> Render(bool showMines = false);

        /// <summary>
        /// Attempt to occupy a square and return its new status
        /// </summary>
        /// <param name="x">X coordinate index</param>
        /// <param name="y">Y coordinate index</param>
        Enums.SquareStatus OccupySquare(int x, int y);

        /// <summary>
        /// Get the current status of a specific square
        /// </summary>
        /// <param name="x">X coordinate index</param>
        /// <param name="y">Y coordinate index</param>
        Enums.SquareStatus GetSquare(int x, int y);
    }
}

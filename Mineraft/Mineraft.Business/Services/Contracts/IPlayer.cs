namespace Mineraft.Business.Services.Contracts
{
    /// <summary>
    /// Describes the player which moves around the board
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Current X position index of the player
        /// </summary>
        int PositionX { get; }

        /// <summary>
        /// Current Y position index of the player
        /// </summary>
        int PositionY { get; }

        /// <summary>
        /// True when the player has managed to get onto the board after selecting a start position
        /// </summary>
        bool OnTheBoard { get; set; }

        /// <summary>
        /// Move the player to new coordinates
        /// </summary>
        /// <param name="x">X coordinate index</param>
        /// <param name="y">Y coordinate index</param>
        void UpdatePosition(int x, int y);
    }
}

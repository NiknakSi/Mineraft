namespace Mineraft.Business
{
    /// <summary>
    /// Enums used by the game
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Describes the current state of the game so the correct screen and logic can be used
        /// </summary>
        public enum GameState
        {
            DifficultySelect,
            StartPositionSelect,
            FreeMovement,
            Winner,
            Loser
        }

        /// <summary>
        /// Difficulty modes
        /// </summary>
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        /// <summary>
        /// Directions in which the player can move
        /// </summary>
        public enum Direction
        {
            North,
            East,
            South,
            West
        }

        /// <summary>
        /// The types of squares available on the board
        /// </summary>
        public enum SquareStatus
        {
            Safe,
            Mine,
            Debris
        }
    }
}

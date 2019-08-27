namespace Mineraft.Business.Services.Contracts
{
    /// <summary>
    /// Describes the core logic used to run the game
    /// </summary>
    public interface IGameEngine
    {
        /// <summary>
        /// The current state of the game
        /// </summary>
        Enums.GameState State { get; }

        /// <summary>
        /// The selected difficulty of the game
        /// </summary>
        Enums.Difficulty DifficultySelection { get; }

        /// <summary>
        /// The number of lives remaining for this game
        /// </summary>
        int LivesRemaining { get; }

        /// <summary>
        /// Register each legal game move
        /// </summary>
        int MoveCounter { get; }

        /// <summary>
        /// Initialise the game engine with some settings
        /// </summary>
        /// <param name="gameTitle">Can be used by the renderer and elsewhere</param>
        /// <param name="difficultyFactor">Used when the board generates the mine field</param>
        /// <param name="startLives">Number of lives the player should start each game with</param>
        void Init(string gameTitle, double difficultyFactor, int startLives);

        /// <summary>
        /// Resets the game state back to the difficulty selection screen
        /// </summary>
        void Reset();

        /// <summary>
        /// Set the game difficulty to a new level
        /// </summary>
        /// <param name="difficulty">Desired game difficulty level</param>
        void SetDifficulty(Enums.Difficulty difficulty);

        /// <summary>
        /// Set the desired start position for the player. This method builds a string which will be parsed as int.
        /// </summary>
        /// <param name="positionInput">Part of or the whole position value. Use null for backspace.</param>
        void SetDesiredStartPosition(int? positionInput);

        /// <summary>
        /// Ensures the current desired start position set via SetDesiredStartPosition is valid and moves game to next state
        /// </summary>
        void ConfirmDesiredStartPosition();

        /// <summary>
        /// Takes all of the game elements for the current state and generates and sends them to the output renderer
        /// </summary>
        void RenderToOutput();

        /// <summary>
        /// Tries to move the player in the specified direction, and returns whether this was successful or not
        /// </summary>
        /// <param name="direction">A north/east/south/west direction to move in</param>
        bool TryMove(Enums.Direction direction);
    }
}

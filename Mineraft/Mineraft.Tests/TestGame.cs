using Microsoft.Extensions.DependencyInjection;
using Mineraft.Business.Services;
using Mineraft.Business.Services.Contracts;
using Mineraft.Tests.Mocks;
using NUnit.Framework;

namespace Mineraft.Tests
{
    [TestFixture]
    public class TestGame
    {
        ServiceProvider _ServiceProvider;

        [SetUp]
        public void Setup()
        {
            _ServiceProvider = new ServiceCollection()
                .AddSingleton<IOutputRenderer, MockConsoleRenderer>()
                .AddSingleton<IPlayer, Player>()
                .AddSingleton<IBoard, Board>()
                .AddSingleton<IGameEngine, GameEngine>()
                .BuildServiceProvider();

            var consoleService = _ServiceProvider.GetService<IOutputRenderer>();
            consoleService.WindowWidth = 120;
            consoleService.WindowHeight = 35;
            consoleService.WindowTitle = "MINERAFT TEST";
        }

        [Test]
        public void TestGameReset()
        {
            var gameService = _ServiceProvider.GetService<IGameEngine>();

            gameService.Reset();

            Assert.AreEqual(Business.Enums.GameState.DifficultySelect, gameService.State, "Game state was not DifficultySelect after reset");
            Assert.AreEqual(0, gameService.MoveCounter, "Move counter was not 0 after reset");
        }

        [Test]
        public void TestDifficultySelect()
        {
            var gameService = _ServiceProvider.GetService<IGameEngine>();
            var boardService = _ServiceProvider.GetService<IBoard>();

            gameService.Reset();
            gameService.SetDifficulty(Business.Enums.Difficulty.Hard);

            Assert.AreEqual(Business.Enums.Difficulty.Hard, gameService.Difficulty, "Difficulty was not Hard after selecting difficulty");
            Assert.AreEqual(Business.Enums.GameState.StartPositionSelect, gameService.State, "Game state was not StartPositionSelect after selecting difficulty");
            Assert.AreEqual(26, boardService.Width, "Board width was not 26 after selecting Hard difficulty");
        }

        [Test]
        public void TestStartPositionSelect()
        {
            var gameService = _ServiceProvider.GetService<IGameEngine>();

            gameService.Reset();
            gameService.SetDifficulty(Business.Enums.Difficulty.Hard);
            gameService.SetDesiredStartPosition(1);
            gameService.ConfirmDesiredStartPosition();

            Assert.True(gameService.MoveCounter == 1, "Move counter was not 1 after start position select");
        }

        [TestCase(Business.Enums.Direction.North, ExpectedResult = true, Description = "Player moves North")]
        [TestCase(Business.Enums.Direction.East, ExpectedResult = true, Description = "Player moves East")]
        [TestCase(Business.Enums.Direction.South, ExpectedResult = true, Description = "Player moves South")]
        [TestCase(Business.Enums.Direction.West, ExpectedResult = true, Description = "Player moves West")]
        public bool TestPlayerMovement(Business.Enums.Direction direction)
        {
            var gameService = _ServiceProvider.GetService<IGameEngine>();
            var playerService = _ServiceProvider.GetService<IPlayer>();

            gameService.Reset();
            gameService.SetDifficulty(Business.Enums.Difficulty.Easy);

            int startPosition = 1;

            while (!playerService.OnTheBoard)
            {
                gameService.SetDesiredStartPosition(startPosition);
                gameService.ConfirmDesiredStartPosition();

                startPosition += 1;
            }

            //used to know if we hit a mine
            var onboardMoveCount = gameService.MoveCounter;

            switch (direction)
            {
                case Business.Enums.Direction.North:
                    gameService.TryMove(direction);
                    return playerService.PositionY == startPosition || gameService.MoveCounter > onboardMoveCount;
                case Business.Enums.Direction.East:
                    gameService.TryMove(direction);
                    return playerService.PositionX == 1 || gameService.MoveCounter > onboardMoveCount;
                case Business.Enums.Direction.South:
                    if (playerService.PositionY == 0)
                    {
                        gameService.TryMove(Business.Enums.Direction.North);
                        if (playerService.PositionY == 0)
                            throw new System.Exception("Blocked by mine");
                    }

                    onboardMoveCount = gameService.MoveCounter;
                    var positionY = playerService.PositionY;
                    gameService.TryMove(Business.Enums.Direction.South);
                    return playerService.PositionY == positionY - 1 || gameService.MoveCounter > onboardMoveCount;
                case Business.Enums.Direction.West:
                    gameService.TryMove(Business.Enums.Direction.East);
                    if (playerService.PositionX == 0)
                        throw new System.Exception("Blocked by mine");

                    onboardMoveCount = gameService.MoveCounter;
                    var positionX = playerService.PositionX;
                    gameService.TryMove(Business.Enums.Direction.West);
                    return playerService.PositionX == positionX - 1 || gameService.MoveCounter > onboardMoveCount;
            }

            return false;
        }
    }
}

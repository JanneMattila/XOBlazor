using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using XO.Tests.Stubs;

namespace XO.Tests
{
    [TestClass]
    public class GameEngineTests
    {
        private Board _board;
        private GameEngine _gameEngine;
        private PlayerStub _humanPlayerStub;
        private PlayerStub _computerPlayerStub;

        [TestInitialize]
        public void TestInitialize()
        {
            _board = new Board();
            _gameEngine = new GameEngine();
            _gameEngine.SetPlayers(string.Empty, string.Empty);
            _gameEngine.ExecuteDraw = (boardData) => { };

            _humanPlayerStub = new PlayerStub(isHuman: true);
            _computerPlayerStub = new PlayerStub(isHuman: false);
        }

        [TestMethod]
        public async Task Draw_is_Executed()
        {
            // Arrange
            var expected = true;
            var actual = false;

            _gameEngine.ExecuteDraw = (boardData) => actual = true;

            // Act
            await _gameEngine.CanvasClickAsync(column: 5, row: 5);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Human_move_is_made()
        {
            // Arrange
            var expected = 55;

            await _gameEngine.CanvasClickAsync(column: 5, row: 5);
            await _gameEngine.CanvasClickAsync(column: 5, row: 5);

            // Act
            var actual = _gameEngine._board.PreviousMove.ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Computer_move_is_made()
        {
            // Arrange
            var expected = 55;
            _computerPlayerStub.Move = Move.FromIndex(_gameEngine._board, 55);
            _gameEngine.SetPlayers(_computerPlayerStub, _humanPlayerStub);
            await Task.Delay(150);

            // Act
            var actual = _gameEngine._board.PreviousMove.ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

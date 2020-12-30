using Microsoft.VisualStudio.TestTools.UnitTesting;
using XO.ComputerPlayers;

namespace XO.Tests.ComputerPlayers
{
    [TestClass]
    public class NormalTests
    {
        private Normal _solver;
        private Board _board;

        [TestInitialize]
        public void TestInitialize()
        {
            _board = new Board();
            _solver = new Normal();
        }

        [TestMethod]
        public void Empty_Board_Move()
        {
            // Arrange
            var expected = 55;

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod]
        public void After_One_Center_Move()
        {
            // Arrange
            _board.MakeMove(5, 5);
            var expected = 65;

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using XO.ComputerPlayers;

namespace XO.Tests.ComputerPlayers
{
    [TestClass]
    public class BasicTests
    {
        private Basic _solver;
        private Board _board;

        [TestInitialize]
        public void TestInitialize()
        {
            _board = new Board();
            _solver = new Basic();
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

        [TestMethod]
        public void After_One_Center_Move()
        {
            // Arrange
            _board.MakeMove(5, 5);
            var expected = 44;

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Win_In_Single_Move()
        {
            // Arrange
            _board.MakeMove(5, 5);
            _board.MakeMove(8, 5);
            _board.MakeMove(5, 4);
            _board.MakeMove(0, 0);
            _board.MakeMove(5, 6);
            _board.MakeMove(0, 7);
            _board.MakeMove(5, 7);
            _board.MakeMove(5, 3);

            // Board before: Board after:
            // "o---------"  "o---------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "-----o----"  "-----o----"
            // "-----x----"  "-----x----"
            // "-----x--o-"  "-----x--o-"
            // "-----x----"  "-----x----"
            // "o----x----"  "o----x----"
            // "----------"  "-----X----"
            // "----------"  "----------"
            var expected = 85; // (5, 8)

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Lose_Unless_Defend_Move()
        {
            // Arrange
            _board.MakeMove(0, 0);
            _board.MakeMove(0, 1);
            _board.MakeMove(1, 2);
            _board.MakeMove(0, 2);
            _board.MakeMove(2, 6);
            _board.MakeMove(0, 3);
            _board.MakeMove(2, 7);
            _board.MakeMove(0, 4);

            // Board before: Board after:
            // "x---------"  "x---------"
            // "o---------"  "o---------"
            // "ox--------"  "ox--------"
            // "o---------"  "o---------"
            // "o---------"  "o---------"
            // "----------"  "X---------"
            // "--x-------"  "--x-------"
            // "--x-------"  "--x-------"
            // "----------"  "----------"
            // "----------"  "----------"
            var expected = 50; // (0, 5)

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Full_Circle_With_Position_In_The_Middle()
        {
            // Arrange
            _board.MakeMove(5, 5);
            _board.MakeMove(5, 6);
            _board.MakeMove(5, 7);
            _board.MakeMove(6, 5);
            _board.MakeMove(7, 5);
            _board.MakeMove(6, 7);
            _board.MakeMove(7, 7);
            _board.MakeMove(7, 6);

            // Board before: Board after:
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "-----xox--"  "-----xox--"
            // "-----o-o--"  "-----oXo--"
            // "-----xox--"  "-----xox--"
            // "----------"  "----------"
            // "----------"  "----------"
            var expected = 66; // (6, 6)

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Ignore]
        public void Other_Player_With_Three_In_Row()
        {
            // Arrange
            _board.MakeMove(5, 4); // x
            _board.MakeMove(4, 5); // o
            _board.MakeMove(3, 4); // x
            _board.MakeMove(5, 5); // o
            _board.MakeMove(4, 4); // x
            _board.MakeMove(6, 4); // o
            _board.MakeMove(5, 3); // x
            _board.MakeMove(4, 6); // o
            _board.MakeMove(3, 5); // x
            _board.MakeMove(6, 2); // o

            // Board before: Board after: 
            // "----------"  "----------"
            // "----------"  "----------"
            // "------o---"  "------o---"
            // "-----x----"  "-----x----"
            // "---xxxo---"  "--Xxxxo---"
            // "---xoo----"  "---xoo----"
            // "----o-----"  "----o-----"
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            var expected = 42; // (2, 4)

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, Ignore]
        public void Other_Player_With_Three_In_Row2()
        {
            // Arrange
            var moves = new int[] { 45, 44, 34, 35, 23, 56, 12, 1, 33, 53 };
            foreach (var move in moves)
            {
                _board.MakeMove(move);
            }

            // Board before: Board after: 
            // "-o--------"  "-o--------"
            // "--x-------"  "--x-------"
            // "---x------"  "---x--X---"
            // "---xxo----"  "---xxo----"
            // "----ox----"  "----ox----"
            // "---o--o---"  "---o--o---"
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            var expected = 62; // (2, 6)

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AI_Should_Block_Other_Player_Straight()
        {
            // Arrange
            var moves = new int[] { 45, 55, 44, 46, 64, 54, 53, 56, 57, 76, 66, 65, 43, 87 };
            foreach (var move in moves)
            {
                _board.MakeMove(move);
            }

            // Board before: Board after: 
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "----------"  "----------"
            // "---xxxo---"  "---xxxo---"
            // "---xooox--"  "---xooox--"
            // "----xox---"  "----xox---"
            // "------o---"  "------o---"
            // "-------o--"  "-------o--"
            // "----------"  "--------X-"
            var expected = 98;

            // Act
            var actual = _solver.MakeMove(_board).ToInt();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

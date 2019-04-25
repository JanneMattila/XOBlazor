using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XO.Tests
{
    [TestClass]
    public class BoardTests
    {
        private Board _board = new Board();

        [TestMethod]
        public void Default_Size()
        {
            // Arrange
            var expected = 10;
            var expectedState = BoardState.Running;
            var expectedSubState = BoardSubState.XTurn;

            // Act & Assert
            Assert.AreEqual(expected, _board.Width);
            Assert.AreEqual(expected, _board.Height);
            Assert.AreEqual(expectedState, _board.State);
            Assert.AreEqual(expectedSubState, _board.SubState);
        }

        [TestMethod]
        public void SetBoard_2x2()
        {
            // Arrange
            var expectedWidth = 2;
            var expectedHeight = 2;

            // Act
            _board.SetBoard($"--\nxo");

            // Act & Assert
            Assert.AreEqual(expectedWidth, _board.Width);
            Assert.AreEqual(expectedHeight, _board.Height);
        }

        [TestMethod]
        public void SetBoard_4x2()
        {
            // Arrange
            var expectedWidth = 4;
            var expectedHeight = 2;

            // Act
            _board.SetBoard($"--xo\nxo--");

            // Act & Assert
            Assert.AreEqual(expectedWidth, _board.Width);
            Assert.AreEqual(expectedHeight, _board.Height);
        }

        [TestMethod]
        public void GetBoard_4x2()
        {
            // Arrange
            var expected = $"--xo\nxo--";
            _board.SetBoard(expected);

            // Act
            var actual = _board.ToString();

            // Act & Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Undo_One_Level()
        {
            // Arrange
            var expectedPiece = Piece.Empty;
            var expectedState = BoardState.Running;
            var expectedSubState = BoardSubState.XTurn;
            var expectedMoveCount = 0;
            var column = 2;
            var row = 4;
            _board.MakeMove(column, row);

            // Act
            var undoMove = _board.Undo();

            // Act & Assert
            Assert.AreEqual(expectedPiece, _board.GetPiece(column, row));
            Assert.AreEqual(expectedState, _board.State);
            Assert.AreEqual(expectedSubState, _board.SubState);
            Assert.AreEqual(expectedMoveCount, _board.Count);
            Assert.AreEqual(column, undoMove?.Column);
            Assert.AreEqual(row, undoMove?.Row);
        }

        [TestMethod]
        public void Undo_Two_Levels()
        {
            // Arrange
            var expectedPiece = Piece.Empty;
            var move1 = new Move() { Column = 2, Row = 2 };
            var move2 = new Move() { Column = 2, Row = 3 };
            _board.MakeMove(move1);
            _board.MakeMove(move2);

            // Act
            var undoMove1 = _board.Undo();
            var undoMove2 = _board.Undo();

            // Act & Assert
            Assert.AreEqual(expectedPiece, _board.GetPiece(undoMove1));
            Assert.AreEqual(expectedPiece, _board.GetPiece(undoMove2));
        }

        [TestMethod]
        public void Evalue_X_Win()
        {
            // Arrange
            var expected = 5;
            var column = 1;
            var row = 0;
            _board.SetBoard($"-xxxxxo\n-------");

            // Act
            var actual = _board.EvaluationAggregate()[column, row];

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Evalue_O_Win()
        {
            // Arrange
            var expected = -5;
            var column = 4;
            var row = 0;
            _board.SetBoard($"-ooooox\n-------");

            // Act
            var actual = _board.EvaluationAggregate()[column, row];

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Player_Turn_After_One_Move()
        {
            // Arrange
            var expected = Player.O;
            _board.MakeMove(1, 1);

            // Act
            var actual = _board.CurrentPlayer;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Player_Turn_After_Two_Moves()
        {
            // Arrange
            var expected = Player.X;
            _board.MakeMove(1, 1);
            _board.MakeMove(2, 2);

            // Act
            var actual = _board.CurrentPlayer;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Validate_Move_List_As_Numbers()
        {
            // Arrange
            var expected = 12;
            _board.MakeMove(2, 1);

            // Act
            var actual = _board.GetMovesAsNumbers().First();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Validate_Move_List_Length()
        {
            // Arrange
            var expected = 2;
            _board.MakeMove(1, 1);
            _board.MakeMove(2, 2);

            // Act
            var actual = _board.GetMoves().Count();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, _board.Count);
        }

        [TestMethod]
        public void Validate_Move_List_Order()
        {
            // Arrange
            var expected = 3;
            _board.MakeMove(1, 1);
            _board.MakeMove(2, 2);
            _board.MakeMove(3, 3);

            // Act
            var actual = _board.GetMoves().Last();

            // Assert
            Assert.AreEqual(expected, actual.Column);
            Assert.AreEqual(expected, actual.Row);
            Assert.AreEqual(expected, _board.Count);
        }

        [TestMethod]
        public void Evalue_O_Win_After_Move()
        {
            // Arrange
            var expected = -5;
            var column = 3;
            var row = 0;
            _board.CurrentPlayer = Player.O;
            _board.SetBoard($"-oo-oox\n-------");

            // Act
            var actual = _board.MakeMove(column, row).Max();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Evalue_X_Cross()
        {
            // Arrange
            var expected = 4;
            var column = 1;
            var row = 1;
            _board.SetBoard(@"
-------
-x-----
--x----
---x---
----x--
-------");

            // Act
            var actual = _board.Evaluate(column, row).Max();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Default_Serialization_Validation()
        {
            // Arrange
            var expectedSize = "10x10";
            var expectedPlayer = 'x';
            var expectedMovesCount = 0;
            var expectedBoardLength = (10 * 10) + 9; // 10x10 and 9 line feeds

            // Act
            var actual = _board.Serialize();

            // Assert
            Assert.AreEqual(expectedSize, actual.Size);
            Assert.AreEqual(expectedPlayer, actual.CurrentPlayer);
            Assert.AreEqual(expectedMovesCount, actual.Moves.Count);
            Assert.AreEqual(expectedBoardLength, actual.Board.Length);
        }

        [TestMethod]
        public void Deserialize_5x6_Validation()
        {
            // Arrange
            var expectedWidth = 5;
            var expectedHeight = 6;
            var expectedPlayer = Player.O;
            var data = new BoardData()
            {
                Size = "5x6",
                FirstPlayer = 'o',
                Moves = new List<int>()
            };

            // Act
            _board.Deserialize(data);

            // Assert
            Assert.AreEqual(expectedWidth, _board.Width);
            Assert.AreEqual(expectedHeight, _board.Height);
            Assert.AreEqual(expectedPlayer, _board.CurrentPlayer);
        }

        [TestMethod]
        public void State_O_Turn_Validation()
        {
            // Arrange
            var expectedState = BoardState.Running;
            var expectedSubState = BoardSubState.OTurn;

            // Act
            _board.MakeMove(new Move() { Column = 1, Row = 1 });

            // Assert
            Assert.AreEqual(expectedState, _board.State);
            Assert.AreEqual(expectedSubState, _board.SubState);
        }

        [TestMethod]
        public void X_Won_State_Validation()
        {
            // Arrange
            var expectedState = BoardState.Finished;
            var expectedSubState = BoardSubState.XWon;

            // Act
            _board.MakeMove(new Move() { Column = 1, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 1, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 2, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 2, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 3, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 3, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 4, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 4, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 5, Row = 1 }); // X

            // Assert
            Assert.AreEqual(expectedState, _board.State);
            Assert.AreEqual(expectedSubState, _board.SubState);
        }

        [TestMethod]
        public void Finished_Game_Move_Prevention_Validation()
        {
            // Arrange
            // Act
            _board.MakeMove(new Move() { Column = 1, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 1, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 2, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 2, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 3, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 3, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 4, Row = 1 }); // X
            _board.MakeMove(new Move() { Column = 4, Row = 2 }); // O
            _board.MakeMove(new Move() { Column = 5, Row = 1 }); // X

            // Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _board.MakeMove(new Move() { Column = 5, Row = 2 }); // X
            });
        }

        [TestMethod]
        public void Deserialization_should_not_crash()
        {
            // Arrange
            var expectedWidth = 3;
            var expectedHeight = 4;
            var expectedPlayer = Player.X;
            var data = new BoardData()
            {
                Size = "3x4"
            };

            // Act
            _board.Deserialize(data);

            // Assert
            Assert.AreEqual(expectedWidth, _board.Width);
            Assert.AreEqual(expectedHeight, _board.Height);
            Assert.AreEqual(expectedPlayer, _board.CurrentPlayer);
        }

        [TestMethod]
        public void Evalue_X_After_Move()
        {
            // Arrange
            var expected = 6;
            var column = 3;
            var row = 0;
            _board.SetBoard($"-xx-xxx\n-------");

            // Act
            var actual = _board.MakeMove(column, row).Max();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Evalue_Left_Top_Corner_To_East()
        {
            // Arrange
            var expected = 5;
            var column = 3;
            var row = 0;
            _board.SetBoard(@"
xxxxx--
ox-----
o-o----
o--x---
o---x--
-------");

            // Act
            var actual = _board.Evaluate(column, row).Max();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Evalue_Left_Top_Corner_To_South()
        {
            // Arrange
            var expected = -5;
            var column = 0;
            var row = 1;
            _board.SetBoard(@"
oxxxx--
ox-----
o-o----
o--x---
o---x--
-------");

            // Act
            var actual = _board.Evaluate(column, row).Max();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Evalue_Left_Top_Corner_To_South_WinningMoves()
        {
            // Arrange
            var expected = 5;
            _board.SetBoard(@"
-xxxx--
ox-----
o-o----
o--x---
o---x--
-------");

            // Act
            var actual = _board.MakeMove(0).Max();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.IsNotNull(_board.WinningMoves);
        }

        [TestMethod]
        public void GetAvailableMovesInRadiusAsNumbers_For_Empty_Board()
        {
            // Arrange
            var index = 45; // Middle of board
            var radius = 1;
            var expected = 9;

            // Act
            var actual = _board.GetAvailableMovesInRadiusAsNumbers(index, radius).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAvailableMovesInRadiusAsNumbers_For_Corner_Northwest()
        {
            // Arrange
            var column = 0;
            var row = 0;
            var radius = 1;
            var expected = 4;

            // Act
            var actual = _board.GetAvailableMovesInRadiusAsNumbers(column, row, radius).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAvailableMovesInRadiusAsNumbers_For_Corner_Southwest()
        {
            // Arrange
            var column = 0;
            var row = 9;
            var radius = 1;
            var expected = 4;

            // Act
            var actual = _board.GetAvailableMovesInRadiusAsNumbers(column, row, radius).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAvailableMovesInRadiusAsNumbers_For_Corner_Northeast()
        {
            // Arrange
            var column = 9;
            var row = 0;
            var radius = 1;
            var expected = 4;

            // Act
            var actual = _board.GetAvailableMovesInRadiusAsNumbers(column, row, radius).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAvailableMovesInRadiusAsNumbers_For_Corner_Southeast()
        {
            // Arrange
            var column = 9;
            var row = 9;
            var radius = 1;
            var expected = 4;

            // Act
            var actual = _board.GetAvailableMovesInRadiusAsNumbers(column, row, radius).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAvailableMoves_For_Empty_Board()
        {
            // Arrange
            var expected = 100;

            // Act
            var actual = _board.GetAvailableMoves().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetAvailableMoves_After_One_Move()
        {
            // Arrange
            var expected = 99;
            _board.MakeMove(50);

            // Act
            var actual = _board.GetAvailableMoves().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Evaluation_When_Combining_Value_With_Opponent()
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
            var move = 66; // (6, 6)
            var expectedValuationValue = 16;

            // Act
            var evaluationResult = _board.MakeMove(move);

            // Assert
            Assert.AreEqual(expectedValuationValue, evaluationResult.Sum());
        }

        [TestMethod, Ignore]
        public void Evaluation_When_Next_To_Wall()
        {
            // TODO: Change evaluation to distinguish "open ended straight" vs. "close straight".
        }
    }
}

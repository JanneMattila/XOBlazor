using System;

namespace XO
{
    public class Move
    {
        private readonly Board _board;

        public Move()
        {
        }

        public Move(Board board)
        {
            _board = board;
        }

        public int Column { get; set; }

        public int Row { get; set; }

        public static Move FromCoordinates(Board board, int column, int row)
        {
            return new Move(board)
            {
                Column = column,
                Row = row
            };
        }

        public static Move FromIndex(Board board, int index)
        {
            return FromCoordinates(
                board,
                index % board.Width,
                index / board.Height);
        }

        public int ToInt(int boardWidth)
        {
            return Column + (Row * boardWidth);
        }

        public int ToInt(Board board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(
                    nameof(board),
                    "Cannot convert move to integer automatically if board width is not known.");
            }

            return ToInt(board.Width);
        }

        public int ToInt()
        {
            return ToInt(_board);
        }
    }
}

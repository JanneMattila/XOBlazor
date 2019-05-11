using System;

namespace XO.ComputerPlayers
{
    public class Randomizer : IPlayer
    {
        public bool IsHuman => false;

        private readonly Random _random = new Random();

        public Move MakeMove(Board board)
        {
            // Wildy search for *ANY* position on board and select that
            while (true)
            {
                var column = _random.Next(board.Width);
                var row = _random.Next(board.Height);

                if (board.IsAvailable(column, row))
                {
                    return Move.FromCoordinates(board, column, row);
                }
            }
        }
    }
}

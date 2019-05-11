using System;
using System.Linq;

namespace XO.ComputerPlayers
{
    public class LastMoveRandomizer : IPlayer
    {
        private readonly Random _random = new Random();

        public bool IsHuman => false;

        public Move MakeMove(Board board)
        {
            // Wildy search for *ANY* position on board and select that
            var lastMove = board.PreviousMove;
            if (lastMove == null)
            {
                // Make first move to the center.
                return Move.FromCoordinates(board, board.Width / 2, board.Height / 2);
            }

            var distance = 1;
            while (true)
            {
                var availableMoves = board.GetAvailableMovesInRadius(lastMove.Column, lastMove.Row, distance).ToList();
                if (availableMoves.Any())
                {
                    return availableMoves[_random.Next(availableMoves.Count)];
                }

                distance++;
            }
        }
    }
}

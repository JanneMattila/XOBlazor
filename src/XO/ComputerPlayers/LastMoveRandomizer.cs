using System;
using System.Linq;

namespace XO.ComputerPlayers
{
    public class LastMoveRandomizer : IPlayer
    {
        public bool IsHuman => false;

        private readonly Random _random = new Random();

        public Move MakeMove(Board board)
        {
            var lastMove = board.PreviousMove;
            if (lastMove == null)
            {
                // Make first move to the center.
                return Move.FromCoordinates(board, board.Width / 2, board.Height / 2);
            }

            // Search available moves nearby the previous move.
            Move move = null;
            for (int radius = 1; radius < board.Height; radius++)
            {
                var availableMoves = board.GetAvailableMovesInRadius(lastMove.Column, lastMove.Row, radius).ToList();
                if (availableMoves.Any())
                {
                    move = availableMoves[_random.Next(availableMoves.Count)];
                    break;
                }
            }
            return move;
        }
    }
}

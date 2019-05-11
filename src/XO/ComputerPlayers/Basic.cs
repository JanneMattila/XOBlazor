using System;
using System.Linq;

namespace XO.ComputerPlayers
{
    public class Basic : IPlayer
    {
        public bool IsHuman => false;

        private readonly Random _random = new Random();
        private Board _board;

        public Move MakeMove(Board board)
        {
            var lastMove = board.PreviousMove;
            if (lastMove == null)
            {
                // Make first move to the center.
                return Move.FromCoordinates(board, board.Width / 2, board.Height / 2);
            }

            _board = board;
            // Do I win in 1 move?
            Console.WriteLine("Do I win in 1 move?");
            var winMove = Solve(
                    currentLevel: 1,
                    targetLevel: 1,
                    playerTurn: true,
                    evaluateCurrent: null,
                    evaluateTarget: (evaluationResult, move, playerTurn) =>
                    {
                        return evaluationResult.HasWinningStraight();
                    }
                );
            if (winMove != null)
            {
                // Win in 1 move
                Console.WriteLine("Win in 1 move");
                return winMove;
            }

            // Do I lose if I do not make certain move?
            Console.WriteLine("Do I lose if I do not make certain move?");
            var defensiveMove = Solve(
                    currentLevel: 1,
                    targetLevel: 2,
                    playerTurn: true,
                    evaluateCurrent: (evaluationResult, move, playerTurn) =>
                    {
                        return false;
                    },
                    evaluateTarget: (evaluationResult, move, playerTurn) =>
                    {
                        return evaluationResult.Evaluations.Any(result => Math.Abs(result.Value) >= Board.StraightLength);
                    }
                );
            if (defensiveMove != null)
            {
                // Must defensive move (otherwise direct lose after our turn)
                return defensiveMove;
            }

            // Do I lose if I miss two moves?
            Console.WriteLine("Do I lose if I miss two moves?");
            var defensiveMove2 = Solve(
                    currentLevel: 1,
                    targetLevel: 3,
                    playerTurn: true,
                    evaluateCurrent: (evaluationResult, move, playerTurn) =>
                    {
                        return false;
                    },
                    evaluateTarget: (evaluationResult, move, playerTurn) =>
                    {
                        return evaluationResult.Evaluations.Any(result => Math.Abs(result.Value) >= Board.StraightLength);
                    }
                );
            if (defensiveMove2 != null)
            {
                // Must defensive move (otherwise direct lose after our turn)
                return defensiveMove2;
            }

            // Simple scan for best move.
            Console.WriteLine("Scan!");
            for (int targetLevel = 2; targetLevel < 4; targetLevel++)
            {
                Console.WriteLine($"Scan: {targetLevel}");
                int scanValue = 0;
                Move bestReturn = null;
                var scanReturn = Solve(
                        currentLevel: 1,
                        targetLevel: targetLevel,
                        playerTurn: true,
                        evaluateCurrent: (evaluationResult, move, playerTurn) =>
                        {
                            var evaluation = Evaluate(evaluationResult, playerTurn);
                            if (scanValue < evaluation)
                            {
                                scanValue = evaluation;
                                bestReturn = move;
                            }
                            return false;
                        },
                        evaluateTarget: (evaluationResult, move, playerTurn) =>
                        {
                            var evaluation = Evaluate(evaluationResult, playerTurn);
                            if (scanValue < evaluation)
                            {
                                scanValue = evaluation;
                                bestReturn = move;
                            }
                            return false;
                        }
                    );
                if (bestReturn != null)
                {
                    return bestReturn;
                }
            }

            // Panic - Let's take any valid move near previous move.
            Console.WriteLine("Searching any move as last last resort.");
            var previousMove = _board.PreviousMove;
            Move panicMove = null;
            for (int radius = 1; radius < _board.Height; radius++)
            {
                Console.WriteLine($"Search: {radius}");
                var move = _board.GetAvailableMovesInRadius(previousMove.Column, previousMove.Row, radius)
                    .FirstOrDefault();
                if (move != null)
                {
                    // Found nearby empty position.
                    panicMove = move;
                    break;
                }
            }
            return panicMove;
        }

        private Move Solve(
            int currentLevel,
            int targetLevel,
            Func<EvaluationResult, Move, bool, bool> evaluateCurrent,
            Func<EvaluationResult, Move, bool, bool> evaluateTarget,
            bool playerTurn)
        {
            var previousMove = _board.PreviousMove;
            var availableMoves = _board.GetAvailableMovesInRadius(previousMove.Column, previousMove.Row, targetLevel);
            foreach (var move in availableMoves)
            {
                var evaluationResult = _board.MakeMove(move);

                try
                {
                    if (currentLevel < targetLevel)
                    {
                        // Current level
                        if (evaluateCurrent(evaluationResult, move, playerTurn))
                        {
                            return move;
                        }

                        var subMove = Solve(currentLevel + 1, targetLevel, evaluateCurrent, evaluateTarget, !playerTurn);
                        if (subMove != null)
                        {
                            return subMove;
                        }
                    }
                    else
                    {
                        // Target level
                        if (evaluateTarget(evaluationResult, move, playerTurn))
                        {
                            return move;
                        }
                    }
                }
                finally
                {
                    _board.Undo();
                }
            }

            return null;
        }

        /// <summary>
        /// Board position evaluation function:
        /// 5 (or more): Infinite
        /// 4: 1000 *  count of fours in the row
        /// 3: 100 * count of three in the row
        /// 2: 10 * count of twos in the row
        /// 1: 1 * count of ones in the row
        /// </summary>
        /// <returns></returns>
        private int Evaluate(EvaluationResult evaluationResult, bool playerTurn)
        {
            if (evaluationResult.Evaluations.Count == 0)
            {
                return 0;
            }

            //var value = evaluationResult.Evaluations.Max(e => Math.Abs(e.Value));
            //var maxValue = evaluationResult.Evaluations.Max(e => e.Value);
            //var minValue = evaluationResult.Evaluations.Min(e => e.Value);
            var sum = evaluationResult.Evaluations.Sum(e => Math.Abs(e.Value));
            return sum;
        }
    }
}

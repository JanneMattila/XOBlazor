using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XO.ComputerPlayers
{
    public class Normal : IPlayer
    {
        public bool IsHuman => false;

        private readonly Random _random = new Random();
        private Board _board;

        public Move MakeMove(Board board)
        {
            var stopwatch = new Stopwatch();
            var lastMove = board.PreviousMove;
            if (lastMove == null)
            {
                // Make first move to the center.
                return Move.FromCoordinates(board, board.Width / 2, board.Height / 2);
            }

            _board = board;
            var availableMoves = _board.GetNearbyAvailableMoves();

            // Do I win in 1 move?
            stopwatch.Start();
            Console.WriteLine("Do I win in 1 move?");
            var winMove = Solve(
                    currentLevel: 1,
                    targetLevel: 1,
                    playerTurn: true,
                    availableMoves: availableMoves,
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

            Console.WriteLine($"No. It took {stopwatch.ElapsedMilliseconds} ms to figure it out.");
            var movesCount = _board.GetMoves().Count();
            // Do I lose if I do not make certain move?
            Console.WriteLine("Do I lose if I do not make certain move?");
            stopwatch.Restart();
            Move defensiveMove = null;
            if (movesCount > 7)
            {
                defensiveMove = Solve(
                    currentLevel: 1,
                    targetLevel: 2,
                    playerTurn: true,
                    availableMoves: availableMoves,
                    evaluateCurrent: (evaluationResult, move, playerTurn) =>
                    {
                        return false;
                    },
                    evaluateTarget: (evaluationResult, move, playerTurn) =>
                    {
                        return evaluationResult.Evaluations.Any(result => Math.Abs(result.Value) >= Board.StraightLength);
                    }
                );
            }
            if (defensiveMove != null)
            {
                // Must defensive move (otherwise direct lose after our turn)
                Console.WriteLine($"Defensive move (otherwise direct lose after our turn). It took {stopwatch.ElapsedMilliseconds} ms to figure it out.");
                return defensiveMove;
            }

            Console.WriteLine($"No. It took {stopwatch.ElapsedMilliseconds} ms to figure it out.");

            // Do I lose if I miss two moves?
            Console.WriteLine("Do I lose if I miss two moves?");
            stopwatch.Restart();

            Move defensiveMove2 = null;
            if (movesCount > 4)
            {
                defensiveMove2 = Solve(
                        currentLevel: 1,
                        targetLevel: 3,
                        playerTurn: true,
                        availableMoves: availableMoves,
                        evaluateCurrent: (evaluationResult, move, playerTurn) =>
                        {
                            return evaluationResult.Evaluations.Any(result => Math.Abs(result.Value) >= 4);
                        },
                        evaluateTarget: (evaluationResult, move, playerTurn) =>
                        {
                            return evaluationResult.Evaluations.Any(result => Math.Abs(result.Value) >= Board.StraightLength);
                        }
                    );
            }

            if (defensiveMove2 != null)
            {
                // Must defensive move (otherwise direct lose after our turn)
                Console.WriteLine($"Defensive move (otherwise direct lose after our turn). It took {stopwatch.ElapsedMilliseconds} ms to figure it out.");
                return defensiveMove2;
            }

            Console.WriteLine($"No. It took {stopwatch.ElapsedMilliseconds} ms to figure it out.");

            // Simple scan for best move.
            Console.WriteLine("Scan!");
            stopwatch.Restart();

            for (int targetLevel = 2; targetLevel < 4; targetLevel++)
            {
                Console.WriteLine($"Scan: {targetLevel}");
                int scanValue = 0;
                Move bestReturn = null;
                var scanReturn = Solve(
                        currentLevel: 1,
                        targetLevel: targetLevel,
                        playerTurn: true,
                        availableMoves: availableMoves,
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
                    Console.WriteLine($"Scan took {stopwatch.ElapsedMilliseconds} ms.");
                    return bestReturn;
                }
            }

            // Panic - Let's take any valid move near previous move.
            Console.WriteLine("Searching any move as last last resort.");
            stopwatch.Restart();

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
                    Console.WriteLine($"Search took {stopwatch.ElapsedMilliseconds} ms.");
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
            bool playerTurn,
            IEnumerable<Move> availableMoves)
        {
            foreach (var move in availableMoves)
            {
                if (_board.GetPiece(move) == Piece.Empty)
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

                            var subMove = Solve(currentLevel + 1, targetLevel, evaluateCurrent, evaluateTarget, !playerTurn, availableMoves);
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

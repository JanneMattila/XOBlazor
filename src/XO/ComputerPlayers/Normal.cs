using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace XO.ComputerPlayers
{
    public class Normal : IPlayer
    {
        class SolveData
        {
            public int Level { get; set; }
            public bool PlayerTurn { get; set; }
            public int Score { get; set; }
            public string Key { get; set; }
            public Board Board { get; set; }
            public List<Move> AvailableMoves { get; set; } = new();
        }

        public bool IsHuman => false;

        private readonly Random _random = new Random();

        public Move MakeMove(Board board)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var lastMove = board.PreviousMove;
            if (lastMove == null)
            {
                // Make first move to the center.
                return Move.FromCoordinates(board, board.Width / 2, board.Height / 2);
            }

            var solveFeed = new SolveData()
            {
                Level = 1,
                PlayerTurn = true,
                Board = board.Clone()
            };
            solveFeed.AvailableMoves.AddRange(board.GetNearbyAvailableMoves());

            var checkpoints = new List<SolveData>
            {
                solveFeed
            };

            var level = 1;
            Move selectedMove = null;
            while (stopwatch.ElapsedMilliseconds < 5_000)
            {
                var solve = checkpoints.FirstOrDefault(c => c.Level == level);
                if (solve == null)
                {
                    if (!checkpoints.Any()) break;

                    level++;

                    var beforeCount = checkpoints.Count;
                    checkpoints = checkpoints
                      .GroupBy(c => c.Key)
                      .Select(c => c.First())
                      .ToList();
                    var afterCount = checkpoints.Count;

                    Console.WriteLine($"Level: {level}, {beforeCount} -> {afterCount}, {stopwatch.Elapsed.TotalSeconds:0.0} seconds");
                    continue;
                }
                checkpoints.Remove(solve);

                foreach (var move in solve.AvailableMoves)
                {
                    var b = solve.Board.Clone();
                    var evaluationResult = b.MakeMove(move.ToInt());
                    var solve2 = new SolveData()
                    {
                        Level = solve.Level + 1,
                        PlayerTurn = !solve.PlayerTurn,
                        Key = b.ToString(),
                        Board = b
                    };

                    var availableMoves = solve.AvailableMoves.ToList();
                    availableMoves.Remove(move);

                    solve2.AvailableMoves.AddRange(availableMoves);
                    checkpoints.Add(solve2);
                }
            }

            Console.WriteLine($"After Level: {level} {stopwatch.Elapsed.TotalSeconds:0.0} seconds");

            if (selectedMove != null)
            {
                return selectedMove;
            }

            var previousMove = board.PreviousMove;
            Move panicMove = null;
            for (int radius = 1; radius < board.Height; radius++)
            {
                Console.WriteLine($"Search: {radius}");
                var move = board.GetAvailableMovesInRadius(previousMove.Column, previousMove.Row, radius)
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

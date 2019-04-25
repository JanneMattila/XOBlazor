using System;
using System.Collections.Generic;
using System.Linq;

namespace XO
{
    public class EvaluationResult
    {
        public EvaluationResult(Piece piece)
        {
            Piece = piece;
            Evaluations = new List<EvaluationValue>();
        }

        public Piece Piece { get; private set; }

        public List<EvaluationValue> Evaluations { get; private set; }

        public bool HasWinningStraight()
        {
            return Evaluations.Any(result => result.Value >= Board.StraightLength);
        }

        public IEnumerable<EvaluationValue> GetWinningStraights()
        {
            return Evaluations.Where(result => result.Value >= Board.StraightLength);
        }

        public int Max()
        {
            if (Evaluations.Count == 0)
            {
                return 0;
            }

            var value = Evaluations.Max(e => e.Value);
            return Piece == Piece.X ? value : -value;
        }

        public int Sum()
        {
            if (Evaluations.Count == 0)
            {
                return 0;
            }

            // Combines both players and opponents evaluations
            return Evaluations.Sum(e => Math.Abs(e.Value));
        }

        public int GetCountOfStraightLength(int minimunLength)
        {
            if (Evaluations.Count == 0)
            {
                return 0;
            }

            return Evaluations.Count(v => v.Value >= minimunLength);
        }
    }
}

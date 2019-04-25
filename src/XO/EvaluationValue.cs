using System.Collections.Generic;

namespace XO
{
    public class EvaluationValue
    {
        public EvaluationValue()
        {
            Value = 1;
            Indexes = new List<int>();
        }

        public int Value { get; set; }

        public List<int> Indexes { get; set; }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XO.Tests
{
    [TestClass]
    public class EvaluationResultTests
    {
        [TestMethod]
        public void Aggregation_Validation_For_X()
        {
            // Arrange
            var evaluationResult = new EvaluationResult(Piece.X);
            evaluationResult.Evaluations.Add(CreateEvaluationValue(1));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(3));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(2));
            var expected = 3;
            var expectedHasWinningStraight = false;

            // Act
            var actual = evaluationResult.Max();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedHasWinningStraight, evaluationResult.HasWinningStraight());
        }

        [TestMethod]
        public void Aggregation_Validation_For_O()
        {
            // Arrange
            var evaluationResult = new EvaluationResult(Piece.O);
            evaluationResult.Evaluations.Add(CreateEvaluationValue(1));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(3));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(2));
            var expected = -3;
            var expectedHasWinningStraight = false;

            // Act
            var actual = evaluationResult.Max();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedHasWinningStraight, evaluationResult.HasWinningStraight());
        }

        [TestMethod]
        public void GetCountOfStraightLength_Validation_For_X()
        {
            // Arrange
            var evaluationResult = new EvaluationResult(Piece.X);
            evaluationResult.Evaluations.Add(CreateEvaluationValue(1));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(1));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(1));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(3));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(3));
            evaluationResult.Evaluations.Add(CreateEvaluationValue(2));
            var expectedStraightOne = 6;
            var expectedStraightTwo = 3;
            var expectedStraightThree = 2;
            var expectedStraightOthers = 0;

            // Act & Assert
            Assert.AreEqual(expectedStraightOne, evaluationResult.GetCountOfStraightLength(1));
            Assert.AreEqual(expectedStraightTwo, evaluationResult.GetCountOfStraightLength(2));
            Assert.AreEqual(expectedStraightThree, evaluationResult.GetCountOfStraightLength(3));
            Assert.AreEqual(expectedStraightOthers, evaluationResult.GetCountOfStraightLength(4));
            Assert.AreEqual(expectedStraightOthers, evaluationResult.GetCountOfStraightLength(5));
        }

        private EvaluationValue CreateEvaluationValue(int value)
        {
            return new EvaluationValue()
            {
                Value = value
            };
        }
    }
}

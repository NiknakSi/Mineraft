using Mineraft.Business;
using NUnit.Framework;

namespace Mineraft.Tests
{
    [TestFixture]
    public class TestUtils
    {
        [TestCase(Enums.SquareStatus.Safe, ExpectedResult = "🌊", Description = "Safe square displays as wave")]
        [TestCase(Enums.SquareStatus.Mine, ExpectedResult = "💣", Description = "Mine square displays as bomb")]
        [TestCase(Enums.SquareStatus.Debris, ExpectedResult = "💥", Description = "Debris square displays as exposion")]
        public string CanConvertSquareToGraphic(Enums.SquareStatus squareStatus)
        {
            //Act
            return Utils.SquareToGraphic(squareStatus);
        }

        [Test]
        public void CanConvertStringToLine()
        {
            //Arrange
            const string inputString = "Hello world";

            //Act
            var output = Utils.StringToLine(inputString);

            //Assert
            Assert.AreEqual(inputString.Length, output.Count, "Length of converted line did not match length of input string");
        }

        [TestCase(0, ExpectedResult = "A", Description = "Position 0 converts to A")]
        [TestCase(1, ExpectedResult = "B", Description = "Position 0 converts to B")]
        [TestCase(2, ExpectedResult = "C", Description = "Position 0 converts to C")]
        public string CanGetXLegendValue(int index)
        {
            //Act
            return Utils.GetXLegendValue(index);
        }
    }
}
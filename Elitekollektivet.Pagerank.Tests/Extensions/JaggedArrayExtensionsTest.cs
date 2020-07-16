using Elitekollektivet.Pagerank.Extensions;
using Xunit;

namespace Elitekollektivet.Pagerank.Tests.Extensions
{
    public class JaggedArrayExtensionsTest
    {
        [Fact(DisplayName="ToMultidimensional_JaggedArray_TransormedCorrectly")]
        [Trait("Category", "Unit")]
        public void TestName()
        {
            var testData = new double[][]
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 }
            };

            var result = testData.ToMultidimensional();
            for (int i = 0; i < testData.Length; i++)
            {
                for (int j = 0; j < testData[0].Length; j++)
                {
                    Assert.Equal(result[i, j], testData[i][j]);
                }
            }
        }
    }
}
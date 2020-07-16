using Xunit;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Elitekollektivet.Pagerank.Tests
{
    public class PagerankTest
    {
        private PagerankBuilder _builder;

        public PagerankTest()
        {
            _builder = new PagerankBuilder();
        }

        [Fact(DisplayName="MakeStochastic_DenseWithZeroRows_EachRowShouldSumToOne")]
        [Trait("Category", "Unit")]
        public async Task MakeStochastic_DenseWithZeroRows_EachRowShouldSumToOne()
        {
            var dataset = new double[,] {
                { .3, .3, .3, .3 },
                {  0,  0,  0, 0  },
                { .5, .5,  0, 0  },
                { .25, .25,  0, 0  }
            };
            var pagerank = _builder.Build(new PagerankOptions{
                MakeStochastic = true
            });

            pagerank.SetLinkMatrix(dataset);
            await pagerank.MakeStochasticAsync();

            Parallel.ForEach(Enumerable.Range(0, pagerank.Size), i =>
            {
                Assert.Equal(1.0, pagerank.GetRow(i).Sum());
            });
        }

        [Fact(DisplayName="MakeStochastic_DenseWithANonZeroRow_EachCoefficientShouldBeTheSame")]
        [Trait("Category", "Unit")]
        public async Task MakeStochastic_DenseWithANonZeroRow_EachCoefficientShouldBeTheSame()
        {
            var dataset = new double[,] {
                { .5, .5,  0, 0  },
                { .5, .5,  0, 0  },
                { .5, .5,  0, 0  },
                { .5, .5,  0, 0  }
            };
            var pagerank = _builder.Build(new PagerankOptions{
                MakeStochastic = true
            });
            pagerank.SetLinkMatrix(dataset);
            await pagerank.MakeStochasticAsync();

            Assert.Equal(.5, pagerank.GetRow(0)[0]);
            Assert.Equal(.5, pagerank.GetRow(0)[1]);
            Assert.Equal(.0, pagerank.GetRow(0)[2]);
            Assert.Equal(.0, pagerank.GetRow(0)[3]);
        }

        [Fact(DisplayName="MakeIrreducible_DenseWithNonZeroRows_ShouldSubtractTheAlphaModifier")]
        [Trait("Category", "Unit")]
        public async Task MakeIrreducible_DenseWithNonZeroRows_ShouldSubtractTheAlphaModifier()
        {
            var dataset = new double[,] {
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 }
            };
            var pagerank = _builder.Build(new PagerankOptions{
                MakeStochastic = true,
                ConvergenceRate = .15
            });
            pagerank.SetLinkMatrix(dataset);
            await pagerank.MakeIrreducibleAsync();

            Parallel.ForEach(Enumerable.Range(0, pagerank.Size), i =>
            {
                Assert.Equal(1.0, pagerank.GetRow(i).Sum());
                Assert.Equal(0.5, pagerank.GetRow(i).ElementAt(0) + pagerank.GetRow(i).ElementAt(2));
                Assert.Equal(0.5, pagerank.GetRow(i).ElementAt(1) + pagerank.GetRow(i).ElementAt(3));
                Assert.True(pagerank.GetRow(i).ElementAt(0) < 0.5);
                Assert.True(pagerank.GetRow(i).ElementAt(1) < 0.5);
                Assert.True(pagerank.GetRow(i).ElementAt(2) > 0);
                Assert.True(pagerank.GetRow(i).ElementAt(3) > 0);
            });
        }

        [Fact(DisplayName="SetLinkMatrix_InvalidDense_ShouldRaiseArgumentException")]
        [Trait("Category", "Unit")]
        public void SetLinkMatrix_InvalidDense_ShouldRaiseArgumentException()
        {
            var dataset = new double[,] {
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 }
            };
            Assert.Throws<ArgumentException>(() => _builder.Build().SetLinkMatrix(dataset));
        }

        [Fact(DisplayName="SetLinkMatrix_ValidDense_ShouldSetDense")]
        [Trait("Category", "Unit")]
        public void SetLinkMatrix_ValidDense_ShouldSetDense()
        {
            var dataset = new double[,] {
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 },
                { .5, .5, 0, 0 }
            };
            
            var pagerank =_builder.Build();
            pagerank.SetLinkMatrix(dataset);
            var result = pagerank.ToArray();

            for (int i = 0; i < dataset.GetLowerBound(0); i++)
            {
                for(int j = 0; i < dataset.GetLowerBound(1); j++)
                {
                    Assert.Equal(dataset[i, j], result[i, j]);
                }
            }
        }

        [Fact(DisplayName="Run_ValidDense_ShouldRankLinks")]
        [Trait("Category", "Unit")]
        public async Task Run_ValidDense_ShouldRankLinks()
        {
            var dataset = new double[,] {
                { .5, .5,  0, 0 },
                { .5,  0, .5, 0 },
                { .5, .5,  0, 0 },
                { 1,   0,  0, 0 }
            };

            var pagerank = _builder.Build(new PagerankOptions
            {
                MakeIrreducible = true,
                MakeStochastic = true,
                ConvergenceRate = .5,
                Iterations = 200
            });
            pagerank.SetLinkMatrix(dataset);
            var result = await pagerank.RunAsync();

            Assert.True(result[0, 0] > result[1, 0] );
            Assert.True(result[0, 1] > result[2, 0] );
            Assert.True(result[0, 2] > result[3, 0] );
        }

        [Fact(DisplayName="SetLinkMatrix_NoMatrix_RaiseNullReferenceException")]
        [Trait("Category", "Unit")]
        public async Task SetLinkMatrix_NoMatrix_RaiseNullReferenceException()
        {
            var pagerank = _builder.Build(new PagerankOptions());
            await Assert.ThrowsAsync<NullReferenceException>(async () => await pagerank.RunAsync()) ;
        }
    }
}
using System;
using Xunit;

namespace Elitekollektivet.Pagerank.Tests
{
    public class PagerankBuilderTest
    {
        [Fact(DisplayName="Build_InvalidConvergenceRate_ThrowsArgumentException")]
        [Trait("Category", "Unit")]
        public void Build_InvalidConvergenceRate_ThrowsArgumentException()
        {
            var builder = new PagerankBuilder();
            builder.ConvergenceRate(100);
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Fact(DisplayName="Build_InvalidIterations_ThrowsArgumentException")]
        [Trait("Category", "Unit")]
        public void Build_InvalidIterations_ThrowsArgumentException()
        {
            var builder = new PagerankBuilder();
            builder.Iterations(-100);
            Assert.Throws<ArgumentException>(() => builder.Build());
        }

        [Fact(DisplayName="Build_NoConfiguration_DefaultIteration")]
        [Trait("Category", "Unit")]
        public void Build_NoConfiguration_DefaultIteration()
        {
            var builder = new PagerankBuilder();
            Assert.Equal(builder.Options.Iterations, builder.DefaultIterations);
        }

        [Fact(DisplayName="Build_Setters_SetValueIsCorrect")]
        [Trait("Category", "Unit")]
        public void Build_Setters_SetValueIsCorrect()
        {
            var builder = new PagerankBuilder();
            builder.ConvergenceRate(.5);
            Assert.Equal(.5, builder.Options.ConvergenceRate);

            builder.Iterations(100);
            Assert.Equal(100, builder.Options.Iterations);

            builder.MakeIrreducible(true);
            Assert.True(builder.Options.MakeIrreducible);

            builder.MakeStochastic(true);
            Assert.True(builder.Options.MakeStochastic);
        }
    }
}
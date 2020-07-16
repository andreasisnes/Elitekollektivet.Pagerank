using System;
using CsvHelper.Configuration;
using Elitekollektivet.Pagerank.Interfaces;

namespace Elitekollektivet.Pagerank
{
    public sealed class PagerankBuilder
    {
        public readonly int DefaultIterations = 100;

        public PagerankOptions Options { get {return _options; } }

        private PagerankOptions _options;

        public PagerankBuilder()
            : this(new PagerankOptions())
        {
        }

        public PagerankBuilder(PagerankOptions options)
        {
            _options = options;
            if (_options.Iterations == 0)
            {
                _options.Iterations = DefaultIterations;
            }
        }

        public PagerankBuilder Iterations(int iterations)
        {
            _options.Iterations = iterations;
            return this;
        }

        public PagerankBuilder MakeStochastic(bool makeStochastic)
        {
            _options.MakeStochastic = makeStochastic;
            return this;
        }

        public PagerankBuilder MakeIrreducible(bool makeIrreducible)
        {
            _options.MakeIrreducible = makeIrreducible;
            return this;
        }

        public PagerankBuilder ConvergenceRate(double convergenceRate)
        {
            _options.ConvergenceRate = convergenceRate;
            return this;
        }

        public IPagerank Build()
        {
            if (_options.ConvergenceRate < 0 || _options.ConvergenceRate > 1 )
            {
                throw new ArgumentException($"argument Convergence rate x={_options.ConvergenceRate} must be 0 <= x <= 1.");
            }
            if (_options.Iterations < 0)
            {
                throw new ArgumentException($"argument iterations x={_options.Iterations} must be a postitive integer");
            }
            return new Pagerank(_options);
        }

        public IPagerank Build(PagerankOptions options)
        {
            return new PagerankBuilder(options).Build();
        }
    }
}
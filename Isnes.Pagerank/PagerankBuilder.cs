using System;
using Isnes.Pagerank.Interfaces;

namespace Isnes.Pagerank
{
    public sealed class PagerankBuilder
    {
        private readonly PagerankOptions _options;

        public PagerankBuilder()
            : this(new PagerankOptions())
        {
        }

        public PagerankBuilder(PagerankOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.Iterations == 0)
            {
                _options.Iterations = DefaultIterations;
            }

            if (_options.ConvergenceRate == null)
            {
                _options.ConvergenceRate = 1.0;
            }
        }

        public static int DefaultIterations
        {
            get { return 100; }
        }

        public PagerankOptions Options
        {
            get { return _options; }
        }

        public static IPagerank Build(PagerankOptions options)
        {
            return new PagerankBuilder(options).Build();
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

        public PagerankBuilder ConvergenceRate(double convergenceRate)
        {
            _options.ConvergenceRate = convergenceRate;
            return this;
        }

        public IPagerank Build()
        {
            if (_options.ConvergenceRate < 0 || _options.ConvergenceRate > 1)
            {
                throw new ArgumentException($"argument Convergence rate x={_options.ConvergenceRate} must be 0 <= x <= 1.");
            }

            if (_options.Iterations < 0)
            {
                throw new ArgumentException($"argument iterations x={_options.Iterations} must be a postitive integer");
            }

            return new Pagerank(_options);
        }
    }
}
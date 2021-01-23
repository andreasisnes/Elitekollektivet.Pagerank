using System;
using System.Linq;
using System.Threading.Tasks;
using Isnes.Pagerank.Extensions;
using Isnes.Pagerank.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Isnes.Pagerank
{
    internal sealed class Pagerank : IPagerank
    {
        private PagerankOptions _options;

        private Matrix<double> dense;

        private bool _madeStochastic;

        private bool _madeIrreducible;

        private object _mutex;

        public Pagerank(PagerankOptions options)
        {
            _mutex = new object();
            _options = options;
            _madeIrreducible = false;
            _madeStochastic = false;
        }

        public int Size
        {
            get { return Dense.RowCount; }
        }

        private Matrix<double> Dense
        {
            get { return dense ?? throw new NullReferenceException("No link-matrix is set"); }
            set { dense = value; }
        }

        public IPagerank SetLinkMatrix(double[][] linkMatrix)
        {
            return SetLinkMatrix(linkMatrix.ToMultidimensional());
        }

        public IPagerank SetLinkMatrix(double[,] linkMatrix)
        {
            lock (_mutex)
            {
                _madeIrreducible = false;
                _madeStochastic = false;
                Dense = DenseMatrix.OfArray(linkMatrix);
                if (Dense.ColumnCount != Dense.RowCount)
                {
                    throw new ArgumentException($"{linkMatrix} is not a square matrix");
                }

                return this;
            }
        }

        public Task<IPagerank> MakeStochasticAsync()
        {
            return Task.Run(() => MakeStochastic());
        }

        public IPagerank MakeStochastic()
        {
            lock (_mutex)
            {
                if (!_madeStochastic)
                {
                    Parallel.ForEach(Enumerable.Range(0, Size), i =>
                    {
                        if (Dense.Row(i).Sum() <= 0.0)
                        {
                            Dense.SetRow(i, DenseVector.Create(Size, 1.0 / Size));
                        }
                        else
                        {
                            Dense.SetRow(i, Dense.Row(i).Divide(Dense.Row(i).Sum()));
                        }
                    });
                }

                _madeStochastic = true;
                return this;
            }
        }

        public Task<IPagerank> MakeIrreducibleAsync()
        {
            return Task.Run(() => MakeIrreducible());
        }

        public IPagerank MakeIrreducible()
        {
            lock (_mutex)
            {
                if (!_madeIrreducible)
                {
                    Dense = Dense.Multiply(_options.ConvergenceRate).Add(DenseMatrix.Create(Size, Size, (1.0 / Size) * (1.0 - _options.ConvergenceRate)));
                }

                _madeIrreducible = true;
                return this;
            }
        }

        public Task<double[,]> RunAsync()
        {
            return Task.Run(() => Execute(_options.Iterations));
        }

        public Task<double[,]> RunAsync(int iterations)
        {
            return Task.Run(() => Execute(iterations));
        }

        public double[,] Run(int iterations)
        {
            return Execute(iterations);
        }

        public double[,] Run()
        {
            return Execute(_options.Iterations);
        }

        public double[,] ToArray()
        {
            return Dense.ToArray();
        }

        public double[] GetRow(int i)
        {
            return Dense.Row(i).ToArray();
        }

        public double[] GetColumn(int j)
        {
            return Dense.Column(j).ToArray();
        }

        private double[,] Execute(int iterations)
        {
            lock (_mutex)
            {
                if (!_madeStochastic && _options.MakeStochastic)
                {
                    MakeStochastic();
                }

                if (!_madeIrreducible && _options.MakeIrreducible)
                {
                    MakeIrreducible();
                }

                return Dense.Transpose().Power(iterations).ToArray();
            }
        }
    }
}

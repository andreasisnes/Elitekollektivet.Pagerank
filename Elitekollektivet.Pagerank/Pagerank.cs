using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Elitekollektivet.Pagerank.Extensions;
using Elitekollektivet.Pagerank.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Elitekollektivet.Pagerank
{
    sealed internal class Pagerank : IPagerank
    {
        private PagerankOptions _options;

        private Matrix<double> dense;

        private Matrix<double> _dense
        {
            get { return dense ?? throw new NullReferenceException("No link-matrix is set"); }
            set { dense = value; }
        }

        private bool _madeStochastic;

        private bool _madeIrreducible;

        private object _mutex;

        public int Size { get {return _dense.RowCount; } }

        public Pagerank(PagerankOptions options)
        {
            _mutex = new object();
            _options = options;
            _madeIrreducible = false;
            _madeStochastic = false;
        }

        public IPagerank SetLinkMatrix(double [][] linkMatrix)
        {
            return SetLinkMatrix(linkMatrix.ToMultidimensional());
        }

        public IPagerank SetLinkMatrix(double [,] linkMatrix)
        {
            lock(_mutex)
            {
                _madeIrreducible = false;
                _madeStochastic = false;
                _dense = DenseMatrix.OfArray(linkMatrix);
                if(_dense.ColumnCount != _dense.RowCount)
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
            lock(_mutex)
            {
                if(!_madeStochastic)
                {
                    Parallel.ForEach(Enumerable.Range(0, Size), i =>
                    {
                        if (_dense.Row(i).Sum() <= 0.0)
                        {
                            _dense.SetRow(i, DenseVector.Create(Size, 1.0 / Size));
                        }
                        else
                        {
                            _dense.SetRow(i, _dense.Row(i).Divide(_dense.Row(i).Sum()));
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
            lock(_mutex)
            {
                if (!_madeIrreducible)
                {
                    _dense = _dense.Multiply(_options.ConvergenceRate).Add(
                        DenseMatrix.Create(Size, Size, (1.0 / Size) * (1.0 - _options.ConvergenceRate))
                    );
                }
                _madeIrreducible = true;
                return this;
            }
        }

        public Task<double[,]> RunAsync()
        {
            return Task.Run(() => _Run(_options.Iterations));
        }

        public Task<double[,]> RunAsync(int iterations)
        {
            return Task.Run(() => _Run(iterations));
        }

        public double[,] Run(int iterations)
        {
            return _Run(iterations);
        }

        public double[,] Run()
        {
            return _Run(_options.Iterations);
        }

        private double[,] _Run(int iterations)
        {
            lock(_mutex)
            {
                if (!_madeStochastic && _options.MakeStochastic)
                {
                    MakeStochastic();
                }
                if (!_madeIrreducible && _options.MakeIrreducible)
                {
                    MakeIrreducible();
                }
                return _dense.Transpose().Power(_options.Iterations).ToArray();
            }
        }

        public double[,] ToArray()
        {
            return _dense.ToArray();
        }

        public double[] GetRow(int i)
        {
            return _dense.Row(i).ToArray();
        }

        public double[] GetColumn(int j) 
        {
            return _dense.Column(j).ToArray();
        }
    }
}

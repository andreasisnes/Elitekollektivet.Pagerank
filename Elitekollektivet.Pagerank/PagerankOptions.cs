namespace Elitekollektivet.Pagerank
{
    public sealed class PagerankOptions
    {
        public int Iterations { get; set; }

        public bool MakeStochastic { get; set; }

        public bool MakeIrreducible { get; set; }

        public double ConvergenceRate { get; set; }
    }
}
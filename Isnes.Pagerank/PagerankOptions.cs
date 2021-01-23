namespace Isnes.Pagerank
{
    public sealed class PagerankOptions
    {
        public int Iterations { get; set; }

        public bool MakeStochastic { get; set; }

        public double? ConvergenceRate { get; set; }
    }
}
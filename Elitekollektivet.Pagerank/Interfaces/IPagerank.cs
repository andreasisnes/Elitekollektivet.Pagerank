using System.Threading.Tasks;

namespace Elitekollektivet.Pagerank.Interfaces
{
    public interface IPagerank
    {
        int Size { get; }

        IPagerank SetLinkMatrix(double [,] linkMatrix);

        IPagerank SetLinkMatrix(double [][] linkMatrix);

        Task<IPagerank> MakeStochasticAsync();

        IPagerank MakeStochastic(); 

        Task<IPagerank> MakeIrreducibleAsync();

        IPagerank MakeIrreducible();

        Task<double[,]> RunAsync();

        Task<double[,]> RunAsync(int iterations);

        double[,] Run(int iterations);

        double[,] Run();

        double[,] ToArray();

        double[] GetRow(int i);

        double[] GetColumn(int j);
    }
}
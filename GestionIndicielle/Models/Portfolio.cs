using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionIndicielle.Models
{
    class Portfolio
    {
        public Matrice PortfolioMatrix;
        public double[,] BenchmarkMatrix;
        public double[,] BenchmarkRendMatrix;
        public Portfolio(double[,] portMat, double[,] benchMat)
        {
            PortfolioMatrix = new Matrice(portMat);
            BenchmarkMatrix = benchMat;
            BenchmarkRendMatrix = Matrice.computeRMatrix(BenchmarkMatrix);
        }



        public void rebalancement(double[,] rebMat)
        {
            PortfolioMatrix.Mat = rebMat;
        }

        public double computeTrackingErrorExPost()
        {
            return 0;
        }
    }
}

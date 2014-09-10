using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionIndicielle.Models
{
    class Portfolio
    {
        public Matrix PortfolioMatrix;
        public Matrix BenchmarkMatrix;

        public Portfolio(double[,] portMat, double[,] benchMat)
        {
            PortfolioMatrix = new Matrix(portMat);
            BenchmarkMatrix = new Matrix(benchMat);
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

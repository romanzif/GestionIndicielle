using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionIndicielle.Models
{
    class Portfolio
    {
        public double BudgetInit;
        public Matrice PortfolioMatrix;
        public double[] NbAssetsHeld;
        public double[,] BenchmarkMatrix;
        public double[,] BenchmarkRendMatrix;
        public Portfolio(double[,] portMat, double[,] benchMat, double budgetInit)
        {
            PortfolioMatrix = new Matrice(portMat,benchMat);
            BenchmarkMatrix = benchMat;
            BenchmarkRendMatrix = Matrice.computeRMatrix(BenchmarkMatrix);
            BudgetInit = budgetInit;
            NbAssetsHeld = new double[PortfolioMatrix.Mat.GetLength(1)];
            for (int i=0;i<NbAssetsHeld.Length;i++)
            {
                // nbactions(i) = budget * poidsaffecte / prix de l'action
                NbAssetsHeld[i] = BudgetInit*PortfolioMatrix.WeightsVect[i]/portMat[portMat.GetLength(0) - 1, i];
            }
        }


        public double[,] computePortfolio(double[,] assetsPrices, double[] weights)
        {
            int dataSize = assetsPrices.GetLength(0);
            int nbAssets = assetsPrices.GetLength(1);
            var res = new double[dataSize, nbAssets];
            for (int j = 0; j < nbAssets; j++)
            {
                for (int i = 0; i < dataSize; i++)
                {
                    res[i, j] = assetsPrices[i, j] * weights[j];
                }
            }
            return res;
        }



        public void computePourtfolioRebalancement(double[,] rebMat)
        {
            PortfolioMatrix.Mat = rebMat;
            for (int i = 0; i < NbAssetsHeld.Length; i++)
            {
                // nbactions(i) = budget * poidsaffecte / prix de l'action
                NbAssetsHeld[i] = BudgetInit * PortfolioMatrix.WeightsVect[i] / rebMat[rebMat.GetLength(0) - 1, i];
            }
        }
    }
}

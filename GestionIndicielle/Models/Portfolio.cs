using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionIndicielle.Models
{
    public class Portfolio
    {
        public double BudgetInit;
        public Matrice PortfolioMatrix;
        public double[] NbAssetsHeld;
        public Portfolio(double[,] rebMat, double[,] estMat, double[,] benchMat, double budgetInit)
        {
            PortfolioMatrix = new Matrice(estMat,benchMat);
            BudgetInit = budgetInit;
            NbAssetsHeld = new double[PortfolioMatrix.Mat.GetLength(1)];
            double[] firstAssetsPrices = new double[PortfolioMatrix.Mat.GetLength(1)];
            for (int i = 0; i < PortfolioMatrix.Mat.GetLength(1); i++)
            {
                firstAssetsPrices[i] = rebMat[0, i];
            }
            for (int i=0;i<NbAssetsHeld.Length;i++)
            {
                // nbactions(i) = budget * poidsaffecte / prix de l'action
                NbAssetsHeld[i] = BudgetInit*PortfolioMatrix.WeightsVect[i]/firstAssetsPrices[i];
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
    }
}

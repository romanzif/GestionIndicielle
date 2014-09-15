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
        public double[] PortfolioValues;
        public double RelativeTargetReturn;
        public Portfolio(double[,] rebMat, double[,] estMat, double[,] benchMat, double budgetInit, double RTR,int index)
        {
            PortfolioMatrix = new Matrice(estMat, benchMat, RTR,index);
            RelativeTargetReturn = RTR;
            BudgetInit = budgetInit;
            NbAssetsHeld = new double[PortfolioMatrix.Mat.GetLength(1)];
            double[] firstAssetsPrices = new double[PortfolioMatrix.Mat.GetLength(1)];
            for (int i = 0; i < PortfolioMatrix.Mat.GetLength(1); i++)
            {
                firstAssetsPrices[i] = estMat[estMat.GetLength(0)-1, i];
            }
            for (int i=0;i<NbAssetsHeld.Length;i++)
            {
                // nbactions(i) = budget * poidsaffecte / prix de l'action
                NbAssetsHeld[i] = BudgetInit*PortfolioMatrix.WeightsVect[i]/firstAssetsPrices[i];
            }
            PortfolioValues = new double[rebMat.GetLength(0)];
            for (int i = 0; i < rebMat.GetLength(0); i++)
            {
                double value = 0;
                for (int j = 0; j < NbAssetsHeld.Length; j++)
                {
                    value += NbAssetsHeld[j]*rebMat[i, j];
                }
                PortfolioValues[i] = value;
            }
        }
    }
}

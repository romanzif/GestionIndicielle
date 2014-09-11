using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionIndicielle.Models;

namespace GestionIndicielle.Parser
{
    class FormatMatrix
    {
        public List<double[,]> EstimationMatrixList;
        public List<double[,]> RebalencementMatrixList;

        

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


        public void ComputeEstimationRebalencementCycle(double[,] assetsPrices1, double[,] assetsPrices2,
            double[,] benchmark1, double[,] benchmark2)
        {
            var port = new Portfolio(assetsPrices1, benchmark1);

            //appel ComputeEstimationPortfolio et ComputeRebalencement
        }
        //public double[,] ComputeEstimationPortfolio(double[,] assetsPrices, double[,] benchmark)
        //{

        //    var port = new Portfolio(assetsPrices, benchmark);

        //}

        public void ComputeRebalencement(double[,] assetsPrices, double[,] benchmark)
        {


        }
    }
}

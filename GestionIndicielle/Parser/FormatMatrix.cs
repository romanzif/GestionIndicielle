using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionIndicielle.Models;

namespace GestionIndicielle.Parser
{
    public class FormatMatrix
    {
        private double[,] _fullDataMatrix { get; set; }
        private int _estimationWindow { get; set; }
        private int _rebalancementPeriode { get; set; }
        public List<double[,]> EstimationMatrixList { get; set; }
        public List<double[,]> RebalancementMatrixList { get; set; }

        public FormatMatrix(double[,] initMatrix, int estimationWindow, int rebalancementPeriod)
        {
            _fullDataMatrix = initMatrix;
            _estimationWindow = estimationWindow;
            _rebalancementPeriode = rebalancementPeriod;
            EstimationMatrixList = new List<double[,]>();
            RebalancementMatrixList = new List<double[,]>();
            generateEstimationWindowList();
            generateRebalencementList();
        }

        public double[,] generateBabyMatrixByCopyPasteOfLines(int firstIndex, int lastIndex)
        {
            int nbAssets = _fullDataMatrix.GetLength(1);
            var res = new double[lastIndex - firstIndex, nbAssets];
            for (int i = firstIndex; i < lastIndex; i++)
            {
                for (int j = 0; j < nbAssets; j++)
                {
                    res[i-firstIndex, j] = _fullDataMatrix[i, j];
                }
            }
            return res;
        }

        public void generateEstimationWindowList()
        {
            int index = _estimationWindow;
            while (index < _fullDataMatrix.GetLength(0))
            {
                EstimationMatrixList.Add(generateBabyMatrixByCopyPasteOfLines(index-_estimationWindow,index));
                index += _rebalancementPeriode;
            }
        }

        public void generateRebalencementList()
        {
            int index = _estimationWindow;
            while (index + _rebalancementPeriode < _fullDataMatrix.GetLength(0))
            {
                RebalancementMatrixList.Add(generateBabyMatrixByCopyPasteOfLines(index,index+_rebalancementPeriode));
                index += _rebalancementPeriode;
            }
        }

    }
}

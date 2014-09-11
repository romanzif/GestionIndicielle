using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionIndicielle.Models
{
    public class ErrorRatios
    {
        public static double[,] ComputeDeltaRend(double[,] rendPort, double[,] rendBench)
        {
            var deltaRend = new double[rendPort.GetLength(0), 1];
            for (int i = 0; i < deltaRend.GetLength(0); i++)
            {
                deltaRend[i, 0] = rendPort[i, 0] - rendBench[i, 0];
            }
            return deltaRend;
        }

        public static double ComputeTrackingErrorExPost(double[,] rendPort, double[,] rendBench)
        {
            double[,] deltaRend = ComputeDeltaRend(rendPort, rendBench);
            double[,] resTemp = Matrice.computeCovarianceMatrix(deltaRend);
            double res = resTemp[0, 0];
            res = Math.Sqrt(res);
            return res;
        }

        public static double ComputeRatioInformation(double[,] rendPort, double[,] rendBench)
        {
            double varDelta = ComputeTrackingErrorExPost(rendPort, rendBench);
            double[,] deltaRend = ComputeDeltaRend(rendPort, rendBench);
            double espDelta = 0;
            for (int i = 0; i < deltaRend.GetLength(0); i++)
            {
                espDelta += deltaRend[i, 0];
            }
            espDelta /= deltaRend.GetLength(0);
            double res = espDelta / varDelta;
            return res;
        }
    }
}

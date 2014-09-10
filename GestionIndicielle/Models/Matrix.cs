using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GestionIndicielle
{
    class Matrix
    {
        private double[,] _mat;

        public double[,] Mat
        {
            get { return _mat; }
            set
            {
                _mat = value;
                MatR = computeRMatrix(_mat);
                CovMat = computeCovarianceMatrix(MatR);
                MatRMoyen = computeRMoyenMatrix(MatR);
                WeightsVect = computeWeightedVector(MatRMoyen, CovMat, Mat);
            }
        }

        public double[] WeightsVect { get; set; }

        public double[] MatRMoyen { get; set; }

        public double[,] MatR { get; set; }
        public double[,] CovMat { get; set; }

        public Matrix(double[,] initMat)
        {
            Mat = initMat;
        }

        const string pathToDll = @"C:\Users\Yael\Desktop\COURS ENSIMAG\Ensimag 3A\Projet Gestion Indicielle\src\GestionIndicielle\GestionIndicielle\Dlls\wre-ensimag-c-4.1.dll";

        [DllImport(pathToDll, EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NORMmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );

        public double[,] computeRMatrix(double[,] returns)
        {
            var res = new double[returns.GetLength(0)-1,returns.GetLength(1)];
            for (int j = 0; j < returns.GetLength(1); j++)
            {
                for (int i = 1; i < returns.GetLength(0); i++)
                {
                    res[i-1, j] = (returns[i, j] - returns[i-1, j])/returns[i-1, j];
                }
            }
            return res;
        }

        public double[] computeRMoyenMatrix(double[,] returns)
        {
            var res = new double[returns.GetLength(1)];
            for (int j = 0; j < returns.GetLength(1); j++)
            {
                double assetRMoy = 0;
                for (int i = 0; i < returns.GetLength(0); i++)
                {
                    assetRMoy += returns[i, j];
                }
                res[j] = assetRMoy/returns.GetLength(0);
            }
            return res;
        }

        public double[,] computeCovarianceMatrix(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int returnFromNorm = NORMmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            if (returnFromNorm != 0)
            {

                throw new Exception(); // Check out what went wrong here
            }
            return covMatrix;
        }

        [DllImport(pathToDll, EntryPoint = "WREallocIT", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NORMallocIT(
            ref int nbAssets,
            double[,] cov,
            double[] expectedReturns,
            double[] benchmarkCov,
            ref double benchmarkExpectedReturns,
            ref int nbEqConst,
            ref int nbIneqConst,
            double[,] C,
            double[] b,
            double[] minWeights,
            double[] maxWeights,
            ref double relativeTargetReturn,
            double[] optimalWeights,
            ref int info
        );


        public double[] computeWeightedVector(double[] returns, double[,] covMat, double[,] mat)
        {
            int nbAssets = mat.GetLength(1);
            double benchmarkExpectedReturns = 0;
            int nbEqConst = 1;
            int nbIneqConst = 0;
            var C = new double[nbAssets,nbEqConst+nbIneqConst];
            for (int i = 0; i < C.GetLength(0); i++)
            {
                C[i, 0] = 1;
            }
            var b = new double[nbEqConst+nbIneqConst];
            b[0] = 1;
            var minWeights = new double[nbAssets];
            var maxWeights = new double[nbAssets];
            for (int i = 0; i < maxWeights.Length; i++)
            {
                maxWeights[i] = 1;
            }
            double relativeTargetReturn = 0.02;
            int info = 0;
            var optimalWeights = new double[nbAssets];

            int returnFromNorm = NORMallocIT(ref nbAssets, covMat, returns, returns, ref benchmarkExpectedReturns, ref nbEqConst, ref nbIneqConst,C,b,minWeights,maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
            if (returnFromNorm != 0)
            {
                throw new Exception(); // Check out what went wrong here
            }
            return optimalWeights;
        }
    }
}

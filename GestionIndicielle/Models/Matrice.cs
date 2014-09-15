using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace GestionIndicielle.Models
{
    public class Matrice
    {
        public int InfoCov;
        public int ReturnFromNormCov;
        public int ReturnFromNormWeights;
        public int InfoWeights;
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

        private double[,] _benchMat;

        public double[,] BenchMat
        {
            get { return _benchMat; }
            set
            {
                _benchMat = value;
            }
        }

        public double[] WeightsVect { get; set; }

        public double[] MatRMoyen { get; set; }

        public double[,] MatR { get; set; }
        public double[,] CovMat { get; set; }

        public double[,] MatTotal { get; set; }

        public double RelativeTargetReturn;

        public int currentRebIndex;
        public Matrice(double[,] initMat, double[,]benchMat,  double RTR,int index)
        {
            currentRebIndex = index;
            RelativeTargetReturn = RTR;
            BenchMat = benchMat;
            MatTotal = new double[initMat.GetLength(0),initMat.GetLength(1)+1];
            for (int i = 0; i < initMat.GetLength(0); i++)
            {
                MatTotal[i, initMat.GetLength(1)] = benchMat[i, 0];
                for (int j = 0; j < initMat.GetLength(1); j++)
                {
                    MatTotal[i, j] = initMat[i, j];
                }
            }
            MatTotal = computeRMatrix(MatTotal);
            MatTotal = computeCovarianceMatrix(MatTotal);
            Mat = initMat;
        }

        const string pathToDll = @"..\..\Dlls\wre-ensimag-c-4.1.dll";

        [DllImport(pathToDll, EntryPoint = "WREmodelingCov", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NORMmodelingCov(
            ref int returnsSize,
            ref int nbSec,
            double[,] secReturns,
            double[,] covMatrix,
            ref int info
        );

        public static double[,] computeRMatrix(double[,] returns)
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

        public static double[] computeRMoyenMatrix(double[,] returns)
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

        public static double[,] computeCovarianceMatrixForErrors(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int returnFromNormCov = NORMmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            int infoCov = info;
            return covMatrix;
        }

        public double[,] computeCovarianceMatrix(double[,] returns)
        {
            int dataSize = returns.GetLength(0);
            int nbAssets = returns.GetLength(1);
            double[,] covMatrix = new double[nbAssets, nbAssets];
            int info = 0;
            int returnFromNormCov = NORMmodelingCov(ref dataSize, ref nbAssets, returns, covMatrix, ref info);
            int infoCov = info;
            if (returnFromNormCov != 0)
            {
                InfoCov = infoCov;
                ReturnFromNormCov = returnFromNormCov;
            }
            return covMatrix;
        }

        [DllImport(pathToDll, EntryPoint = "WREmodelingSDLScorr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NORMmodelingSDLScorr(
            ref int p,
            double[,] Q,
            ref double constPrecision,
            ref double minEigenValue,
            double[,] X,
            ref int info
            );

        public static double[,] computeDPCorrMatrix(double[,] nonDPCorrMatrix)
        {
            int p = nonDPCorrMatrix.GetLength(0);
            double constPrecision = 0.00000001;
            double minEigenValue = 0.0000001;
            var corrMatrix = new double[p, p];
            int info = 0;
            int returnFromNorm = NORMmodelingSDLScorr(ref p, nonDPCorrMatrix, ref constPrecision, ref minEigenValue, corrMatrix, ref info);
            if (returnFromNorm != 0)
            {
                MessageBox.Show("Erreur lors de la génération des poids. \nVeuillez augmenter la période d'estimation", "Fatal Error");
                    // Check out what went wrong here
            }

            return corrMatrix;
        }

        public static double[,] computeCovToCorr(double[,] CovMatrix)
        {
            int SizeMatrix = CovMatrix.GetLength(0);
            var CorrMatrix = new double[SizeMatrix, SizeMatrix];
            for (int i = 0; i < SizeMatrix; i++)
            {
                for (int j = 0; j < SizeMatrix; j++)
                {
                    CorrMatrix[i, j] = CovMatrix[i, j] / Math.Sqrt(CovMatrix[i, i] * CovMatrix[j, j]);
                }
            }
            return CorrMatrix;
        }

        public static double[,] computeCorrToCov(double[,] CorrMatrix, double[] CovVector)
        {
            int SizeMatrix = CorrMatrix.GetLength(0);
            var CovMatrix = new double[SizeMatrix, SizeMatrix];
            for (int i = 0; i < SizeMatrix; i++)
            {
                for (int j = 0; j < SizeMatrix; j++)
                {

                    CovMatrix[i, j] = CorrMatrix[i, j] * Math.Sqrt(CovVector[i] * CovVector[j]);
                }
            }
            return CovMatrix;
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
            int nbEqConst = 1;
            int nbIneqConst = 0;
            double[,] benchRMatrix = computeRMatrix(BenchMat);
            double benchExpectedReturns = computeRMoyenMatrix(benchRMatrix)[0];
            var benchCov = new double[nbAssets];
            for (int i = 0; i < nbAssets; i++)
            {
                benchCov[i] = MatTotal[i, MatTotal.GetLength(1) - 1];
            }
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
            double relativeTargetReturn = RelativeTargetReturn;
            int info = 0;
            var optimalWeights = new double[nbAssets];

            int returnFromNormWeights = NORMallocIT(ref nbAssets, covMat, returns, benchCov , ref benchExpectedReturns, ref nbEqConst, ref nbIneqConst,C,b,minWeights,maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
            if (returnFromNormWeights != 0)
            {
                
                if (info == -108 && returnFromNormWeights == 5)
                {
                    var CovVector = new double[covMat.GetLength(0)];
                    for (int i = 0; i < CovVector.GetLength(0); i++)
                    {
                        CovVector[i] = covMat[i, i];
                    }
                    covMat = computeCorrToCov(computeDPCorrMatrix(computeCovToCorr(covMat)), CovVector);
                    returnFromNormWeights = NORMallocIT(ref nbAssets, covMat, returns, benchCov , ref benchExpectedReturns, ref nbEqConst, ref nbIneqConst,C,b,minWeights,maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
                }
                if (info == -100 && returnFromNormWeights == 5)
                {
                    RelativeChange = true;
                    relativeTargetReturn = 1;
                }
                while (info == -100 && returnFromNormWeights == 5)
                {
                    relativeTargetReturn /= 10;
                    returnFromNormWeights = NORMallocIT(ref nbAssets, covMat, returns, benchCov, ref benchExpectedReturns, ref nbEqConst, ref nbIneqConst, C, b, minWeights, maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
                }
                double dizaine = relativeTargetReturn;
                relativeTargetReturn *= 10;
                info = -100;
                returnFromNormWeights = 5;
                while (info == -100 && returnFromNormWeights == 5)
                {
                    relativeTargetReturn -= dizaine;
                    returnFromNormWeights = NORMallocIT(ref nbAssets, covMat, returns, benchCov, ref benchExpectedReturns, ref nbEqConst, ref nbIneqConst, C, b, minWeights, maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
                }
                RelativeTargetReturn = relativeTargetReturn;
            }
            InfoWeights = info;
            ReturnFromNormWeights = returnFromNormWeights;
            return optimalWeights;
        }

        
        public bool RelativeChange = false;
    }
}

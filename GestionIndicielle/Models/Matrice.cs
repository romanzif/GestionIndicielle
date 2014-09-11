﻿using System;
using System.Runtime.InteropServices;

namespace GestionIndicielle.Models
{
    public class Matrice
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

        public Matrice(double[,] initMat, double[,]benchMat)
        {
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

        const string pathToDll = @"C:\Users\Yael\Desktop\COURS ENSIMAG\Ensimag 3A\Projet Gestion Indicielle\src\GestionIndicielle\GestionIndicielle\Dlls\wre-ensimag-c-4.1.dll";

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

        public static double[,] computeCovarianceMatrix(double[,] returns)
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
            double relativeTargetReturn = 0.02;
            int info = 0;
            var optimalWeights = new double[nbAssets];

            int returnFromNorm = NORMallocIT(ref nbAssets, covMat, returns, benchCov , ref benchExpectedReturns, ref nbEqConst, ref nbIneqConst,C,b,minWeights,maxWeights, ref relativeTargetReturn, optimalWeights, ref info);
            if (returnFromNorm != 0)
            {
                throw new Exception(); // Check out what went wrong here
            }
            return optimalWeights;
        }

        

    }
}
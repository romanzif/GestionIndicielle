using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionIndicielle.Models
{
    public class Benchmark
    {
        public double BudgetInit;
        public double [,] BenchmarkMatrix;
        public double[] BenchmarkValue;
        public double[,] BenchmarkRendMatrix;

        public Benchmark(double[,] benchMat, double budgetInit)
        {
            BenchmarkMatrix = benchMat;
            BudgetInit = budgetInit;
            BenchmarkRendMatrix = Matrice.computeRMatrix(BenchmarkMatrix);
            BenchmarkValue = new double[benchMat.GetLength(0)];
            BenchmarkValue[0] = BudgetInit;
            for (int i = 1; i < BenchmarkValue.GetLength(0); i++)
            {
                BenchmarkValue[i] = BenchmarkValue[i-1]*(1 + BenchmarkRendMatrix[i-1, 0]);
            }
        }       
    }
}

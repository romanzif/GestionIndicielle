using System;

namespace GestionIndicielle.Models
{
    class Matrice
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public double[,] Mat { get; set; }

        public Matrice(int row, int col)
        {
            Row = row;
            Col = col;
            Mat=new double[row,col];
        }

        public void Print()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    Console.Write(Mat[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}

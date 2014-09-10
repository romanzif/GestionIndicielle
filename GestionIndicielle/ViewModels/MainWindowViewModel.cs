using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Mvvm;
using GestionIndicielle.Models;
using GestionIndicielle.Parser;

namespace GestionIndicielle.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        private Matrice _mat;
        public Matrice Mat
        {
            get { return _mat; }
            set { 
                    _mat = value;
                    OnPropertyChanged(() => this.Mat);
                }
        }
            
            //double [,] tmp = { {1, 1, 1}, {2, 2, 2}, {3, 3, 3}};
            //Mat = new Matrix(tmp);


            //double[,] myCovMatrix = Mat.MatR;
            //Console.WriteLine("Covariance matrix:");
            //for (int i = 0; i < myCovMatrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < myCovMatrix.GetLength(0); j++)
            //    {
            //        Console.WriteLine("Cov(" + i + "," + j + ")=" + myCovMatrix[i, j]);

            //    }
            //}

            //double[] myRMoy = Mat.MatRMoyen;
            //Console.WriteLine("RMoyMatrix:");
            //for (int i = 0; i < myRMoy.GetLength(0); i++)
            //{
            //        Console.WriteLine("Cov(" + i + ")" + myRMoy[i]);
            //}
            //Console.WriteLine("Type enter to exit");
            //Console.Read();
            public Matrice D,I;
        
            public MainWindowViewModel()
            {
                DateTime tFin = new DateTime(2006, 1, 17, 0, 0, 0);
                DateTime tDebut= new DateTime(2006,1,2,0,0,0);

                D = new Matrice(new double[DaysIgnoreWeekends(tDebut,tFin), 29]);
                I = new Matrice(new double[DaysIgnoreWeekends(tDebut, tFin), 1]);
                Parse.LoadPrice(D,tDebut,tFin);
                Parse.LoadIndice(I,tDebut,tFin);
            }

            private int DaysIgnoreWeekends(DateTime tDebut, DateTime tFin)
            {
               TimeSpan days = tFin.Subtract(tDebut);
               int count = 0;
               for (int a = 0; a < days.Days + 1; a++)
               {
                   if (tDebut.DayOfWeek != DayOfWeek.Saturday && tDebut.DayOfWeek != DayOfWeek.Sunday)
                       count++;
                   tDebut = tDebut.AddDays(1.0);
               }
               Console.Write(count);
               return count;
            }
    }
}

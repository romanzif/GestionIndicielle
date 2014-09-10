using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Mvvm;

namespace GestionIndicielle.ViewModel
{
    class MainWindowViewModel : BindableBase
    {
        private Matrix _mat;
        public Matrix Mat
        {
            get { return _mat; }
            set { 
                    _mat = value;
                    OnPropertyChanged(() => this.Mat);
                }
        }

        public MainWindowViewModel()
        {
            
            double [,] tmp = { {1, 1, 1}, {2, 2, 2}, {3, 3, 3}};
            Mat = new Matrix(tmp);


            double[,] myCovMatrix = Mat.MatR;
            Console.WriteLine("Covariance matrix:");
            for (int i = 0; i < myCovMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < myCovMatrix.GetLength(0); j++)
                {
                    Console.WriteLine("Cov(" + i + "," + j + ")=" + myCovMatrix[i, j]);

                }
            }

            double[] myRMoy = Mat.MatRMoyen;
            Console.WriteLine("RMoyMatrix:");
            for (int i = 0; i < myRMoy.GetLength(0); i++)
            {
                    Console.WriteLine("Cov(" + i + ")" + myRMoy[i]);
            }
            Console.WriteLine("Type enter to exit");
            Console.Read();
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Input;
using GestionIndicielle.Models;
using GestionIndicielle.Parser;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;


namespace GestionIndicielle.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DateTime Tfin = new DateTime(2006, 1, 17, 0, 0, 0);
        public DateTime Tdebut = new DateTime(2006, 1, 2, 0, 0, 0);
        private string _periodeEstimation, _periodeRebalancement,_budget;
        private int _perEstimation, _perRebalancement,_budg;
        private DateTime _tGraphDebut = new DateTime(2006, 1, 2, 0, 0, 0);
        private DateTime _tGraphFin = new DateTime(2006, 1, 17, 0, 0, 0);

        

        public DateTime TGraphDebut
        {
            get { return _tGraphDebut; }
            set
            {
                _tGraphDebut = value; 
                Console.Write(_tGraphDebut);
            }
        }
 

        public DateTime TGraphFin
        {
            get { return _tGraphFin; }
            set
            {
                if (DateTime.Compare(_tGraphDebut, value) < 0)
                {
                    _tGraphFin = value;
                }
                else
                {
                    _tGraphFin = _tGraphDebut.AddDays(1);
                }
            }
        }


        public string  PeriodeEstimation
        {
            get { return _periodeEstimation; }
            set
            {
                if (_periodeEstimation != value)
                {
                    _periodeEstimation = value;
                    OnPropertyChanged(() =>PeriodeEstimation);
                    _perEstimation = int.Parse(_periodeEstimation);
                    Console.Write(_perEstimation);
                };
            }
 
        }

        public string PeriodeRebalancement
        {
            get { return _periodeRebalancement; }

            set
            {
                if (_periodeRebalancement != value)
                {
                    _periodeRebalancement = value;
                    OnPropertyChanged(()=>PeriodeRebalancement);
                    _perRebalancement = int.Parse(_periodeRebalancement);
                    Console.Write(_periodeRebalancement);
                }
            }
        }

        public string Budget
        {
            get { return _budget; }
            set
            {
                if(_budget != value)
                {
                    _budget = value;
                    OnPropertyChanged(() => Budget);
                    _budg = int.Parse(_budget);
                    Console.Write(_budget);
                };
            }

        }
        public ICommand Selection { get; private set; }

        public Matrice D, I;

         public MainWindowViewModel()
         {
            Selection = new DelegateCommand(Click);

            D = new Matrice(new double[DaysIgnoreWeekends(Tdebut,Tfin), 29]);
            I = new Matrice(new double[DaysIgnoreWeekends(Tdebut, Tfin), 1]);
            Parse.LoadPrice(D,Tdebut,Tfin);
            Parse.LoadIndice(I,Tdebut,Tfin);
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

        public void Begin(DateTime d)
        {
            Tdebut = d;
        }

        public void End(DateTime d)
        {
            Tfin = d;
        }

        public void Click()
        {
            SelectBalancement();
            SelectEstimation();
            SelectBudget();
        }

        private void SelectBalancement()
        {
            //PeriodeRebalancement = Rebalancement.selectedText();
            Console.Write("Balancement");
        }

        private void SelectEstimation()
        {
           // PeriodeEstimation = Estimation.selectedText();
            Console.Write("Estimation");
        }

        private void SelectBudget()
        {
            // PeriodeEstimation = Estimation.selectedText();
            Console.Write("Budget");
        }
    }
}

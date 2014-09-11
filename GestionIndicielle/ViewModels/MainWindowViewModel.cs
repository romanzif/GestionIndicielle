using System;
using System.Linq;
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
        private string _periodeEstimation, _periodeRebalancement,_budget;
        public string  PeriodeEstimation
        {
            get { return _periodeEstimation; }
            set
            {
                if (_periodeEstimation != value)
                {
                    _periodeEstimation = value;
                    OnPropertyChanged(() =>PeriodeEstimation);
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
                };
            }

        }

        public ICommand Selection { get; private set; }
        public double[,] D, I;
        public FormatMatrix FormatedBigMatrix;
        public FormatMatrix FormatedBenchMatrix;
        public DateTime Tfin = new DateTime(2010, 1, 17, 0, 0, 0);
        public DateTime Tdebut = new DateTime(2006, 1, 2, 0, 0, 0);

        public MainWindowViewModel()
        {
            Selection = new DelegateCommand(Click);
            D = new double[DaysIgnoreWeekends(Tdebut, Tfin), 29];
            I = new double[DaysIgnoreWeekends(Tdebut, Tfin), 1];
            PeriodeEstimation = "100"; // 2semaines 
            PeriodeRebalancement = "45"; //2mois
            Budget = "100";
            Parse.LoadPrice(D, Tdebut, Tfin);
            Parse.LoadIndice(I, Tdebut, Tfin);
            FormatedBigMatrix = new FormatMatrix(D, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            FormatedBenchMatrix = new FormatMatrix(I, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            var estMatrix = new Matrice(FormatedBigMatrix.EstimationMatrixList.First(), FormatedBenchMatrix.EstimationMatrixList.First());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Input;
using GestionIndicielle.Models;
using GestionIndicielle.Parser;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace GestionIndicielle.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _periodeEstimation, _periodeRebalancement,_budget;
        private DateTime _tGraphDebut = new DateTime(2006, 1, 2, 0, 0, 0);
        private DateTime _tGraphFin = new DateTime(2006, 1, 3, 0, 0, 0);
        public double TrackError=2.5;
        public double RatioInfo=3.0;

        private PlotModel plotModel;
        private PlotModel plotModel2;

        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        public PlotModel PlotModel2
        {
            get { return plotModel2; }
            set { plotModel2 = value; OnPropertyChanged("PlotModel2"); }
        }
        public string TrackingError
        {
            get { return TrackError.ToString(); }
            set { ; }
        }


        public string RatioInformation
        {
            get { return RatioInfo.ToString(); }
            set { ; }
        }

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
                }
            }

        }

        public ICommand Selection { get; private set; }
        public double[,] D, I;
        public FormatMatrix FormatedBigMatrix;
        public FormatMatrix FormatedBenchMatrix;
        public List<Portfolio> MyPortList;
        public DateTime Tfin = new DateTime(2010, 1, 17, 0, 0, 0);
        public DateTime Tdebut = new DateTime(2006, 1, 2, 0, 0, 0);

        public MainWindowViewModel()
        {
            PlotModel = new PlotModel();
            PlotModel2 = new PlotModel();
            Selection = new DelegateCommand(Click);
            D = new double[DaysIgnoreWeekends(Tdebut, Tfin), 29];
            I = new double[DaysIgnoreWeekends(Tdebut, Tfin), 1];
            PeriodeEstimation = "100"; // 2semaines 
            PeriodeRebalancement = "100"; //2mois
            Budget = "100";
            Parse.LoadPrice(D, Tdebut, Tfin);
            Parse.LoadIndice(I, Tdebut, Tfin);
            FormatedBigMatrix = new FormatMatrix(D, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            FormatedBenchMatrix = new FormatMatrix(I, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            MyPortList = new List<Portfolio>();
            MyPortList.Add(new Portfolio(FormatedBigMatrix.RebalancementMatrixList.First(), FormatedBigMatrix.EstimationMatrixList.First(), FormatedBenchMatrix.EstimationMatrixList.First(), int.Parse(Budget)));

            for (int i = 0; i < FormatedBigMatrix.RebalancementMatrixList.Count; i++)
            {
                MyPortList.Add(new Portfolio(FormatedBigMatrix.RebalancementMatrixList[i], FormatedBigMatrix.EstimationMatrixList[i], FormatedBenchMatrix.EstimationMatrixList[i], int.Parse(Budget)));
            }
            SetUpModel();
            double[] essai = {2,3,5,6,1,4,8,5};
            double[] essai2 = { 5, 10, 20, 15, 1, 4, 8, 5 };
            LoadData(essai, essai2);
            LoadData2(essai, essai2);
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

        private void SetUpModel()
        {
            PlotModel.LegendTitle = "Portefeuille vs Benchmark";
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.TopRight;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;

            var dateAxis = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new OxyPlot.Axes.LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel.Axes.Add(valueAxis);

            PlotModel2.LegendTitle = "Portefeuille vs Benchmark";
            PlotModel2.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel2.LegendPlacement = LegendPlacement.Outside;
            PlotModel2.LegendPosition = LegendPosition.TopRight;
            PlotModel2.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel2.LegendBorder = OxyColors.Black;

            var dateAxis2 = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            PlotModel2.Axes.Add(dateAxis2);
            var valueAxis2 = new OxyPlot.Axes.LinearAxis(AxisPosition.Left, 0) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel2.Axes.Add(valueAxis2);
        }

        private void LoadData(double[] valPortef, double [] valBenchmark)
        {
            var tmp = new PlotModel { Title = "Valeur du Portefeuille vs Benchmark "};

            var series1 = new OxyPlot.Series.LineSeries { Title = "Portefeuille", MarkerType = MarkerType.Circle };
            for (int i =0; i<valPortef.Length; i++) {
                series1.Points.Add(new DataPoint(i, valPortef[i]));
            }

            var series2 = new OxyPlot.Series.LineSeries { Title = "Benchmark", MarkerType = MarkerType.Circle };
            for (int i = 0; i < valBenchmark.Length; i++)
            {
                series2.Points.Add(new DataPoint(i, valBenchmark[i]));
            }

            tmp.Series.Add(series1);
            tmp.Series.Add(series2);
            this.PlotModel = tmp;
        }

        private void LoadData2(double[] valPortef, double[] valBenchmark)
        {
            var tmp = new PlotModel { Title = "Rendement" };

            var series1 = new OxyPlot.Series.LineSeries { Title = "Portefeuille", MarkerType = MarkerType.Circle };
            for (int i = 0; i < valPortef.Length; i++)
            {
                series1.Points.Add(new DataPoint(i, valPortef[i]));
            }

            var series2 = new OxyPlot.Series.LineSeries { Title = "Benchmark", MarkerType = MarkerType.Circle };
            for (int i = 0; i < valBenchmark.Length; i++)
            {
                series2.Points.Add(new DataPoint(i, valBenchmark[i]));
            }

            tmp.Series.Add(series1);
            tmp.Series.Add(series2);
            this.PlotModel2 = tmp;
        }
      
    }
}

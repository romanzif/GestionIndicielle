using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
        private DateTime _tGraphFin = new DateTime(2013, 9, 3, 0, 0, 0);
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
            get
            {
                TrackError *= 100;
                return TrackError.ToString("#0.000", System.Globalization.CultureInfo.InvariantCulture) + "%"; ; }
            set { ; }
        }


        public string RatioInformation
        {
            get
            {
                RatioInfo *= 100;
                return RatioInfo.ToString("#0.000", System.Globalization.CultureInfo.InvariantCulture) + "%"; ;
            }
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

        public ICommand OkCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ForwardCommand { get; private set; }
        public double[,] D, I;
        public FormatMatrix FormatedBigMatrix;
        public FormatMatrix FormatedBenchMatrix;
        public List<Portfolio> MyPortList;
        public List<Benchmark> BenchList;
        public DateTime Tfin = new DateTime(2010, 1, 17, 0, 0, 0);
        public DateTime Tdebut = new DateTime(2006, 1, 2, 0, 0, 0);
        private DateTime[] _calendrier;
        private int currentGraphIndex;
        private string _graphIndex;

        public string GraphIndex
        {
            get { return _graphIndex; }
            set { _graphIndex = value; OnPropertyChanged(()=>GraphIndex); }
        }



        public MainWindowViewModel()
        {
            PlotModel = new PlotModel();
            PlotModel2 = new PlotModel();
            OkCommand = new DelegateCommand(Click);
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            PeriodeEstimation = "50"; // 2semaines 
            PeriodeRebalancement = "75"; //2mois
            Budget = "100";
            generateWholeWindowOnChange();
        }

        public void Back()
        {
            if (currentGraphIndex == 0)
            {
                currentGraphIndex = MyPortList.Count - 1;
            }
            else
            {
                currentGraphIndex--;
            }
            GraphIndex = "Rebalancement " + (currentGraphIndex + 1).ToString();
            double[] essai = MyPortList[currentGraphIndex].PortfolioValues;
            double[] essai2 = BenchList[currentGraphIndex].BenchmarkValue;
            LoadData2(essai, essai2);
        }

        public void Forward()
        {
            if (currentGraphIndex == MyPortList.Count - 1)
            {
                currentGraphIndex = 0;
            }
            else
            {
                currentGraphIndex++;
            }
            GraphIndex = "Rebalancement " + (currentGraphIndex + 1).ToString();
            double[] essai = MyPortList[currentGraphIndex].PortfolioValues;
            double[] essai2 = BenchList[currentGraphIndex].BenchmarkValue;
            LoadData2(essai, essai2);
        }

        public DateTime[] Calendrier
        {
            get { return Calendrier; }
            set
            {
                int i = 0;
                for (DateTime j = Tdebut; DateTime.Compare(j, Tfin) < 0; j.AddDays(1))
                {
                    if (j.DayOfWeek != DayOfWeek.Sunday && j.DayOfWeek != DayOfWeek.Saturday)
                    {
                        _calendrier[i] = j;
                        i++;
                    }
                }
            }
        }

        public void generateWholeWindowOnChange()
        {
            D = new double[DaysIgnoreWeekends(Tdebut, Tfin), 29];
            I = new double[DaysIgnoreWeekends(Tdebut, Tfin), 1];
            Parse.LoadPrice(D, Tdebut, Tfin);
            Parse.LoadIndice(I, Tdebut, Tfin);
            FormatedBigMatrix = new FormatMatrix(D, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            FormatedBenchMatrix = new FormatMatrix(I, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            MyPortList = new List<Portfolio>();
            double budget = double.Parse(Budget);
            for (int i = 0; i < FormatedBigMatrix.RebalancementMatrixList.Count; i++)
            {
                var currentPort = new Portfolio(FormatedBigMatrix.RebalancementMatrixList[i],
                    FormatedBigMatrix.EstimationMatrixList[i], FormatedBenchMatrix.EstimationMatrixList[i], budget
                    );
                MyPortList.Add(currentPort);
                int index = FormatedBigMatrix.RebalancementMatrixList[i].GetLength(0) - 1;
                budget = currentPort.PortfolioValues[index];
            }
            BenchList = new List<Benchmark>();
            budget = double.Parse(Budget);
            for (int i = 0; i < FormatedBigMatrix.RebalancementMatrixList.Count; i++)
            {
                var currentBench = new Benchmark(FormatedBenchMatrix.RebalancementMatrixList[i], budget);
                BenchList.Add(currentBench);
                int index = FormatedBenchMatrix.RebalancementMatrixList[i].GetLength(0) - 1;
                budget = currentBench.BenchmarkValue[index];
            }
            double[,] P = new double[MyPortList.Count * MyPortList.First().PortfolioValues.Length, 1];
            double[] PortAsArray = new double[MyPortList.Count * MyPortList.First().PortfolioValues.Length];
            for (int i = 0; i < MyPortList.Count; i++)
            {
                double[] currentPortValues = MyPortList[i].PortfolioValues;
                for (int j = 0; j < currentPortValues.Length; j++)
                {
                    P[j + i * currentPortValues.Length, 0] = currentPortValues[j];
                    PortAsArray[j + i * currentPortValues.Length] = currentPortValues[j];
                }
            }
            double[,] B = new double[BenchList.Count * BenchList.First().BenchmarkValue.Length, 1];
            double[] BenchAsArray = new double[BenchList.Count * BenchList.First().BenchmarkValue.Length];
            for (int i = 0; i < BenchList.Count; i++)
            {
                double[] currentBenchValues = BenchList[i].BenchmarkValue;
                for (int j = 0; j < currentBenchValues.Length; j++)
                {
                    B[j + i * currentBenchValues.Length, 0] = currentBenchValues[j];
                    BenchAsArray[j + i * currentBenchValues.Length] = currentBenchValues[j];
                }
            }
            double[,] RendP = Matrice.computeRMatrix(P);
            double[,] RendB = Matrice.computeRMatrix(B);
            TrackError = ErrorRatios.ComputeTrackingErrorExPost(RendP, RendB);
            RatioInfo = ErrorRatios.ComputeRatioInformation(RendP, RendB);
            SetUpModel();
            double[] essai = PortAsArray;
            double[] essai2 = BenchAsArray;
            LoadData(essai, essai2);
            essai = MyPortList.First().PortfolioValues;
            essai2 = BenchList.First().BenchmarkValue;
            LoadData2(essai, essai2);
            currentGraphIndex = 0;
            GraphIndex = "Rebalancement " + (currentGraphIndex + 1).ToString();
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

        private void Click()
        {
            SelectBalancement();
            SelectEstimation();
            SelectBudget();
            generateWholeWindowOnChange();
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
            this.PlotModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 });
        
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        private string _RTR;
        public double TrackError=0;
        public double RatioInfo=0;

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

        public ObservableCollection<string> AssetList { get; set; }


        public string RatioInformation
        {
            get
            {
                RatioInfo *= 100;
                return RatioInfo.ToString("#0.000", System.Globalization.CultureInfo.InvariantCulture) + "%"; ;
            }
            set { ; }
        }

        public DateTime TDebut
        {
            get { return _tDebut; }
            set
            {
                _tDebut = value; 
                Console.Write(_tDebut);
            }
        }
 

        public DateTime TFin
        {
            get { return _tFin; }
            set
            {
                if (DateTime.Compare(_tDebut, value.AddDays(-int.Parse(PeriodeEstimation) - int.Parse(PeriodeRebalancement)*2)) < 0)
                {
                    _tFin = value;
                }
                else
                {
                    _tFin = _tDebut.AddDays(int.Parse(PeriodeEstimation)+int.Parse(PeriodeRebalancement)*2);
                }
            }
        }

        public string RelativeTargetReturn
        {
            get
            {
                return _RTR;
            }
            set
            {
                if (_RTR != value)
                {
                    _RTR = value;
                    OnPropertyChanged(() => RelativeTargetReturn);

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
        private List<string> Assets;

        public FormatMatrix FormatedBigMatrix;
        public FormatMatrix FormatedBenchMatrix;
        public List<Portfolio> MyPortList;
        public List<Benchmark> BenchList;
        private DateTime _tFin = new DateTime(2013, 9, 3, 0, 0, 0);
        private DateTime _tDebut = new DateTime(2006, 1, 2, 0, 0, 0);
        private DateTime[] _calendrier;
        private int currentGraphIndex;
        private string _graphIndex;

        


        public string GraphIndex
        {
            get { return _graphIndex; }
            set { _graphIndex = value; OnPropertyChanged(()=>GraphIndex); }
        }

        public ObservableCollection<String> SelectedItems { get; private set; }

        public IList<string> SelectedAssetsList; 
        public MainWindowViewModel()
        {
            Assets = new List<string>();
            Assets = Parse.LoadAssets(TDebut);
            AssetList = new ObservableCollection<string>();
            foreach (var asset in Assets)
                AssetList.Add(asset);
            PlotModel = new PlotModel();
            PlotModel2 = new PlotModel();
            OkCommand = new DelegateCommand(Click);
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            PeriodeEstimation = "50"; 
            PeriodeRebalancement = "100";
            RelativeTargetReturn = "0";
            Budget = "100";
            this.SelectedItems = new ObservableCollection<String>();
            this.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            SelectedAssetsList = new List<string>();
            generateWholeWindowOnChange(AssetList);
        }

        void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                SelectedAssetsList.Clear();
                foreach (String str in this.SelectedItems)
                    SelectedAssetsList.Add(str);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            SetUpModel();
            LoadData2(essai, essai2);
        }

        public DateTime[] Calendrier
        {
            get { return Calendrier; }
            set
            {
                int i = 0;
                for (DateTime j = TDebut; DateTime.Compare(j, TFin) < 0; j.AddDays(1))
                {
                    if (j.DayOfWeek != DayOfWeek.Sunday && j.DayOfWeek != DayOfWeek.Saturday)
                    {
                        _calendrier[i] = j;
                        i++;
                    }
                }
            }
        }

        public void generateWholeWindowOnChange(IList<string> dataList)
        {
            D = new double[DaysIgnoreWeekends(TDebut, TFin), dataList.Count];
            I = new double[DaysIgnoreWeekends(TDebut, TFin), 1];
            D = Parse.LoadPrice(dataList, TDebut, TFin);
            I=Parse.LoadIndice(dataList, TDebut, TFin);
            FormatedBigMatrix = new FormatMatrix(D, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            FormatedBenchMatrix = new FormatMatrix(I, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            MyPortList = new List<Portfolio>();
            double budget = double.Parse(Budget, CultureInfo.InvariantCulture);
            double rtr = double.Parse(RelativeTargetReturn, CultureInfo.InvariantCulture);
            for (int i = 0; i < FormatedBigMatrix.RebalancementMatrixList.Count; i++)
            {
                var currentPort = new Portfolio(FormatedBigMatrix.RebalancementMatrixList[i],
                    FormatedBigMatrix.EstimationMatrixList[i], FormatedBenchMatrix.EstimationMatrixList[i], budget, rtr
                    );
                MyPortList.Add(currentPort);
                int index = FormatedBigMatrix.RebalancementMatrixList[i].GetLength(0) - 1;
                budget = currentPort.PortfolioValues[index];
            }
            BenchList = new List<Benchmark>();
            budget = double.Parse(Budget, CultureInfo.InvariantCulture);
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
            OnPropertyChanged(()=>TrackingError);
            OnPropertyChanged(() => RatioInformation);
            double[] essai = PortAsArray;
            double[] essai2 = BenchAsArray;
            SetUpModel();
            LoadData(essai, essai2);
            essai = MyPortList.First().PortfolioValues;
            essai2 = BenchList.First().BenchmarkValue;
            LoadData2(essai, essai2);



            currentGraphIndex = 0;
            GraphIndex = "Rebalancement " + (currentGraphIndex + 1).ToString();
            checkForBugs();
        }

        public void checkForBugs()
        {
            foreach (var portfolio in MyPortList)
            {
                if (portfolio.PortfolioMatrix.InfoCov == 0 && portfolio.PortfolioMatrix.ReturnFromNormCov == 301)
                {
                    MessageBox.Show("Données générées invalides,  \n" +
                                    "veuillez entrer une période d'estimation supérieure ou égale à 3", "Fatal Error");
                    break;
                }
                else if (portfolio.PortfolioMatrix.InfoCov != 0  || portfolio.PortfolioMatrix.ReturnFromNormCov != 0)
                {
                    MessageBox.Show("erreur cov non gérée", "Fatal Error");
                    break;
                }
            }
            foreach (var portfolio in MyPortList)
            {
                if (portfolio.PortfolioMatrix.InfoWeights == -100 && portfolio.PortfolioMatrix.ReturnFromNormWeights == 5)
                {
                    MessageBox.Show("Relative Target Return Invalid", "Fatal Error");
                    break;
                }
                else if (portfolio.PortfolioMatrix.InfoWeights == -108 &&
                         portfolio.PortfolioMatrix.ReturnFromNormWeights == 5)
                {
                    if (!stopMessage)
                        MessageBox.Show("Les données générées avec la période d'estimation fournie étaient invalides.\n" +
                                    "Les données que nous fournissons sont celles avec la plus petite période d'estimation possible.\n", "Information");
                    stopMessage = true;
                    PeriodeEstimation = (int.Parse(PeriodeEstimation)+1).ToString();
                    generateWholeWindowOnChange(SelectedAssetsList);
                    
                    break;
                }
                else if (portfolio.PortfolioMatrix.InfoWeights != 0 || portfolio.PortfolioMatrix.ReturnFromNormWeights != 0)
                {
                    MessageBox.Show("erreur weights non gérée", "Fatal Error");
                    break;
                }
             
            }
        }

        public bool stopMessage = false;
        private int DaysIgnoreWeekends(DateTime TDebut, DateTime TFin)
        {
           TimeSpan days = TFin.Subtract(TDebut);
           int count = 0;
           for (int a = 0; a < days.Days + 1; a++)
           {
               if (TDebut.DayOfWeek != DayOfWeek.Saturday && TDebut.DayOfWeek != DayOfWeek.Sunday)
                   count++;
               TDebut = TDebut.AddDays(1.0);
           }
           Console.Write(count);
           return count;
        }

        public void Begin(DateTime d)
        {
            TDebut = d;
        }

        public void End(DateTime d)
        {
            TFin = d;
        }

        private void Click()
        {
            SelectBalancement();
            SelectEstimation();
            SelectBudget();
            bool error = false;
            stopMessage = false;
            if (SelectedAssetsList.Count < 2)
            {
                MessageBox.Show("Veuillez sélectionner 2 titres ou plus", "Erreur");
                error = true;
            }
            try
            {
                double budget = Double.Parse(Budget, CultureInfo.InvariantCulture);
                if (budget <= 0)
                {
                    MessageBox.Show(budget + " n'est pas un budget valide", "Erreur");
                    error = true;
                }
            }
            catch
            {
                MessageBox.Show("Syntaxe invalide, veuillez entrer un double. exemple: 100.67", "Erreur Budget");
                error = true;
            }
            try
            {
                double rel = Double.Parse(RelativeTargetReturn, CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Syntaxe invalide, veuillez entrer un double. exemple: 0.067", "Erreur Relative Target Return");
                error = true;
            }
            try
            {
                double estim = int.Parse(PeriodeEstimation) ;
                if (estim <= 0)
                {
                    MessageBox.Show(estim + " n'est pas une période d'estimation valide", "Erreur");
                    error = true;
                }
            }
            catch
            {
                MessageBox.Show("Syntaxe invalide, veuillez entrer un integer. exemple: 100", "Erreur Periode Estimation");
                error = true;
            }
            try
            {
                double reb = int.Parse(PeriodeRebalancement) ;
                if (reb <= 0)
                {
                    MessageBox.Show(reb + " n'est pas une période de rebalancement valide", "Erreur");
                    error = true;
                }
            }
            catch
            {
                MessageBox.Show("Syntaxe invalide, veuillez entrer un integer. exemple: 100", "Erreur Periode Rebalancement");
                error = true;
            }

            if (!error)
                generateWholeWindowOnChange(SelectedAssetsList);
       
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
            PlotModel.LegendPosition = LegendPosition.BottomLeft;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;

            var dateAxis = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot};
            dateAxis.Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(TDebut);
            dateAxis.Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(TFin);
            dateAxis.IntervalType = DateTimeIntervalType.Years;
            dateAxis.MinorIntervalType = DateTimeIntervalType.Months;
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new OxyPlot.Axes.LinearAxis(AxisPosition.Left) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel.Axes.Add(valueAxis);

            PlotModel2.LegendTitle = "Portefeuille vs Benchmark";
            PlotModel2.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel2.LegendPlacement = LegendPlacement.Outside;
            PlotModel2.LegendPosition = LegendPosition.BottomLeft;
            PlotModel2.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel2.LegendBorder = OxyColors.Black;

            var dateAxis2 = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot};
            PlotModel2.Axes.Add(dateAxis2);
            var valueAxis2 = new OxyPlot.Axes.LinearAxis(AxisPosition.Left) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
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

            var dateAxis = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot };
            //dateAxis.Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(TDebut);
            //dateAxis.Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(TFin);
            dateAxis.IntervalType = DateTimeIntervalType.Years;
            dateAxis.MinorIntervalType = DateTimeIntervalType.Months;
            tmp.Axes.Add(dateAxis);
            var valueAxis = new OxyPlot.Axes.LinearAxis(AxisPosition.Left) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value", IntervalLength = 50 };
            tmp.Axes.Add(valueAxis);
            this.PlotModel = tmp;

        }

        private void LoadData2(double[] valPortef, double[] valBenchmark)
        {
            var tmp = new PlotModel { Title = "Valeur du portefeuille vs Benchmark (Zoom)" };

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

            var dateAxis = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot };
            //dateAxis.Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(TDebut);
            //dateAxis.Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(TFin);
            dateAxis.IntervalType = DateTimeIntervalType.Months;
            tmp.Axes.Add(dateAxis);
            var valueAxis = new OxyPlot.Axes.LinearAxis(AxisPosition.Left) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value", IntervalLength = 50 };
            tmp.Axes.Add(valueAxis);
            this.PlotModel2 = tmp;
        }

        private void UpdateData2(double[] valPortef, double[] valBenchmark)
        {

            var tmp = new PlotModel { Title = "Valeur du portefeuille vs Benchmark (Zoom)" };

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
            this.PlotModel2.Axes.Add(new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });
        }
    }
}

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
                return RatioInfo.ToString("#0.000", System.Globalization.CultureInfo.InvariantCulture) + "%";
            }
            set { ; }
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
        private DateTime _tDebut, _tFin;

        public DateTime TFin
        {
            get { return _tFin; } 
            set { _tFin = value; OnPropertyChanged(()=>TFin); }
        }
        public DateTime TDebut
        {
            get { return _tDebut; }
            set { _tDebut = value; OnPropertyChanged(() => TDebut); }
        }
        private DateTime[] _calendrier;

        public ObservableCollection<String> SelectedItems { get; private set; }

        public IList<string> SelectedAssetsList; 
        public MainWindowViewModel()
        {
            TDebut = new DateTime(2006, 1, 2, 0, 0, 0);
            TFin = new DateTime(2013, 9, 3, 0, 0, 0);
            Assets = new List<string>();
            Assets = Parse.LoadAssets(TDebut);
            AssetList = new ObservableCollection<string>();
            foreach (var asset in Assets)
                AssetList.Add(asset);
            PlotModel = new PlotModel();
            PlotModel2 = new PlotModel();
            OkCommand = new DelegateCommand(Click);
            PeriodeEstimation = "50"; 
            PeriodeRebalancement = "100";
            RelativeTargetReturn = "0";
            Budget = "100";
            this.SelectedItems = new ObservableCollection<String>();
            this.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            SelectedAssetsList = new List<string>();
            generateWholeWindowOnChangeBackTest(AssetList);
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

        public void generateWholeWindowOnChangeBackTest(IList<string> dataList)
        {
            D = new double[DaysIgnoreWeekends(TDebut, TFin), dataList.Count];
            I = new double[DaysIgnoreWeekends(TDebut, TFin), 1];
            D = Parse.LoadPrice(dataList, TDebut, TFin);
            I=Parse.LoadIndice(dataList, TDebut, TFin);
            FormatedBigMatrix = new FormatMatrix(D, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            FormatedBenchMatrix = new FormatMatrix(I, int.Parse(PeriodeEstimation), int.Parse(PeriodeRebalancement));
            
            if (FormatedBigMatrix.RebalancementMatrixList.Count == 0)
            {
                MessageBox.Show(
                    "Attention, la période choisie n'est pas assez importante en comparaison de la fenêtre d'estimation et de la période de rebalancement",
                    "Fatal Error");
                stopMessage = true;
            }
            else
            {
                MyPortList = new List<Portfolio>();
                double budget = double.Parse(Budget, CultureInfo.InvariantCulture);
                double rtr = double.Parse(RelativeTargetReturn, CultureInfo.InvariantCulture)/100;
                string relativeTargetRebReCalc ="";
                string relativeTargetReCalcResult ="";
                bool change = false;
                for (int i = 0; i < FormatedBigMatrix.RebalancementMatrixList.Count; i++)
                {
                    var currentPort = new Portfolio(FormatedBigMatrix.RebalancementMatrixList[i],
                        FormatedBigMatrix.EstimationMatrixList[i], FormatedBenchMatrix.EstimationMatrixList[i], budget, rtr,i
                        );
                    MyPortList.Add(currentPort);
                    if (currentPort.PortfolioMatrix.RelativeChange)
                    {
                        int currentIndex = i + 1;
                        relativeTargetRebReCalc += currentIndex + " ";
                        relativeTargetReCalcResult += currentPort.PortfolioMatrix.RelativeTargetReturn + " ";
                        change = true;
                    }
                    int index = FormatedBigMatrix.RebalancementMatrixList[i].GetLength(0) - 1;
                    budget = currentPort.PortfolioValues[index];
                }
                if (change)
                        MessageBox.Show(
                            "Le relative Target Return était trop important pour les rebalancements " + relativeTargetRebReCalc +".\n" +
                            "Ils ont été fixé aux valeurs " + relativeTargetReCalcResult+".", "Information");
                double[,] P = new double[MyPortList.Count * MyPortList.First().PortfolioValues.Length+1, 1];
                double[] PortAsArray = new double[MyPortList.Count * MyPortList.First().PortfolioValues.Length +1];
                P[0, 0] = double.Parse(Budget);
                PortAsArray[0] = double.Parse(Budget);
                for (int i = 0; i < MyPortList.Count; i++)
                {
                    double[] currentPortValues = MyPortList[i].PortfolioValues;
                    for (int j = 0; j < currentPortValues.Length; j++)
                    {
                            P[j + i*currentPortValues.Length +1, 0] = currentPortValues[j];
                            PortAsArray[j + i*currentPortValues.Length+1] = currentPortValues[j];
                    }
                }

                
                double [] benchArray = convertMatrixToArray(I, int.Parse(PeriodeEstimation) - 1, PortAsArray.Length + int.Parse(PeriodeEstimation)-1, I[int.Parse(PeriodeEstimation) - 1, 0] / double.Parse(Budget));
                double[,] benchMatrix = convertArrayToMatrix(benchArray);
                double[,] RendP = Matrice.computeRMatrix(P);
                double[,] RendB = Matrice.computeRMatrix(benchMatrix);
                TrackError = ErrorRatios.ComputeTrackingErrorExPost(RendP, RendB);
                RatioInfo = ErrorRatios.ComputeRatioInformation(RendP, RendB);
                OnPropertyChanged(() => TrackingError);
                OnPropertyChanged(() => RatioInformation);
                double[] essai = PortAsArray;
                double[] essai2 = benchArray;
                SetUpModel();
                LoadData(essai, essai2);
                double[] RmoyPMatrix = convertMatrixToArray(RendP,0,RendP.GetLength(0),1);
                double[] RmoyBMatrix = convertMatrixToArray(RendB,0,RendB.GetLength(0),1);
                essai = RmoyPMatrix;
                essai2 = RmoyBMatrix;
                LoadData2(essai, essai2);
                checkForBugs();
            }
            
        }


        public double[] convertMatrixToArray(double[,] matrix, int firstIndex,int lastIndex,double divisionFactor)
        {
            if (matrix.GetLength(1) != 1)
                MessageBox.Show("Attention, matrice ayant plus de 1 colonne lors de la conversion en vecteur", "Warning");
            var res = new double[lastIndex-firstIndex];
            for (int i = firstIndex; i < lastIndex; i++)
                res[i-firstIndex] = matrix[i, 0]/divisionFactor;
            return res;
        }

        public double[,] convertArrayToMatrix(double[] array)
        {
            var res = new double[array.Length,1];
            for (int i = 0; i < res.Length; i++)
                res[i,0] = array[i];
            return res;
        }

        public void checkForBugs()
        {
            foreach (var portfolio in MyPortList)
            {
                if (portfolio.PortfolioMatrix.InfoCov == 0 && portfolio.PortfolioMatrix.ReturnFromNormCov == 301)
                {
                    MessageBox.Show("Données générées invalides,  \n" +
                                    "veuillez entrer une période d'estimation supérieure ou égale à 3", "Error");
                    break;
                }
                else if (portfolio.PortfolioMatrix.InfoCov != 0  || portfolio.PortfolioMatrix.ReturnFromNormCov != 0)
                {
                    MessageBox.Show("Erreur inconnue", "Fatal Error");
                    break;
                }
            }
            foreach (var portfolio in MyPortList)
            {
                if (portfolio.PortfolioMatrix.InfoWeights == -100 && portfolio.PortfolioMatrix.ReturnFromNormWeights == 5)
                {
                    MessageBox.Show("Relative Target Return invalide", "Fatal Error");
                    break;
                }
                else if (portfolio.PortfolioMatrix.InfoWeights == -108 &&
                         portfolio.PortfolioMatrix.ReturnFromNormWeights == 5)
                {
                    if (!stopMessage)
                        MessageBox.Show("Les données générées avec la période d'estimation fournie étaient invalides.\n" +
                                    "Veuillez patienter, recherche en cours de la plus petite période d'estimation possible.", "Information");
                    stopMessage = true;
                    PeriodeEstimation = (int.Parse(PeriodeEstimation)+1).ToString();
                    generateWholeWindowOnChangeBackTest(SelectedAssetsList);
                    
                    break;
                }
                else if (portfolio.PortfolioMatrix.InfoWeights != 0 || portfolio.PortfolioMatrix.ReturnFromNormWeights != 0)
                {
                    MessageBox.Show("Erreur inconnue", "Fatal Error");
                    break;
                }
             
            }
        }
        private bool stopMessage = false;

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

        public bool checkForCorrectData()
        {
            bool error = false;
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
                if (rel < 0)
                {
                    MessageBox.Show("Attention, vous avez choisi un relative targer return négatif!", "Warning");
                }
            }
            catch
            {
                MessageBox.Show("Syntaxe invalide, veuillez entrer un double. exemple: 0.067", "Erreur Relative Target Return");
                error = true;
            }
            try
            {
                double estim = int.Parse(PeriodeEstimation);
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
                double reb = int.Parse(PeriodeRebalancement);
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
            if (TDebut < new DateTime(2006, 1, 2, 0, 0, 0))
            {
                MessageBox.Show("La date de début a été fixée au 2 janvier 2006, aucune donnée n'étant disponible avant.", "Erreur Date de début");
                TDebut = new DateTime(2006, 1, 2, 0, 0, 0);
            }
            if (TDebut > new DateTime(2013, 9, 3, 0, 0, 0))
            {
                MessageBox.Show("La date de début a été fixée au 2 janvier 2006, \naucune donnée n'étant disponible après le 3 septembre 2013.", "Erreur Date de début");
                TDebut = new DateTime(2006, 1, 2, 0, 0, 0);
            }
            if (TFin > new DateTime(2013, 9, 3, 0, 0, 0))
            {
                MessageBox.Show("La date de fin a été fixée au 3 septembre 2013, aucune donnée n'étant disponible après.", "Erreur Date de fin");
                TFin = new DateTime(2013, 9, 3, 0, 0, 0);
            }
            if (TFin < new DateTime(2006, 1, 2, 0, 0, 0))
            {
                MessageBox.Show("La date de fin a été fixée au 3 septembre 2013, \naucune donnée n'étant disponible avant le 2 janvier 2006.", "Erreur Date de fin");
                TFin = new DateTime(2013, 9, 3, 0, 0, 0);
            }
            if (TDebut > TFin)
            {
                MessageBox.Show("Veuillez donner une date de début antérieure à la date de fin", "Erreur Date");
                error = true;
            }
            
            return error;
        }
        private void Click()
        {
            stopMessage = false;
            bool error = checkForCorrectData();
            if (!error)
                generateWholeWindowOnChangeBackTest(SelectedAssetsList);

            if (stopMessage)
            {
                MessageBox.Show("Les données générées avec la période d'estimation fournie étaient invalides.\n" +
                                    "Donnée générée avec une période d'estimation de "+PeriodeEstimation, "Information");
            }
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

            var series1 = new OxyPlot.Series.LineSeries { Title = "Portefeuille", MarkerType = MarkerType.None };
            for (int i =0; i<valPortef.Length; i++) {
                series1.Points.Add(new DataPoint(i, valPortef[i]));
            }

            var series2 = new OxyPlot.Series.LineSeries { Title = "Benchmark", MarkerType = MarkerType.None };
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
            var tmp = new PlotModel { Title = "Rendements du portefeuille vs Benchmark" };

            var series1 = new OxyPlot.Series.LineSeries { Title = "Portefeuille", MarkerType = MarkerType.None };
            for (int i = 0; i < valPortef.Length; i++)
            {
                series1.Points.Add(new DataPoint(i, valPortef[i]));
            }

            var series2 = new OxyPlot.Series.LineSeries { Title = "Benchmark", MarkerType = MarkerType.None };
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

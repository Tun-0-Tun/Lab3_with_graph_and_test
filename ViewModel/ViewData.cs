using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab3;
using static Lab3.V1DataArray;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using OxyPlot;
using System.Windows.Input;
using ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Diagnostics;
using OxyPlot.Series;
using OxyPlot.Legends;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ViewModel
{

    public class FuncCollection
    {
        public static List<FValue> FuncList { get; set; } = new List<FValue>() { F1, F2 };

        public static double[] F1(double x)
        {
            double y1 = x * x * x;
            double y2 = x;
            return new double[] { y1, y2 };
        }
        public static double[] F2(double x)
        {
            double y1 = x * x;
            double y2 = x ;
            return new double[] { y1, y2 };
        }
        public static double startF(double x) => x + 1; //Начальное приближение
    }

    public interface IUIServices
    {
        void ReportError(string message);
    }
    public class ViewData: ViewModelBase
    {

        // Maximum number of iterations 
        public int MaxIterations { get; set; }

        // Value of the discrepancy norm for termination 
        public double DiscrepancyRate { get; set; }

        // Number of nodes of a uniform grid on which spline values ​​are calculated 
        public int UniformNum { get; set; }

        // Number of nodes for smoothing spline (for construction) 
        public int SmoothingSplineNum { get; set; }

        // Function ID for initialization 
        public int FuncId { get; set; }

        // Whether the grid is uniform or non-uniform 
        public bool IsGridUniform { get; set; }

        // Number of grid nodes 
        public int NodesNum { get; set; }

        // Segment boundaries with grid nodes 
        public double[] GridBoundaries { get; set; }

        // Reference to V1DataArray 
        public V1DataArray? DataArrayObj;

        // Reference to SplineData 
        public SplineData? SplineDataObj;

        public MainPlot plotModel { get; set; }

        private readonly IUIServices uiServices;

        public ICommand RunCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public IEnumerable<string> Table_listbox_rawData
        {
            //get => this.SplineDataObj.ResSpline != null ? (this.SplineDataObj.ResSpline).Zip((rawData.y), (first, second) => to_str(first, second, "##00.000")) : null;
            get => this.SplineDataObj != null ? this.SplineDataObj.ResSpline.Zip(this.SplineDataObj.ResSpline, (first, second) => first.ToString("f6")) : null;
        }

        public IEnumerable<string> Table_secondData
        {
            //get => this.SplineDataObj.ResSpline != null ? (this.SplineDataObj.ResSpline).Zip((rawData.y), (first, second) => to_str(first, second, "##00.000")) : null;
            get => this.SplineDataObj != null ? this.SplineDataObj.ResultOnAddonGrid.Zip(this.SplineDataObj.ResultOnAddonGrid, (first, second) => first[0].ToString() +"    " + first[1].ToString()) : null;
        }



        public ViewData(IUIServices uiServices)
        {
            GridBoundaries = new double[2] {2, 5};
            NodesNum = 100;
            IsGridUniform = false;
            FuncId = 0;
            SmoothingSplineNum = 50;
            UniformNum = 130;
            DiscrepancyRate = 1e-6;
            MaxIterations = 1000;
            this.uiServices = uiServices;


            DataArrayObj = null;
            SplineDataObj = null;
            RunCommand = new RelayCommand(DataFromControlsItem_Click, CheckControlsItem_Click);
            LoadCommand = new RelayCommand(DataFromFileItem_Click);
            SaveCommand = new RelayCommand(SaveItem_Click, CheckSaveItem_Click);
        }

        private void SaveItem_Click(object sender)
        {
            try
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                string FilePath = "";
                if (saveFileDialog.ShowDialog() == true)
                {
                    FilePath = saveFileDialog.FileName;
                }
                this.InitializeThroughControl();
                this.Save(FilePath);
            }
            catch (Exception ex)
            {
                uiServices.ReportError(ex.Message);
            }
        }

        private void DataFromControlsItem_Click(object sender)
        {
            try
            {
                this.InitializeThroughControl();
                this.InitializeData();
                this.Calculation();
                this.plotModel = new MainPlot(this.SplineDataObj);
                RaisePropertyChanged(nameof(Table_listbox_rawData));
                RaisePropertyChanged(nameof(Table_secondData));
                RaisePropertyChanged(nameof(ChartData));

            }
            catch (Exception ex)
            {
                uiServices.ReportError(ex.Message);
            }
        }


        public bool CheckControlsItem_Click(object sender)
        {
            try
            {
                bool result = true;
                string? error = null;
                string[] error_types = { "arrayNodesAmount", "splineCalculationNodesAmount", "left_border", "splineNodesAmount" };
                foreach (var field in error_types)
                {
                    error = this[field];
                    result = error == null && result;
                }
                //e.CanExecute = result;
                return result;
            }
            catch (Exception ex)
            {
                //e.CanExecute = false;
                return false;
            }
        }
        private void DataFromFileItem_Click(object sender)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string FilePath = "";
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath = openFileDialog.FileName;
                }
                this.Load(FilePath);
                this.InitializeData();
                this.Calculation();
                RaisePropertyChanged(nameof(Table_listbox_rawData));
                RaisePropertyChanged(nameof(Table_secondData));
                RaisePropertyChanged(nameof(ChartData));
            }
            catch (Exception ex)
            {
                uiServices.ReportError(ex.Message);
            }
        }



        private bool CheckSaveItem_Click(object sender)
        {
            try
            {
                if (this.DataArrayObj == null) return false;
                else {return true; }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void Calculation()
        {
            Func<double, double> fInit = FuncCollection.startF;
            SplineDataObj.CalcSpline(SplineDataObj, fInit, IsGridUniform);
        }

        public void InitializeData()
        {
            if (SmoothingSplineNum <= 1)
            {
                throw new Exception("Invalid number of nodes for smoothing spline");
            }
            if (UniformNum <= 1)
            {
                throw new Exception("Invalid number of nodes for uniform grid");
            }
            SplineDataObj = new SplineData(DataArrayObj, SmoothingSplineNum, MaxIterations, UniformNum);
        }


        public void InitializeThroughControl()
        {
            FValue f = FuncCollection.FuncList[FuncId];

            if (GridBoundaries[1] == 0 || GridBoundaries[1] <= GridBoundaries[0])
            {
                throw new Exception("Invalid segment boundaries");
            }

            if (NodesNum <= 1)
            {
                throw new Exception("Invalid number of nodes");
            }

            DataArrayObj = new V1DataArray("Test", DateTime.Now, NodesNum, GridBoundaries[0], GridBoundaries[1], f, IsGridUniform);
        }

        public bool Save(string filename)
        {
            return DataArrayObj.Save(filename);
        }

        public bool Load(string filename)
        {
            DataArrayObj = new V1DataArray("", new DateTime());
            return V1DataArray.Load(filename, ref DataArrayObj);
        }

        public void Print(string text)
        {
            Console.WriteLine(text);
        }

        public string this[string propertyName] => propertyName switch
        {
            "arrayNodesAmount" => NodesNum < 3 ? "The number of grid nodes where discrete function values are defined must be greater than or equal to 3" : null,
            "splineCalculationNodesAmount" => UniformNum < 3 ? "The number of nodes in the uniform grid where spline values are calculated must be greater than or equal to 3" : null,
            "left_border" or "right_border" => GridBoundaries[0] >= GridBoundaries[1] ? "The left boundary of the interval must be less than the right boundary" : null,
            "splineNodesAmount" => SmoothingSplineNum < 2 ||  (SmoothingSplineNum > NodesNum)  ? "The number of nodes in the smoothing spline must be greater than or equal to 2 and less than or equal to the number of defined discrete function values" : null,
            _ => null
        };

        public PlotModel ChartData
        {
            get
            {
                PlotModel pm = new PlotModel { Title = "Graph" };
                pm.Series.Clear();

                pm.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    MaximumPadding = 0.1,
                    MinimumPadding = 0.1,
                    StringFormat = "##0.00",
                    Title = "x"
                });
                pm.Axes.Add(new OxyPlot.Axes.LinearAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                    MaximumPadding = 0.1,
                    MinimumPadding = 0.1,
                    StringFormat = "##0.00",
                    Title = "y"
                });

                if (SplineDataObj == null)
                    return pm;

                for (int js = 0; js < 2; js++)
                {
                    OxyColor color = (js == 0) ? OxyColors.Transparent : OxyColors.Red;
                    LineSeries lineSeries = new LineSeries();
                    if (js == 0)
                    {
                        for (int j = 0; j < SplineDataObj.DArray.XArray.Length; j++) lineSeries.Points.Add(new DataPoint(SplineDataObj.DArray.XArray[j], SplineDataObj.DArray[0][j]));
                        lineSeries.MarkerType = MarkerType.Circle;
                        lineSeries.MarkerSize = 4;
                        lineSeries.MarkerStroke = OxyColors.Green;
                        lineSeries.MarkerFill = OxyColors.Green;
                        lineSeries.Title = "Data";
                    }
                    else
                    //{
                    //    for (int j = 0; j < nGrid; j++)
                    //        lineSeries.Points.Add(new DataPoint(splineData.data[j].x, splineData.data[j].y));
                    //    lineSeries.Title = "Spline Data";
                    //}
                    {
                        var splinePoints = SplineDataObj.ResultOnAddonGrid;
                        foreach (var splinePoint in splinePoints)
                        {
                            lineSeries.Points.Add(new DataPoint(splinePoint[0], splinePoint[1]));
                        }
                        lineSeries.Title = "ResultOnAddonGrid";
                    }
                    lineSeries.Color = color;
                    Legend legend = new Legend() { LegendPosition = LegendPosition.LeftTop };
                    pm.Legends.Add(legend);
                    pm.Series.Add(lineSeries);
                }
                return pm;
            }
        }



    }
}

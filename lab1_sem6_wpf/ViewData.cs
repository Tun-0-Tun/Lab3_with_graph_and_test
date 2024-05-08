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

namespace lab1_sem6_wpf
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
    public class ViewData
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



        public ViewData()
        {
            GridBoundaries = new double[2] {2, 5};
            NodesNum = 100;
            IsGridUniform = true;
            FuncId = 0;
            SmoothingSplineNum = 50;
            UniformNum = 130;
            DiscrepancyRate = 1e-6;
            MaxIterations = 1000;


            DataArrayObj = null;
            SplineDataObj = null;
        }

       

        public void Calculation()
        {
            Func<double, double> fInit = FuncCollection.startF;
            SplineData.CalcSpline(SplineDataObj, fInit, IsGridUniform);
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
            "splineNodesAmount" => SmoothingSplineNum < 2 || SmoothingSplineNum > UniformNum ? "The number of nodes in the smoothing spline must be greater than or equal to 2 and less than or equal to the number of defined discrete function values" : null,
            _ => null
        };

    }
}

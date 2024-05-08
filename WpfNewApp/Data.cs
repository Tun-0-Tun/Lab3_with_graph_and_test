using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNewApp
{


    public class FuncCollection
    { 

        public static double[] F1(double x)
        {
            double y1 = x * x * x;
            double y2 = x;
            return new double[] { y1, y2 };
        }
        public static double[] F2(double x)
        {
            double y1 = x * x;
            double y2 = x;
            return new double[] { y1, y2 };
        }
        public static double startF(double x) => x + 1; //Начальное приближение
    }
    public class Data: IDataErrorInfo
    {
        public int NodesNum { get; set; }
        public int LeftBound { get; set; }

        public double[] Xarray { get; set; }

        public double[] Yarray { get; set;}

        public MainPlot plotModel { get; set; }

        public string Error => throw new NotImplementedException();

        public Data()
        {
            LeftBound = 1;
            NodesNum = 20;
            Xarray = new double[NodesNum];
            Yarray = new double[NodesNum];

            for (int i = 0; i < NodesNum; i++)
            {
                double step = LeftBound / NodesNum;
                Xarray[i] = i * step;
                Yarray[i] = (i * step) * (i * step);
            }
        }

        public void Calculation()
        {
            Xarray = new double[NodesNum];
            Yarray = new double[NodesNum];
            for (int i = 0; i < NodesNum; i++)
            {
                double step = LeftBound / NodesNum;
                Xarray[i] = i * step;
                Yarray[i] = (i * step) * (i * step);
            }
        }

        public string this[string propertyName] => propertyName switch
        {
            "arrayNodesAmount" => NodesNum < 3 || NodesNum > 100 ? "The number of grid nodes where discrete function values are defined must be greater than or equal to 3" : null,
            "splineCalculationNodesAmount" => LeftBound < 0 || LeftBound > 1 ? "The number of nodes in the uniform grid where spline values are calculated must be greater than or equal to 3" : null,
            _ => null
        };

    }


}

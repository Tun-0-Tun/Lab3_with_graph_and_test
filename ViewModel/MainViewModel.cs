using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab3;

namespace ViewModel
{

    using System;

    using OxyPlot;
    using OxyPlot.Legends;
    using OxyPlot.Series;
    class MainViewModel
    {

        public SplineData data { get; set; }
        //public PlotModel CurplotModel { get; private set; }
        public MainViewModel()
        {
            this.MyModel = new PlotModel { Title = "Example 1" };
            this.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        public void plot(SplineData data)
        {
            this.data = data;
            //this.MyModel = new PlotModel { Title = "Approximation results" };
            AddSeries();
        }
        private void AddSeries()
        {
            MyModel.Series.Clear();
            OxyColor color = OxyColors.Red;
            LineSeries lineSeries = new LineSeries();
            var points = data.DArray.XArray;
            var values = data.DArray[0];
            for (int i = 0; i < points.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(points[i], values[i]));
            }
            lineSeries.MarkerType = MarkerType.Diamond;
            lineSeries.Color = OxyColors.Transparent;
            lineSeries.MarkerSize = 5;
            lineSeries.MarkerFill = color;
            lineSeries.MarkerStroke = color;
            lineSeries.MarkerStrokeThickness = 2.3;
            lineSeries.Title = "Дискретные значения функции";
            Legend legend = new Legend { LegendPosition = LegendPosition.LeftTop, LegendPlacement = LegendPlacement.Inside };
            MyModel.Legends.Add(legend);
            MyModel.Series.Add(lineSeries);

            lineSeries = new LineSeries();
            var splinePoints = data.ResultOnAddonGrid;
            foreach (var splinePoint in splinePoints)
                lineSeries.Points.Add(new DataPoint(splinePoint[0], splinePoint[1]));
            OxyColor splineColor = OxyColors.Blue;
            lineSeries.Color = splineColor;
            lineSeries.MarkerType = MarkerType.Circle;
            lineSeries.MarkerSize = 3;
            lineSeries.MarkerFill = splineColor;
            lineSeries.MarkerStroke = splineColor;
            lineSeries.Title = "Аппроксимирующий сплайн";

            legend = new Legend { LegendPosition = LegendPosition.LeftTop, LegendPlacement = LegendPlacement.Inside };
            MyModel.Legends.Add(legend);
            MyModel.Series.Add(lineSeries);
        }

    public PlotModel MyModel { get; private set; }
    }
}


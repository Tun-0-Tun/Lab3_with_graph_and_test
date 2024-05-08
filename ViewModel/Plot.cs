using Lab3;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    using System;

    using OxyPlot;
    using OxyPlot.Series;
    public class MainPlot
    {
        public SplineData data { get; set; }


        public PlotModel CurplotModel { get; private set; }
        public MainPlot(SplineData data)
        {
            this.data = data;
            this.CurplotModel = new PlotModel { Title = "Approximation" };
            AddSeries();
        }
        private void AddSeries()
        {
            CurplotModel.Series.Clear();

            // Add Discrete Function Values Series
            AddDiscreteFunctionSeries();

            // Add Approximating Spline Series
            AddApproximatingSplineSeries();

            AddLegend();
        }

        private void AddDiscreteFunctionSeries()
        {
            OxyColor color = OxyColors.Green;
            LineSeries lineSeries = new LineSeries
            {
                Title = "Discrete Function Values",
                MarkerType = MarkerType.Triangle, // Changed marker type to square
                MarkerSize = 5,
                MarkerFill = color,
                MarkerStroke = color,
                MarkerStrokeThickness = 1.3,
                Color = OxyColors.Transparent
            };

            var points = data.DArray.XArray;
            var values = data.DArray[0];
            for (int i = 0; i < points.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(points[i], values[i]));
            }

            CurplotModel.Series.Add(lineSeries);
        }

        private void AddApproximatingSplineSeries()
        {
            OxyColor splineColor = OxyColors.Blue;
            LineSeries lineSeries = new LineSeries
            {
                Title = "Approximating Spline",
                MarkerType = MarkerType.Circle, // Changed marker type to cross
                MarkerSize = 3, // Increased marker size for better visibility
                Color = splineColor
            };

            var splinePoints = data.ResultOnAddonGrid;
            foreach (var splinePoint in splinePoints)
            {
                lineSeries.Points.Add(new DataPoint(splinePoint[0], splinePoint[1]));
            }

            CurplotModel.Series.Add(lineSeries);
        }

        private void AddLegend()
        {
            Legend legend = new Legend { LegendPosition = LegendPosition.LeftTop, LegendPlacement = LegendPlacement.Inside };
            CurplotModel.Legends.Add(legend);
        }
    }
}


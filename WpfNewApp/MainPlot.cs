using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNewApp
{
    using System;

    using OxyPlot;
    using OxyPlot.Legends;
    using OxyPlot.Series;
    public class MainPlot
    {
        public Data data { get; set; }


        public PlotModel CurplotModel { get; private set; }
        public MainPlot(Data data)
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

            var points = data.Xarray;
            var values = data.Yarray;
            for (int i = 0; i < points.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(points[i], values[i]));
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

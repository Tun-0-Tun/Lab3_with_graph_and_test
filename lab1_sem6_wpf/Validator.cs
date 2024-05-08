using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Lab3;

namespace lab1_sem6_wpf
{
    internal class ParseBoundaries : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double[] seg = (double[])value;
            return seg[0].ToString() + " " + seg[1].ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string str = ((string)value);
                return ParseString(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                double[] k = { 0, 0 };
                return k;
            }
        }

        private double[] ParseString(string str)
        {
            int len = str.Length;
            double[] edges = new double[2];
            string[] numbers = str.Split(' ');

            if (numbers.Length != 2)
            {
                throw new Exception("Некорректный ввод");
            }

            edges[0] = double.Parse(numbers[0]);
            edges[1] = double.Parse(numbers[1]);

            return edges;
        }
    }
}

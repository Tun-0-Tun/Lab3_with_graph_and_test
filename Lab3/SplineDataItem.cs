using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class SplineDataItem
    {
        public double nodeValue {  get; set; }
        public double realValue{ get; set; }
        public double resultValue { get; set; }

        public SplineDataItem(double netValue, double exactValue, double calculatedValue)
        {
            this.nodeValue = netValue;
            this.realValue = exactValue;
            this.resultValue = calculatedValue;
        }
        public string ToString(string format)
        {
            return $"{nodeValue.ToString(format)},            {realValue.ToString(format)},          {resultValue.ToString(format)}";
        }
        public override string ToString()
        {
            return $"{nodeValue.ToString()}, {realValue.ToString()}, {resultValue.ToString()}";
        }
    }
}

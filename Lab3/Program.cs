using Lab3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Lab3
{
    public static class Programm
    {

        //public static double FUNC1(double x) => 17*x + 6;
        public static double FUNC2(double x) => 2 * x * x * x + 3 * x * x + 6;
        public static double[] FUNC1(double x)
        {
            var ret = new double[2];
            ret[0] = x * x;
            ret[1] = x * x;

            return ret;
        }
        public static double FUNC3(double x) => 5 * x + 14;
    }
}

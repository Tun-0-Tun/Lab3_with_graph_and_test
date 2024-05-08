using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class SplineData
    {
        public V1DataArray DArray { get; set; }
        public int NodeCnt {  get; set; }
        public double[] ResultNonUniformSpline { get; set; }
        public int MaxIterations { get; set; }
        public List<SplineDataItem> ResSpline { get; set; }
        public int ActualNumberOfIterations { get; set; }
        public double MinDisp { get; set; }

        //public List<SplineDataItem> ResultOnAddonGrid { get; set; }
        public List<double[]> ResultOnAddonGrid { get; set; }
        public int StopReason { get; set; }
        public int UniformNum {get; set; }



        // Импорт нашего cpp модуля
        [DllImport("C:\\Users\\alexp\\source\\repos\\Lab3_Pevtsov\\Build\\x64\\Debug\\dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SplineInterpolation(int numPoints, 
            double[] Points,
            int NumValues,
            double[] Values,
            int NodeSize,
            double[] UniformGridValues,
            double[] SplineValues,
            ref int stop_reason,
            int maxIterations,
            ref int ActualNumberOfIterations,
            double[] addGrid,
            double[] addSplineData,
            int addSize);

      

        public SplineData(V1DataArray Array, int NodeSize, int MaxIterations, int UniformNum)
        {
            this.DArray = Array;
            this.NodeCnt = NodeSize;
            this.MaxIterations = MaxIterations;
            this.ResultNonUniformSpline = new double[this.DArray.XArray.Length];
            this.UniformNum = UniformNum;
            ResSpline = new List<SplineDataItem>();
            ResultOnAddonGrid = new List<double[]>();
        }
        public static double[] uniformGrid(double left_border, double right_border, int num_of_dots)
        {
            double step = (right_border - left_border) / (num_of_dots - 1);
            double[] ret = new double[num_of_dots];
            for (int i = 0; i < num_of_dots; i++)
            {
                ret[i] = left_border + step * i;
            }
            return ret;
        }

        public static double[] NonUniformGrid(double leftBorder, double rightBorder, int numOfDots)
        {
            Random random = new Random();
            double[] ret = new double[numOfDots];
            ret[0] = leftBorder;
            ret[numOfDots - 1] = rightBorder;

            for (int i = 1; i < numOfDots - 1; i++)
            {
                double range = (rightBorder - ret[i - 1]) / (numOfDots - i); // Уменьшаем диапазон по мере продвижения к правой границе
                double randomValue = random.NextDouble() * range;
                double nextValue = ret[i - 1] + randomValue;
                ret[i] = nextValue;
            }
            Array.Sort(ret); // Сортировка массива точек по возрастанию
            return ret;
        }

        public void CalcSpline(SplineData splineData, Func<double, double> initialApproximationFunc, bool isUniform)
        {
            int stopReasonLocal = 0;
            double[] valuesOnGrid;
            if (isUniform == true)
            {
                valuesOnGrid = uniformGrid(splineData.DArray[0][0],
                                                        splineData.DArray[0][splineData.DArray[0].Length - 1],
                                                        splineData.NodeCnt);
            }
            else
            {
                valuesOnGrid = NonUniformGrid(splineData.DArray[0][0],
                                                        splineData.DArray[0][splineData.DArray[0].Length - 1],
                                                        splineData.NodeCnt);
            }
            
            for (int i = 0; i < valuesOnGrid.Length; ++i)
            {
                valuesOnGrid[i] = initialApproximationFunc(valuesOnGrid[i]);
            }
            int actualIterations = 0;


            double[] adduniformGrid;

            
            adduniformGrid = uniformGrid(splineData.DArray.XArray[0],
                                                   splineData.DArray.XArray[splineData.DArray.XArray.Length - 1],
                                                   splineData.UniformNum);
            
           
            double[] valuesOnAdduniformGrid = new double[splineData.UniformNum];
            SplineInterpolation(splineData.DArray.XArray.Length,
                                splineData.DArray.XArray,
                                splineData.DArray[0].Length,
                                splineData.DArray[0],
                                splineData.NodeCnt,
                                valuesOnGrid,
                                splineData.ResultNonUniformSpline,
                                ref stopReasonLocal,
                                splineData.MaxIterations,
                                ref actualIterations,
                                adduniformGrid,
                                valuesOnAdduniformGrid,
                                splineData.UniformNum);
            splineData.MinDisp = 0;
            for (int i = 0; i < splineData.ResultNonUniformSpline.Length; ++i)
            {
                splineData.MinDisp += (splineData.ResultNonUniformSpline[i] - splineData.DArray[0][i]) *
                                      (splineData.ResultNonUniformSpline[i] - splineData.DArray[0][i]);
                splineData.ResSpline = splineData.ResSpline.Append(new SplineDataItem(splineData.DArray.XArray[i],
                                                                                      splineData.DArray[0][i],
                                                                                      splineData.ResultNonUniformSpline[i])).ToList();
            }
            splineData.MinDisp = Math.Sqrt(splineData.MinDisp);
            splineData.StopReason = stopReasonLocal;
            splineData.ActualNumberOfIterations = actualIterations;


            // Add grid cnt
            for (int i = 0; i < valuesOnAdduniformGrid.Length; ++i)
            {
                splineData.ResultOnAddonGrid = splineData.ResultOnAddonGrid.Append(new double[]{ adduniformGrid[i],
                                                                                          valuesOnAdduniformGrid[i]}).ToList();
            }
        }

        public string ToLongString(string format)
        {
            var output = new StringBuilder();

            output.AppendLine(DArray.ToLongString(format));
            output.AppendLine("Spline approximation results:");

            foreach (var item in ResSpline)
            {
                output.AppendLine(item.ToString(format));
            }

            output.AppendLine();
            output.AppendLine($"Minimal residual value: {MinDisp}");
            output.Append("Stop reason: ");

            // Создаем словарь для хранения строковых представлений причин останова
            Dictionary<int, string> stopReasons = new Dictionary<int, string>
                {
                    { 1, "specified number of iterations has been exceeded" },
                    { 2, "specified trust region size has been reached" },
                    { 3, "specified residual norm has been reached" },
                    { 4, "the specified row norm of the Jacobian matrix has been reached" },
                    { 5, "specified trial step size has been reached" },
                    { 6, "the specified difference between the norm of the function and the error has been reached" }
                };

            // Проверяем, есть ли значение StopReason в словаре
            if (stopReasons.ContainsKey(StopReason))
            {
                output.AppendLine(stopReasons[StopReason]);
            }
            else
            {
                output.AppendLine($"Unknown stop reason: {StopReason}");
            }

            output.AppendLine($"Actual number of iterations: {ActualNumberOfIterations}");

            return output.ToString();
        }
        public bool Save(string filename, string format)
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(filename))
                {
                    fs.WriteLine(ToLongString(format));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }


        

    }
}
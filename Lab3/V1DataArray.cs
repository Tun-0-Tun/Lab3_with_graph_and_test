using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lab3 {
    public class V1DataArray : V1Data {
        public override IEnumerator<DataItem> GetEnumerator(){
            for (int i=0; i < XArray.Length; ++i){
                yield return new DataItem(XArray[i], FieldGrid[0][i], FieldGrid[1][i]);
            }
        }

        public bool Save(string filename)
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(filename))
                {
                    string s = JsonSerializer.Serialize(this.ObjectKey);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(this.Date);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(this.XArray);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(this.FieldGrid[0]);
                    fs.WriteLine(s);
                    s = JsonSerializer.Serialize(this.FieldGrid[1]);
                    fs.WriteLine(s);
               
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Console.WriteLine("Saving done");
            }
            return true;
        }

        public static bool Load(string filename, ref V1DataArray array)
        {
            try
            {
                using (StreamReader fs = new StreamReader(filename))
                {
                    string s = fs.ReadLine();
                    array.ObjectKey = JsonSerializer.Deserialize<string>(s);
                    s = fs.ReadLine();
                    array.Date = JsonSerializer.Deserialize<DateTime>(s);
                    s = fs.ReadLine();
                    array.XArray = JsonSerializer.Deserialize<double[]>(s);
                    s = fs.ReadLine();
                    var tmp1 = JsonSerializer.Deserialize<double[]>(s);
                    s = fs.ReadLine();
                    var tmp2 = JsonSerializer.Deserialize<double[]>(s);
                    array.FieldGrid = new double[2][]{tmp1, tmp2};
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Console.WriteLine("Reading done");
            }
            return true;
        }

        public double[] XArray { get; set; }
        public double[][] FieldGrid { get; set; }

        public delegate double[] FValue(double x);

        public V1DataArray(string key, DateTime date) : base(key, date) {
            XArray = Array.Empty<double>();
            FieldGrid = new double [2][];
            FieldGrid[0] = Array.Empty<double>();
            FieldGrid[1] = Array.Empty<double>();
        }

        public V1DataArray(string key, DateTime date, double[] x, FValue transform) : base(key, date) {
            x = x.Distinct().ToArray();
            
            XArray = new double[x.Length];
        
            Array.Copy(x, XArray, x.Length);

            FieldGrid = new double[2][];
            FieldGrid[0] = new double[x.Length];
            FieldGrid[1] = new double[x.Length];

            for (int i = 0; i < x.Length; i++) {
                var tmp = transform(x[i]);
                FieldGrid[0][i] = tmp[0];
                FieldGrid[1][i] = tmp[1];
            }
        }

        public V1DataArray(string key, DateTime date, int nX, double xL, double xR, FValue transform, bool isUniform) : base(key, date)
        {
            XArray = new double[nX];
            FieldGrid = new double[2][];
            FieldGrid[0] = new double[nX];
            FieldGrid[1] = new double[nX];

            if (isUniform)
            {
                double step = (xR - xL) / (nX - 1);

                for (int i = 0; i < nX; ++i)
                {
                    XArray[i] = xL + step * i;
                    var tmp = transform(xL + step * i);

                    FieldGrid[0][i] = tmp[0];
                    FieldGrid[1][i] = tmp[1];
                }
            }
            else
            {
                Random rand = new Random();

                double previousPoint = xL;

                // Остальные точки
                for (int i = 0; i < nX; ++i)
                {
                    double maxStep = (xR - previousPoint) / (nX - i); // ограничиваем максимальный шаг
                    double step = rand.NextDouble() * maxStep;
                    previousPoint += step;
                    XArray[i] = previousPoint;
                    var tmp = transform(XArray[i]);

                    FieldGrid[0][i] = tmp[0];
                    FieldGrid[1][i] = tmp[1];
                }
            }
        }

        public double[] this[int index] {
            get => FieldGrid[index];
        }

        public V1DataList Property {
            get { return new V1DataList(ObjectKey, Date); }
        }

        public override double MaxDistance {
            get {
                var allX = new double[XArray.Length];

                for (var i = 0; i < XArray.Length; i++) {
                    var xI = XArray[i];
                    allX[i] = xI;
                }

                Array.Sort(allX);
                return allX.Last() - allX.First();
            }
        }

        public override string ToString() {
            return $"V1DataArray {base.ToString()} "; //itemsList.Count.ToString();
        }

        public override string ToLongString(string format) {
            var builder = new StringBuilder();
            builder.Append('\n');
            for (int i = 0; i < XArray.Length; ++i) {
                builder.Append($"{XArray[i].ToString(format)} {FieldGrid[0][i]} {FieldGrid[1][i]} \n");
            }

            return $"{this} {builder}"; //+ ret_value; //то же, что и ToString + доп данные}
        }
    }
}
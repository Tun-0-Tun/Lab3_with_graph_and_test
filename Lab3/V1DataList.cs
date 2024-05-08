using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3 {
    public delegate double[] FDI(double x);

    public class V1DataList : V1Data {
        public override IEnumerator<DataItem> GetEnumerator(){
            return ItemsList.GetEnumerator();
        }
        public FDI Transform { get; set; } = value => new[] { value };
        private static double[] _givenPoints = Array.Empty<double>();
    
        private List<DataItem> ItemsList { get; }

        public V1DataList(string key, DateTime date) : base(key, date) {
            ItemsList = new List<DataItem>(); //распределение
        }

        public V1DataList(string key, DateTime date, double[] x, FDI transform) : this(key, date) {
            Transform = transform;
            _givenPoints = x;

            foreach (var xi in x.Distinct().ToArray()) {
                var field = transform(xi);
                ItemsList.Add(new DataItem(xi, field[0], field[1]));
            }
        }


        public override double MaxDistance {
            get {
                var allX = new double[ItemsList.Count];

                for (var i = 0; i < ItemsList.Count; i++) {
                    allX[i] = ItemsList[i].X;
                }

                Array.Sort(allX);
                return allX.Last() - allX.First();
            }
        }

        public static explicit operator V1DataArray(V1DataList source) {
            return new V1DataArray(source.ObjectKey, source.Date, _givenPoints, (double value) => source.Transform(value));
        }

        public override string ToString() {
            return $"V1DataList {base.ToString()} {ItemsList.Count}";
        }

        public override string ToLongString(string format) {
            var output = new StringBuilder();

            foreach (var i in ItemsList) {
                output.Append($"{i.ToLongString(format)}\n");

            }

            return $"{this}\n{output}"; //то же, что и ToString + доп данные
        }
    }
}
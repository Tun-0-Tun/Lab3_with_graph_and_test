using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace Lab3 {
    public class V1MainCollection : System.Collections.ObjectModel.ObservableCollection<V1Data> {

        public double Mean{
            
            get {
                var tmp = (from i in Items
                                from j in i select Math.Sqrt(j.Y1 * j.Y1 + j.Y2 * j.Y2));
                if (!tmp.Any()){
                    return double.NaN;
                }                
                var res = tmp.Sum() / tmp.Count();
                return res;
            }
        }

        public DataItem? MaxDisp{
            get {
                var m = Mean;
                if (Items.Count() == 0){
                    return null;
                }
                var tmp = (from i in Items
                                from j in i select j).OrderBy(x => Math.Abs(Math.Sqrt(x.Y1 * x.Y1 + x.Y2 * x.Y2) - m));
                
                return tmp.Last();
            }
        }
        public IEnumerable<double>? Doubled_x{
            get {
                if (Items.Count() == 0){
                    return null;
                }
                
                var all_x = (from i in Items from j in i where i is V1DataList select j.X);
                var tmp  = (from i in Items from j in i where (all_x.Count(y => y == j.X) > 1) select j.X);

                //Console.WriteLine(all_x.Count() + "<------");
                return tmp;

            }
        }

        

        public static double[] SquaredArray(double x) {
            var ret = new double[2];

            ret[0] = x * x;
            ret[1] = x * x;

            return ret;
        }
        public bool Contains(string key) {
            return Items.Any(data => data.ObjectKey == key);
        }

        public bool Add(V1Data data) {
            foreach (var item in Items) {
                if (item.ObjectKey == data.ObjectKey) {
                    return false;
                }
            }

            base.Add(data);
            return true;
        }



        public V1MainCollection(int nV1DataArray, int nV1DataList) {
            double[] x = new double[3];
            for (int i = 0; i < nV1DataArray; ++i){
                x[0] = 1.2 + i;
                x[1] = 0 + i;
                x[2] = 5.5 + i * i;
                V1DataArray v1_a = new V1DataArray("a" + i, DateTime.Today,x,SquaredArray);
                Add(v1_a);
            }
            for (int i = 0; i < nV1DataList; ++i){
                x[0] = 1.2 - i;
                x[1] = 0 - i;
                x[2] = 5.5 - i;
                V1DataList v1_b = new V1DataList("d" + i, DateTime.Today,x,SquaredArray);
                Add(v1_b);
            }
        }

        public string ToLongString(string format) {
            var output = new StringBuilder();


            foreach (var item in Items) {
                output.Append($"{item.ToLongString(format)}\n");
            }

            return output.ToString();
        }

        public override string ToString() {
            var items = new StringBuilder();
            
            foreach (var item in Items) {
                items.Append($"{item}\n"); 
            }

            return items.ToString();
        }
    }
}
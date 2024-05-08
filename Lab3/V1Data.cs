using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace Lab3
{
    public abstract class V1Data: IEnumerable<DataItem>
    {
        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator(){
             return GetEnumerator();
        }
        public string ObjectKey { get; set;}
        public DateTime Date { get; set;}

        public V1Data(string ObjectKey, DateTime Date) 
        {
            this.ObjectKey = ObjectKey;
            this.Date = Date;
        }
        
        public abstract double MaxDistance { get; }
        public abstract string ToLongString(string format);
        public virtual string ToString(string format) 
        {
             return $"{ObjectKey} {Date}";
        }
    }
}
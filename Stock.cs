using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sym
{
    public class Stock
    {
        public static int Limit = 1000;
        public string Code;
        public List<Rec> HistoricalData = new List<Rec>();
        public double[] CloseData;
        public Stock(string fileName, string date = null)
        {
            DateTime limitDate = DateTime.Now;
            if (date != null)
            {
                limitDate = DateTime.Parse(date);
            }

            this.Code = System.IO.Path.GetFileNameWithoutExtension(fileName);

            using(System.IO.StreamReader sr = new System.IO.StreamReader(fileName))
            {
                int cnt = 0;
                bool isHedaer = true;
                while(sr.Peek() != -1)
                {
                    if (isHedaer)
                    {
                        sr.ReadLine();
                        isHedaer = false;
                    }


                    if (cnt > Limit) break;
                    string[] a = sr.ReadLine().Split(',');

                    DateTime d = DateTime.Parse(a[0]);

                    if (limitDate < d)
                    {
                        continue;
                    }

                    double ac = double.Parse(a[6]);
                    double c = double.Parse(a[4]);
                    double o = double.Parse(a[1]) * ac / c;
                    double h = double.Parse(a[2]) * ac / c;
                    double l = double.Parse(a[3]) * ac / c;
                    double v = double.Parse(a[5]);
                    c = ac;

                    HistoricalData.Insert(0, new Rec { Date = d, Open = o, Hight = h, Low = l, Close = c, Volume = v });
                    cnt++;
                }
            }

        }
        public double[] GetValues(int index, int len)
        {
            var newList = this.HistoricalData.Skip(index - len).Take(len);

            return newList.Select(x => x.Close).ToArray();
        }
        public int GetValueIndex(DateTime date)
        {
            int cnt = 0;
            foreach (var rec in this.HistoricalData)
            {
                if (rec.Date >= date)
                {
                    return cnt;
                }
                cnt++;
            }

            return -1;
        }

        
        public Stock.Rec GetValue(DateTime date)
        {
            int index = GetValueIndex(date);

            if (index == -1) return null; 
                
            return this.HistoricalData[index];
        }
        public class Rec
        {
            public DateTime Date;
            public double Open;
            public double Hight;
            public double Low;
            public double Close;
            public double Volume;
        }
    }
}

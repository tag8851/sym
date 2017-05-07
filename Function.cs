using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sym
{
    class Function
    {
        public class LinerResult
        {
            public double a;
            public double b;
            public double r;
        }
        public static double GetDiffFromBolinger(double stdev, double ma, double value)
        {
            return (value - ma) / stdev;
        }
        public static double GetDiffFromMA(double ma, double value)
        {
            return value / ma - 1; 
        }
        public static double[] Normalize(double[] array)
        {
            double[] newArray = new double[array.Length];

            newArray[0] = 1;
            for(int i = 1; i < array.Length; i++)
            {
                newArray[i] = array[i] / array[0];
            }
            return newArray;
        }
        public static LinerResult Linest(double[] y)
        {
            var x = new double[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                x[i] = i;
            }

            return Linest(x, y);
        }
        public static LinerResult Linest(double[] array1, double[] array2)
        {
            double x = 0;
            double y = 0;
            double xy = 0;
            double x2 = 0;
            double xx = 0;
            double y2 = 0;
            double n = array1.Length;

            for (int i = 0; i < n; i++)
            {
                x2 += array1[i] * array1[i];
                y2 += array2[i] * array2[i];
                xy += array1[i] * array2[i];
                x += array1[i];
                y += array2[i];
            }
            double r1 = ((n * xy) - (x * y)) / Math.Sqrt((n * x2 - (x * x)) * (n * y2 - (y * y)));

            return new LinerResult
            {
                a = ((n * xy) - (x * y)) / ((n * x2) - (x * x)),
                b = ((x2 * y) - (xy * x)) / ((n * x2) - (x * x)),
                r = r1 * r1
            };
        }
        public static double Stddev(double[] array)
        {
            double summ = 0.0;
            double sumv = 0.0;
            int num = array.Length;

            for (int i = 0; i < num; i++)
            {
                summ = summ + array[i];
                sumv = sumv + array[i] * array[i];
            }
            double mean = summ / num;
            double variance = (sumv / num) - (mean * mean);
            double ret = Math.Sqrt(Math.Abs(variance));
            return ret;
        }
        public static double Volatility(double[] array)
        {
            double ret = (Stddev(array) * Math.Sqrt(250));
            return ret;
        }
        public static double[] Rate(double[] array)
        {
            int num = array.Length;
            double[] ret = new double[num - 1];

            for (int i = 0; i < num - 1; i++)
            {
                ret[i] = (array[i + 1] - array[i]) / array[i];
            }
            return ret;
        }
        public static void VarExcel(double[] array, ref double mean, ref double var)
        {
            int num = array.Length;

            double sq_sum = 0;
            double sum = 0;

            for (int i = 0; i < num; i++)
            {
                sum = sum + array[i];
                sq_sum = sq_sum + array[i] * array[i];
            }
            mean = sum / num;
            var = sq_sum / (num - 1) - mean * mean;
        }
        public static double Average(double[] array)
        {
            int num = array.Length;

            double sum = 0;

            for (int i = 0; i < num; i++)
            {
                sum = sum + array[i];
            }
            sum = sum / num;
            return sum;
        }
    }
}

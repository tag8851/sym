using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sym
{
    /// <summary>
    /// 最新状況
    /// </summary>
    public class Alg2 : Alg
    {
        public class Alg1ParamContext : IParamContext
        {
            public eSide Side;
            public int Len;
            public double Diff;
            public double TP;
            public double ST;
        } 
        private StreamWriter writer = new StreamWriter("out.txt");
        public override void Init()
        {
            this.BeforeExecuting += Alg2_BeforeExecuting;

            Stock.Limit = 300;
            this.IsDetail = true;
            this.ExecuteMode = eExecuteMode.Current;
            Params = null;
        }

        void Alg2_BeforeExecuting(object sender, BeforeExecutingEventArgs args)
        {
            //if (args.Company.Market != "第一部") args.Cancel = true;
        }
        public override void Terminate()
        {
            writer.Close();
        }
        public override void Execute(CompanyInfo.Company company, Stock stock, int index, List<Position> positions, IParamContext paramContext)
        {
            index = stock.HistoricalData.Count - 1;
            Stock.Rec curValue = stock.HistoricalData[index];

            //移動平均乖離
            double value = curValue.Close;
            double bValue = stock.HistoricalData[index - 1].Close;

            double ma125 = GetMA(stock, index, 125);
            double ma15 = GetMA(stock, index, 10);
            double ma5 = GetMA(stock, index, 5);
            double vola = (curValue.Hight - curValue.Low) / curValue.Low;
            double volume = curValue.Volume;

            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", 
                company.Code, 
                company.Name, 
                company.Market,
                company.Industry,
                company.Period,
                value, 
                bValue, 
                ma5, 
                ma15, 
                ma125, 
                vola, 
                volume);

        }

    }
}

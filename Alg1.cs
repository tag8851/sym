using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sym
{
    public class Alg1 : Alg
    {
        public class Alg1ParamContext : IParamContext
        {
            public eSide Side;
            public int Len;
            public double Diff;
            public double TP;
            public double ST;
        }

        public override void Init()
        {
            this.BeforeExecuting += Alg1_BeforeExecuting;

            Stock.Limit = 30000;
            this.IsDetail = true;

            Params = new Alg1ParamContext[] {
                new Alg1ParamContext {Side = eSide.Sell, Len = 125, Diff = 0.5, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Buy, Len = 25, Diff = -0.1, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Buy, Len = 10, Diff = -0.2, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Buy, Len = 25, Diff = -0.2, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Sell, Len = 10, Diff = 0.1, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Sell, Len = 25, Diff = 0.1, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Sell, Len = 10, Diff = 0.20, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Sell, Len = 25, Diff = 0.20, TP = 0.05, ST = 0.05}
                //new Alg1ParamContext {Side = eSide.Sell, Len = 10, Diff = 0.10, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Buy, Len = 10, Diff = 0.10, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Sell, Len = 15, Diff = 0.10, TP = 0.05, ST = 0.05},
                //new Alg1ParamContext {Side = eSide.Buy, Len = 15, Diff = 0.10, TP = 0.05, ST = 0.05}
            };
        }

        void Alg1_BeforeExecuting(object sender, BeforeExecutingEventArgs args)
        {
            //if (args.Company.Code != "6817") args.Cancel = true;
            //if (args.Company.Market != "第一部") args.Cancel = true;
            //if (args.Company.Market != "マザーズ") args.Cancel = true;
        }
        public override void Terminate()
        {
        }
        public override void Execute(CompanyInfo.Company company, Stock stock, int index, List<Position> positions, IParamContext paramContext)
        {
            var param = paramContext as Alg1ParamContext;

            Stock.Rec curValue = stock.HistoricalData[index];

            //if(curValue.Date.ToString("yyyyMMdd") == "20170413")
            //{

            //}
      

            //線形回帰
            //var values = stock.GetValues(index, len);

            //if (values.Length != len) return;

            //values = Function.Normalize(values);

            //var reg = Function.Linest(values);
            //if (reg.a > 0.01 && reg.r > 0.6)
            //{
            //    positions.Add(Position.NewPosition(company, eSide.Buy, curValue.Open, curValue.Date));
            //}
            
            //移動平均乖離
            //double ma = GetMA(stock, index, len);
            //if (Function.GetDiffFromMA(ma, curValue.Open) < diff)
            //{
            //    positions.Add(Position.NewPosition(company, eSide.Buy, curValue.Open, curValue.Date));
            //}

            //移動平均乖離(売り）
            double ma = GetMA(stock, index, param.Len);

            if (param.Side == eSide.Buy)
            {
                if (Function.GetDiffFromMA(ma, curValue.Open) < param.Diff)
                {
                    if (!PositionExsist(company.Code))
                    {
                        positions.Add(Position.NewPosition(company, param.Side, curValue.Open, curValue.Date));
                    }
                }
            }
            else
            {
                if (Function.GetDiffFromMA(ma, curValue.Open) > param.Diff)
                {
                    if (!PositionExsist(company.Code))
                    {
                        positions.Add(Position.NewPosition(company, param.Side, curValue.Open, curValue.Date));
                    }
                }
            }

            //評価
            for(int i = 0; i < positions.Count; i++)
            {
                Position p = positions[i];
                if(p.PositionStatus != ePositionStatus.New) continue;

                //Loscut
                if (this.CheckST(p, curValue.Open, param.ST))
                {
                    if(param.Side == eSide.Buy)
                    {
                        p.ClosePosition(ePositionStatus.LossCut, curValue.Low, curValue.Date);
                    }
                    else
                    {
                        p.ClosePosition(ePositionStatus.LossCut, curValue.Hight, curValue.Date);
                    }

                    //p.ClosePosition(ePositionStatus.LossCut, curValue.Open, curValue.Date);
                }
                //Profit
                if (this.CheckTP(p, curValue.Open, param.TP))
                {
                    p.ClosePosition(ePositionStatus.Profit, curValue.Open, curValue.Date);
                }
                //Timeout
                if(this.CheckTimeOut(p, curValue.Date, 10))
                {
                    p.ClosePosition(ePositionStatus.TimeOver, curValue.Open, curValue.Date);
                }
            }
        }




        private double GetMA(Stock stock, int index, int len)
        {
            var newList = stock.HistoricalData.Skip(index - len).Take(len);
            var newList1 = stock.HistoricalData.Skip(index - len);

            return newList.Average(x => x.Close);
        }
    }
}

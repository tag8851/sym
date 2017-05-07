using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sym
{
    public interface IParamContext
    {
    }

    #region Event
    public class BeforeExecutingEventArgs
    {
        public CompanyInfo.Company Company;
        public bool Cancel = false;
    }
    public delegate void BeforeExecutingHandler(object sender, BeforeExecutingEventArgs args);
    #endregion

    public abstract class Alg
    {
        public enum eExecuteMode
        {
            Historical,
            Current
        }
        public event BeforeExecutingHandler BeforeExecuting;
        private void OnBeforeExecuting(BeforeExecutingEventArgs args)
        {
            if (BeforeExecuting != null)
            {
                BeforeExecuting(this, args);
            }
        }
        public string StockDir = @"C:\Python27\data";
        public bool IsDetail = false;
        public eExecuteMode ExecuteMode = eExecuteMode.Historical;
        protected IList<IParamContext> Params;
        public CompanyInfo CompanyInfo  = new CompanyInfo("CompanyInfo.xml");
        private List<Position> AllPositions = new List<Position>();
        private List<Position> CurPositions = new List<Position>();

        public abstract void Init();
        public abstract void Terminate();
        public abstract void Execute(CompanyInfo.Company company, Stock stock, int index, List<Position> positions, IParamContext paramContext);
        public void Run()
        {
            if (CompanyInfo == null) return;

            using (StreamWriter summary = new StreamWriter("summary.txt"))
            {

                int paramNo = 0;
                if(Params != null)
                {
                    foreach (var param in Params)
                    {
                        RunBody(summary, paramNo, CompanyInfo, param);
                    }
                }
                else
                {
                    RunBody(summary, paramNo, CompanyInfo, null);
                }

            }
        }
        private void  RunBody(StreamWriter summary, int paramNo, CompanyInfo companyInfo, IParamContext param)
        {
            foreach (CompanyInfo.Company c in companyInfo.Companies)
            {
                var args = new BeforeExecutingEventArgs
                {
                    Company = c
                };

                OnBeforeExecuting(args);

                if (args.Cancel) continue;

                var path = StockDir + "/" + c.Code + ".csv";
                if (System.IO.File.Exists(path))
                {
                    Stock stock = new Stock(path);

                    if (ExecuteMode == eExecuteMode.Historical)
                    {
                        for (int i = 25; i < stock.HistoricalData.Count; i++)
                        {
                            Execute(c, stock, i, CurPositions, param);
                        }
                    }
                    else
                    {
                        Execute(c, stock, 0, CurPositions, param);
                    }

                    AllPositions.AddRange(CurPositions);

                    CurPositions.Clear();

                    Console.WriteLine("param:{0} code:{1}", paramNo, c.Code);
                }

                //Output Detail
                if (IsDetail)
                {
                    using (StreamWriter detail = new StreamWriter(paramNo.ToString("000")  + "_detail.txt"))
                    {
                        foreach (Position p in this.AllPositions)
                        {
                            detail.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                                p.Company.Code,
                                p.Company.Name,
                                p.Company.Market,
                                p.Company.Industry,
                                p.PositionStatus, 
                                p.TradeDate.ToString("yyyy-MM-dd"), 
                                p.GetProfitPercentage()
                            );
                        }
                    }
                }
            }
            var result = PositionManager.GetSummary(AllPositions);

            summary.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
                paramNo,
                result.TotalCount,
                result.ProfitCount,
                result.LossCount,
                result.TimeOverCount,
                result.Expected.ToString("0.0000"),
                result.TotalProfit,
                result.TotalLoss
            );

            AllPositions.Clear();
            paramNo++;
        }

        public int GetPositionCount()
        {
            int count = 0;

            foreach (Position p in CurPositions)
            {
                if(p.PositionStatus == ePositionStatus.New)
                {
                    count++;
                }
            }
            return count;
        }
        public double GetMA(Stock stock, int index, int len)
        {
            var newList = stock.HistoricalData.Skip(index + 1 - len).Take(len);

            return newList.Average(x => x.Close);
        }

        public bool PositionExsist(string code)
        {
            return CurPositions.Any(m => m.Company.Code == code && m.PositionStatus == ePositionStatus.New);
        }
        public bool CheckTP(Position position, double value, double tp)
        {
            if(position.Side == eSide.Buy)
            {
                return ((position.TradeValue *(1 + tp)) < value) ? true : false;
            }
            else
            {
                return ((position.TradeValue * (1 - tp)) > value) ? true : false;
            }
        }
        public bool CheckST(Position position, double value, double st)
        {
            if (position.Side == eSide.Buy)
            {
                return ((position.TradeValue * (1 - st)) > value) ? true : false;
            }
            else
            {
                return ((position.TradeValue * (1 + st)) < value) ? true : false;
            }
        }
        public bool CheckTimeOut(Position position, DateTime date, int len)
        {
            var sp = date - position.TradeDate;

            return (sp.Days > len) ? true : false;
        }
        public bool Check(Position position, Stock.Rec value, double tp, double st, int len)
        {
            if (position.Side == eSide.Buy)
            {
                //TPのチェック(Open値でチェック）
                if (((position.TradeValue * (1 + tp)) < value.Open))
                {
                    position.PositionStatus = ePositionStatus.Profit;
                    position.CloseValue = value.Open;
                    position.CloseDate = value.Date;
                    return true;
                }
                //STのチェック(Low値でチェック）
                if (((position.TradeValue * (1 - st)) > value.Low))
                {
                    position.PositionStatus = ePositionStatus.LossCut;
                    position.CloseValue = value.Low;
                    position.CloseDate = value.Date;
                    return true;
                }
            }
            else
            {
                //TPのチェック(Open値でチェック）
                if (((position.TradeValue * (1 - tp)) > value.Open))
                {
                    position.PositionStatus = ePositionStatus.Profit;
                    position.CloseValue = value.Open;
                    position.CloseDate = value.Date;
                    return true;
                }
                //STのチェック(Hight値でチェック）
                if (((position.TradeValue * (1 + st)) < value.Hight))
                {
                    position.PositionStatus = ePositionStatus.LossCut;
                    position.CloseValue = value.Hight;
                    position.CloseDate = value.Date;
                    return true;
                }
            }
            //TimeOut
            //var sp = value.Date - position.TradeDate;
            //if(sp.Days > len)
            //{
            //    position.PositionStatus = ePositionStatus.TimeOver;
            //    position.CloseValue = value.Hight;
            //    position.CloseDate = value.Date;
            //    return true;
            //}

            return false;
        }
    }
}

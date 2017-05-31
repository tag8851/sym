using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sym
{
    /// <summary>
    /// 決算日ロジック
    /// </summary>
    public class Alg3 : Alg
    {
        private StreamWriter writer = new StreamWriter("alg3.txt");
        public override void Init()
        {
            Stock.Limit = 3000;
            Params = null;

            //CompanyInfo.Companies = CompanyInfo.Companies.Where(m => m.Code == "1301").ToList();
            //CompanyInfo.Companies = CompanyInfo.Companies.Where(m => m.Code == "8035" || m.Code == "6506" || m.Code == "6146").ToList();

            foreach (var c in CompanyInfo.Companies)
            {
                Kessan.KessanInfo3 info = Kessan.KessanInfo3.Create(c.Code);
                Stock stock = new Stock(StockDir + "/" + c.Code + ".csv");
                try
                {
                    Kessan.KessanInfo3.Account[] a = info.GetAccounts();
                    foreach (var account in info.GetAccounts())
                    {
                        //if (account.Term.ToString() == "Q4" && account.Nen == 2009)
                        //{
                        //}
                        if (account.FilingDate == null) continue;

                        int index = stock.GetValueIndex(DateTime.Parse(account.FilingDate));
                        //決算日よくじつへ
                        //index = index + 1;

                        //評価は寄値
                        var p1 = Exper(stock, index, 0.05, 0.05, 2);
                        var p2 = Exper(stock, index, 0.05, 0.05, 15);
                        var p3 = Exper(stock, index, 0.05, 0.05, 125);

                        double ma5 = GetMA(stock, index, 5);
                        double ma15 = GetMA(stock, index, 10);
                        double ma125 = GetMA(stock, index, 125);

                        Stock.Rec rec = stock.HistoricalData[index];
                        double curValue = stock.HistoricalData[index].Open;

                        if (p1 != null)
                        {
                            writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}\t{23}\t{24}\t{25}\t{26}",
                            //writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{13}",
                                    c.Code,
                                    //c.Name,
                                    c.Market,
                                    c.Period,
                                    account.Nen,
                                    account.Term.ToString(),
                                    account.FilingDate,

                                    Function.GetDiffFromMA(ma5, curValue),
                                    Function.GetDiffFromMA(ma15, curValue),
                                    Function.GetDiffFromMA(ma125, curValue),
                                    
                                    p1.PositionStatus.ToString(),
                                    p1.TradeDate.ToString("yyyy-MM-dd"),
                                    p1.TradeValue,
                                    p1.CloseDate.ToString("yyyy-MM-dd"),
                                    p1.CloseValue,
                                    p1.GetProfitPercentage(),
                                    
                                    p2.PositionStatus.ToString(),
                                    p2.TradeDate.ToString("yyyy-MM-dd"),
                                    p2.TradeValue,
                                    p2.CloseDate.ToString("yyyy-MM-dd"),
                                    p2.CloseValue,
                                    p2.GetProfitPercentage(),
                                    
                                    p3.PositionStatus.ToString(),
                                    p3.TradeDate.ToString("yyyy-MM-dd"),
                                    p3.TradeValue,
                                    p3.CloseDate.ToString("yyyy-MM-dd"),
                                    p3.CloseValue,
                                    p3.GetProfitPercentage()
                            );
                        }
                    }

                }
                catch (Exception e)
                {
                }

                Console.WriteLine(c.Code);
            }
        }
        private Position Exper(Stock stock, int index, double tp, double st, int limit)
        {
            if((stock.HistoricalData.Count - limit) < index || index == -1) return null;

            Position p = new Position();
            p.TradeDate = stock.HistoricalData[index].Date;
            p.TradeValue = stock.HistoricalData[index].Open;
            p.TradeIndex = index;

            for(int i = 0; i < limit; i++)
            {
                Stock.Rec curValue = stock.HistoricalData[index + i];

                if (Check(p, curValue, tp, st, limit))
                {
                    return p;
                }
            }
            p.PositionStatus = ePositionStatus.TimeOver;
            p.CloseValue = stock.HistoricalData[index + limit].Close;
            p.CloseDate = stock.HistoricalData[index + limit].Date;
            return p;
        }
        void Alg3_BeforeExecuting(object sender, BeforeExecutingEventArgs args)
        {
        }
        public override void Terminate()
        {
            writer.Close();
        }
        public override void Execute(CompanyInfo.Company company, Stock stock, int index, List<Position> positions, IParamContext paramContext)
        {
        }
    }
}

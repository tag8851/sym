using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sym
{
    public class PositionManager
    {
        public class Result
        {
            public double TotalProfit;
            public double TotalLoss;
            public int ProfitCount;
            public int LossCount;
            public int TotalCount;
            public int TimeOverCount;
            public double Expected;
            public double Winner;
        }
        public PositionManager()
        {
        }
        public static Result GetSummary(List<Position> positions)
        {
            var result = new Result();

            foreach(var pos in positions)
            {
                switch(pos.PositionStatus)
                {
                    case ePositionStatus.Profit:
                        result.ProfitCount++;
                        break;
                    case ePositionStatus.LossCut:
                        result.LossCount++;
                        break;
                    case ePositionStatus.TimeOver:
                        result.TimeOverCount++;
                        break;
                }
                if(pos.PositionStatus != ePositionStatus.New)
                {
                    result.TotalCount++;

                    var v = pos.GetProfitPercentage();
                    if(v > 0)
                    {
                        result.TotalProfit = result.TotalProfit + v;
                    }
                    else
                    {
                        result.TotalLoss = result.TotalLoss + v;
                    }
                }
            }
            if (result.LossCount > 0)
            {
                result.Winner = result.ProfitCount / result.LossCount;
                //期待値
                result.Expected = (result.TotalProfit + result.TotalLoss) / result.TotalCount;
            }

            return result;
        }
    }
}

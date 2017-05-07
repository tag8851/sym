using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sym
{
    public class Position
    {
        public CompanyInfo.Company Company;
        public string Name;
        public double MarketCapitalization;
        public ePositionStatus PositionStatus;
        public eSide Side;
        public int TradeIndex;
        public double TradeMA;
        public DateTime TradeDate;
        public double TradeValue;
        public int CloseIndex;
        public double CloseValue;
        public DateTime CloseDate;
        public double GetProfit()
        {
            if(Side == eSide.Buy)
            {
                return CloseValue - TradeValue; 
            }
            else
            {
                return TradeValue - CloseValue; 
            }
        }
        public double GetProfitPercentage()
        {
            return GetProfitPercentage(Side, TradeValue, CloseValue);
        }
        public static double GetProfitPercentage(eSide side, double tradeValue, double closeValue)
        {
            if (side == eSide.Buy)
            {
                return (closeValue - tradeValue) / closeValue;
            }
            else
            {
                return (tradeValue - closeValue) / closeValue;
            }
        }
        public static Position NewPosition(CompanyInfo.Company company, eSide side, double tradeValue, DateTime tradeDate)
        {
            var ret = new Position
                {
                    Company = company,
                    PositionStatus = ePositionStatus.New, 
                    Side = side, 
                    TradeValue = tradeValue, 
                    TradeDate = tradeDate
                };

            return ret;
        }
        public void ClosePosition(ePositionStatus status, double closeValue, DateTime closeDate)
        {
            this.PositionStatus = status;
            this.CloseValue = closeValue;
            this.CloseDate = closeDate;
        }
    }
    public enum ePositionStatus
    {
        New,
        Profit,
        LossCut,
        TimeOver
    }
    public enum eSide
    {
        Buy, Sell
    }
}

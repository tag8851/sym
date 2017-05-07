using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kessan
{
    public class KessanInfo3
    {
        public enum Term
        {
            Q1 = 0,
            Q2 = 1,
            Q3 = 2,
            Q4 = 3
        }
        public Account CurrentAccount;
        private SortedList<int, Nendo> nendos = new SortedList<int, Nendo>();
        private static Term GetTerm(string termStr)
        {
            switch (termStr)
            {
                case "1Q":
                    return Term.Q1;
                case "2Q":
                    return Term.Q2;
                case "3Q":
                    return Term.Q3;
                case "4Q":
                    return Term.Q4;
            }
            return Term.Q1;
        }
        public static KessanInfo3 Create(string code)
        {
            try
            {
                KessanInfo3 kessanInfo = new KessanInfo3();

                XmlDocument doc = new XmlDocument();

                doc.Load(@"C:\dev\株関連\fcalc\Kessan\Kessan\bin\Debug\Output/Companies/" + code + "/HistoricalData.xml");

                foreach (XmlElement record in doc.SelectNodes("//Record"))
                {
                    int nendo = int.Parse(XmlUtil.GetValue(record, "年度"));
                    Term term = GetTerm(XmlUtil.GetValue(record, "期"));

                    Account account = new Account();

                    kessanInfo.CurrentAccount = account;

                    kessanInfo.SetAccount(nendo, term, account);

                    account.Nen = nendo;
                    account.Term = term;
                    account.FilingDate = XmlUtil.GetValue(record, "提出日");
                    account.NetSales = XmlUtil.GetDblValue(record, "売上高");
                    account.OperatingIncome = XmlUtil.GetDblValue(record, "営業利益");
                    account.OrdinaryIncome = XmlUtil.GetDblValue(record, "経常利益");
                    account.NetIncome = XmlUtil.GetDblValue(record, "純利益");

                    account.NetSales_2Q = XmlUtil.GetDblValue(record, "予想売上高2Q");
                    account.OperatingIncome_2Q = XmlUtil.GetDblValue(record, "予想営業利益2Q");
                    account.OrdinaryIncome_2Q = XmlUtil.GetDblValue(record, "予想経常利益2Q");
                    account.NetIncome_2Q = XmlUtil.GetDblValue(record, "予想純利益2Q");

                    account.NetSales_4Q = XmlUtil.GetDblValue(record, "予想売上高");
                    account.OperatingIncome_4Q = XmlUtil.GetDblValue(record, "予想営業利益");
                    account.OrdinaryIncome_4Q = XmlUtil.GetDblValue(record, "予想経常利益");
                    account.NetIncome_4Q = XmlUtil.GetDblValue(record, "予想純利益");
                    
                    account.TotalAsset = XmlUtil.GetDblValue(record, "総資産");
                    account.NetAsset = XmlUtil.GetDblValue(record, "純利益資産");
                    account.Shares = XmlUtil.GetDblValue(record, "株数");
 
                    //1Qの場合は2Qの補完を行う
                    if (term == Term.Q1)
                    {
                        Account account2Q = new Account();
                        account2Q.NetSales = XmlUtil.GetDblValue(record, "予想売上高2Q");
                        account2Q.OperatingIncome = XmlUtil.GetDblValue(record, "予想営業利益2Q");
                        account2Q.OrdinaryIncome = XmlUtil.GetDblValue(record, "予想経常利益2Q");
                        account2Q.NetIncome = XmlUtil.GetDblValue(record, "予想純利益2Q");
                        kessanInfo.SetAccount(nendo, Term.Q2, account2Q);
                   }
                    if (term != Term.Q4)
                    {
                        //4Qの補完を行う
                        Account account4Q = new Account();
                        account4Q.NetSales = XmlUtil.GetDblValue(record, "予想売上高");
                        account4Q.OperatingIncome = XmlUtil.GetDblValue(record, "予想営業利益");
                        account4Q.OrdinaryIncome = XmlUtil.GetDblValue(record, "予想経常利益");
                        account4Q.NetIncome = XmlUtil.GetDblValue(record, "予想純利益");
                        kessanInfo.SetAccount(nendo, Term.Q4, account4Q);
                    }

                    //4Qの予想は次期
                    if (term == Term.Q4)
                    {
                        Account account2Q = new Account();
                        account2Q.NetSales = XmlUtil.GetDblValue(record, "予想売上高2Q");
                        account2Q.OperatingIncome = XmlUtil.GetDblValue(record, "予想営業利益2Q");
                        account2Q.OrdinaryIncome = XmlUtil.GetDblValue(record, "予想経常利益2Q");
                        account2Q.NetIncome = XmlUtil.GetDblValue(record, "予想純利益2Q");
                        kessanInfo.SetAccount(nendo + 1, Term.Q2, account2Q);

                        Account account4Q = new Account();
                        account4Q.NetSales = XmlUtil.GetDblValue(record, "予想売上高");
                        account4Q.OperatingIncome = XmlUtil.GetDblValue(record, "予想営業利益");
                        account4Q.OrdinaryIncome = XmlUtil.GetDblValue(record, "予想経常利益");
                        account4Q.NetIncome = XmlUtil.GetDblValue(record, "予想純利益");
                        kessanInfo.SetAccount(nendo + 1, Term.Q4, account4Q);


                        //CashFlow
                        account.CF_Operating = XmlUtil.GetDblValue(record, "営業キャッシュフロー");
                        account.CF_Investing = XmlUtil.GetDblValue(record, "投資キャッシュフロー");
                        account.CF_Financing = XmlUtil.GetDblValue(record, "財務キャッシュフロー");
                        account.CF_Cash = XmlUtil.GetDblValue(record, "現金");

                    }
                }

                return kessanInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SetAccount(int nen, Term term, Account account)
        {
            if (!nendos.ContainsKey(nen))
            {
                Nendo nendo = new Nendo();
                nendo.KessanInfo = this;
                nendos.Add(nen, nendo);
            }
            nendos[nen].SetAccount(term, account);
        }
        //
        public int[] GetNendos()
        {
            int[] ret = new int[nendos.Count];
            for (int i = 0; i < nendos.Count; i++)
            {
                ret[i] = nendos.Keys[i];
            }

            
            /*int[] ret = new int[nendos.Count * 4];
            for (int i = 0; i < nendos.Count; i++)
            {
                ret[i * 4] = nendos.Keys[i] * 10 + 1;
                ret[i * 4 + 1] = nendos.Keys[i] * 10 + 2;
                ret[i * 4 + 2] = nendos.Keys[i] * 10 + 3;
                ret[i * 4 + 3] = nendos.Keys[i] * 10 + 4;
            }*/

            return ret;
        }

        //全年度、期を返す
        public Account[] GetAccounts()
        {
            Account[] ret = new Account[nendos.Count * 4];
            for (int i = 0; i < nendos.Count; i++)
            {
                ret[i * 4] = nendos.Values[i].GetAccount(Term.Q1);
                ret[i * 4 + 1] = nendos.Values[i].GetAccount(Term.Q2);
                ret[i * 4 + 2] = nendos.Values[i].GetAccount(Term.Q3);
                ret[i * 4 + 3] = nendos.Values[i].GetAccount(Term.Q4);
            }

            return ret;
        }

        public Account[] GetAccounts(Term term)
        {
            Account[] ret = new Account[nendos.Count];
            int cnt = 0;
            foreach (Account account in GetAccounts())
            {
                if (account.Term == term)
                {
                    ret[cnt] = account;
                    cnt++;
                }
            }

            //Account[] ret = new Account[nendos.Count];
            //for (int i = 0; i < nendos.Count; i++)
            //{
            //    Nendo n = nendos[i];
            //    ret[i] = nendos[i].GetValue(term);
            //}

            return ret;
        }


        public Account GetAccount(int nendo, Term term)
        {
            if (nendos.ContainsKey(nendo))
            {
                return nendos[nendo].GetAccount(term);
            }
            else
            {
                return null;
            }
        }
        public Account GetAccount(int nendo, Term term, int diff)
        {
            if (diff < 0)
            {
                if (term == Term.Q1)
                {
                    nendo = nendo - 1;
                    term = Term.Q4;
                }
                else
                {
                    term = term - 1;
                }
            }
            else if(diff > 0)
            {
                if (term == Term.Q4)
                {
                    nendo = nendo + 1;
                    term = Term.Q1;
                }
                else
                {
                    term = term + 1;
                }
            }

            if (nendos.ContainsKey(nendo))
            {
                return nendos[nendo].GetAccount(term);
            }
            else
            {
                return null;
            }
        }
        public Account GetRecentAccount()
        {
            Account[] accounts = this.GetAccounts();

            if (accounts.Length == 0) return null;

            for (int i = 0; i < accounts.Length; i++)
            {
                if(accounts[accounts.Length - i - 1].FilingDate != null)
                {
                    return accounts[accounts.Length - i -1];
                }
            }

            return null;
        }
        //
        public class Nendo
        {
            public KessanInfo3  KessanInfo;
            public Account[] Terms = new Account[4];
            public Nendo()
            {
                for (int i = 0; i < 4; i++)
                {
                    this.Terms[i] = new Account();
                    this.Terms[i].Nendo = this;
                }
            }
            public void SetAccount(Term term, Account account)
            {
                account.Nendo = this;
                Terms[(int)term] = account;
            }
            public Account GetAccount(Term term)
            {
                return Terms[(int)term];
            }
        }
        //
        public class Account
        {
            public Nendo Nendo;

            public int Nen;
            public Term Term;
            public string FilingDate;
            public double NetSales;
            public double OperatingIncome;
            public double OrdinaryIncome;
            public double NetIncome;

            public double NetSales_2Q;
            public double OperatingIncome_2Q;
            public double OrdinaryIncome_2Q;
            public double NetIncome_2Q;

            public double NetSales_4Q;
            public double OperatingIncome_4Q;
            public double OrdinaryIncome_4Q;
            public double NetIncome_4Q;

            public double TotalAsset;
            public double NetAsset;
            public double Shares;
            public double CF_Operating;
            public double CF_Investing;
            public double CF_Financing;
            public double CF_Cash;
            public Account()
            {

                NetSales = double.NaN;
                OperatingIncome = double.NaN;
                OrdinaryIncome = double.NaN;
                NetIncome = double.NaN;


                NetSales_2Q = double.NaN;
                OperatingIncome_2Q = double.NaN;
                OrdinaryIncome_2Q = double.NaN;
                NetIncome_2Q = double.NaN;

                NetSales_4Q = double.NaN;
                OperatingIncome_4Q = double.NaN;
                OrdinaryIncome_4Q = double.NaN;
                NetIncome_4Q = double.NaN;

                TotalAsset = double.NaN;
                NetAsset = double.NaN;
                Shares = double.NaN;
                CF_Operating = double.NaN;
                CF_Investing = double.NaN;
                CF_Financing = double.NaN;
                CF_Cash = double.NaN;

            }
            public Account GetAccount(int nen, Term term, int diff)
            {
                return this.Nendo.KessanInfo.GetAccount(nen, term, diff);
            }

            public Account GetAccount(int diff)
            {
                return this.Nendo.KessanInfo.GetAccount(this.Nen, this.Term, diff);
            }
            public double EPS
            {
                get
                {
                    if (this.Shares < 1 || this.NetIncome < 0)
                    {
                        return double.NaN;
                    }

                    return this.NetIncome / this.Shares * 1000000;
                    //double v = this.Nendo.GetValue(Term.Q4).NetIncome;

                    //if (v < 0) return double.NaN;

                    //return v / Shares * 1000000;
                }
            }
            public double SPS
            {
                get
                {
                    if (this.Shares < 1 || this.NetSales < 0)
                    {
                        return double.NaN;
                    }

                    return this.NetSales  / this.Shares * 1000000;
                }
            }
            public double CFPS
            {
                get
                {
                    if (this.Shares < 1 || this.CF_Operating < 0)
                    {
                        return double.NaN;
                    }


                    return this.CF_Operating / this.Shares * 1000000;
                }
            }
            public double DPS
            {
                get
                {
                    if (this.Shares < 1 || this.CF_Operating < 0)
                    {
                        return double.NaN;
                    }

                    return this.CF_Operating / this.Shares * 1000000;
                }
            }          
            public double GetEPS(double value) 
            {
                if (value < 0) return double.NaN;

                return value / Shares * 1000000;
            }
            public double BPS
            {
                get
                {
                    if (TotalAsset < 0) return double.NaN;
                    return NetAsset / Shares * 1000000;
                }
            }

        }
    }
}


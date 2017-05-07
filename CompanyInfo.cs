using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections;
namespace sym
{
    public class CompanyInfo
    {
        public List<Company> Companies = new List<Company>();
        private Dictionary<string, Company> dic = new Dictionary<string, Company>();
        public CompanyInfo()
        {
        }
        public CompanyInfo(string path)
        {
            if (Path.GetExtension(path) == ".txt")
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string buf = "";
                    while ((buf = reader.ReadLine()) != null)
                    {
                        string[] ar = buf.Split('\t');
                        Company company = new Company();
                        company.Code = ar[0];
                        company.IsIndex = false;
                        company.Name = ar[1];
                        company.Industry = ar[2];
                        company.Period = int.Parse(ar[3].Substring(0, 1));
                        company.Unit = int.Parse(ar[4]);
                        company.EntryDate = ar[5];
                        this.Companies.Add(company);
                        this.dic.Add(company.Code, company);
                    }
                }
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                foreach (XmlElement e in doc.SelectNodes("Companies/*"))
                {
                    Company company = new Company();
                    company.Code = e.GetAttribute("Code");
                    company.Name = e.GetAttribute("Name");
                    company.Name = company.Name.Replace(" ", "");
                    company.Name = company.Name.Replace("@", "");
                    company.Name = company.Name.Replace("\t", "");


                    company.Place = e.GetAttribute("Place");
                    company.Industry = e.GetAttribute("Industry");
                    company.Market = e.GetAttribute("Market");
                    company.Period = int.Parse(e.GetAttribute("Period"));
                    company.IsN225 = bool.Parse(e.GetAttribute("N225"));

                    company.Unit = int.Parse(e.GetAttribute("Unit"));

                    /*if (e.GetAttribute("KessanDate") != "-")
                    {
                        company.KessanDate = e.GetAttribute("KessanDate").Substring(0, 10);
                        company.KessanTime = e.GetAttribute("KessanDate").Substring(10);
                    }
                    else
                    {
                        company.KessanDate = e.GetAttribute("KessanDate");
                        company.KessanTime = "-";
                    }*/
                    company.IsIndex = false;

                    double.TryParse(e.GetAttribute("Shares"), out company.Shares);

                    company.EntryDate = e.GetAttribute("EntryDate");
                    company.KessanDate1Q = e.GetAttribute("KessanDate2Q");
                    company.KessanDate2Q = e.GetAttribute("KessanDate3Q");
                    company.KessanDate3Q = e.GetAttribute("KessanDate4Q");
                    company.KessanDate4Q = e.GetAttribute("KessanDate1Q");
                    company.KessanDate = GetRecentKessanDate(company);


                    this.Companies.Add(company);
                    this.dic.Add(company.Code, company);
                }
            }
        }
        private string GetRecentKessanDate(Company company)
        {
            string ret = company.KessanDate1Q;

            if (company.KessanDate2Q.CompareTo(ret) > 0) ret = company.KessanDate2Q;
            if (company.KessanDate3Q.CompareTo(ret) > 0) ret = company.KessanDate3Q;
            if (company.KessanDate4Q.CompareTo(ret) > 0) ret = company.KessanDate4Q;



            return ret;
        }
        public Company Find(string code)
        {
            if (dic.ContainsKey(code))
            {
                return dic[code];
            }
            return null;
        }
        public List<Company> Finds(string[] codes)
        {
            List<Company> ret = new List<Company>();

            foreach (string code in codes)
            {
                if (dic.ContainsKey(code))
                {
                    ret.Add(dic[code]);
                }
 
            }
            return ret;
        }

        public class Company
        {
            public string Industry;
            public string Market;
            public string Place;
            public string Code;
            public string Name;
            public int Period;
            public bool IsIndex;
            public bool IsN225;
            public string KessanDate;
            public string KessanDate1Q;
            public string KessanDate2Q;
            public string KessanDate3Q;
            public string KessanDate4Q;
            public string KessanTime;
            public int Unit;
            public string EntryDate;
            public double Shares;
        }
 
    }

}

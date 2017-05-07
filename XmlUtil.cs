using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kessan
{
    class XmlUtil
    {
        public static string GetValue(XmlElement element, string id)
        {
            string  s = "Item[@Id='" + id + "']";

            XmlElement e = (XmlElement)element.SelectSingleNode(s);

            return e.InnerText;
        }
        public static double GetDblValue(XmlElement element, string id)
        {
            string s = "Item[@Id='" + id + "']";

            XmlElement e = (XmlElement)element.SelectSingleNode(s);

            if (e == null)
            {
                return 0;
            }


            double v = 0;

            double.TryParse(e.InnerText, out v);



            return v;
        }
    }
}

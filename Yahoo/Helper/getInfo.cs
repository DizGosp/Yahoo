using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Yahoo.Helper
{
    //Klasa u koju se spremaju podaci o detaljnim informacijama prilikom convertovanja iz JSona
    public class getInfo
    {
        [DataContract]
        public class MarketCap
        {
            [DataMember]
            public string longFmt { get; set; }
        }

        [DataContract]
        public class Price
        {
            [DataMember]
            public MarketCap marketCap { get; set; }
        }
        public class SummaryProfile
        {
            [DataMember]
            public int fullTimeEmployees { get; set; }
            [DataMember]
            public string city { get; set; }
            [DataMember]
            public string country { get; set; }
        }
        public class Root2
        {
            [DataMember]
            public SummaryProfile summaryProfile { get; set; }
            [DataMember]
            public Price price { get; set; }
        }
    }
}

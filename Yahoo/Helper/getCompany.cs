using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Yahoo.Helper
{
    //Klasa u koju se spremaju podaci o listi tickersa prilikom convertovanja iz JSona
    public class getCompany
    {
        [DataContract]
        public class Result
        {
            [DataMember]
            public string symbol { get; set; }
            [DataMember]
            public string shortName { get; set; }
        }

        [DataContract]
        public class MarketSummaryAndSparkResponse
        {
            [DataMember]
            public List<Result> result { get; set; }
        }

        [DataContract]
        public class Root
        {
            [DataMember]
            public MarketSummaryAndSparkResponse marketSummaryAndSparkResponse { get; set; }
        }
    }
}

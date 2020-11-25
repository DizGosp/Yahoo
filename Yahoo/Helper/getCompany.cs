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
        public class Quote
        {
            [DataMember]
            public string shortName { get; set; }
            [DataMember]
            public string symbol { get; set; }
        }

            [DataContract]
        public class Result
        {
            [DataMember]
            public List<Quote> quotes { get; set; }
         
        }

        [DataContract]
        public class Finance
        {
            [DataMember]
            public List<Result> result { get; set; }
            [DataMember]
            public object error { get; set; }
        }

        [DataContract]
        public class Root
        {
            [DataMember]
            public Finance finance { get; set; }
        }

    }
}

﻿using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class BasicData
    {
        [DataMember]
        public decimal? WeekRange52 { get; set; }

        [DataMember]
        public decimal? AverageVolume { get; set; }

        [DataMember]
        public decimal? MarketCapitalization{ get; set; }

        [DataMember]
        public decimal? EnterpriseValue { get; set; }

        [DataMember]
        public decimal? SharesOutstanding { get; set; }

        [DataMember]
        public decimal? Beta { get; set; }

        [DataMember]
        public decimal?  BarraBeta{ get; set; }



    }
}
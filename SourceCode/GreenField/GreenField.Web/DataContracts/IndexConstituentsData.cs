﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class IndexConstituentsData
    {
        [DataMember]
        public string ConstituentName { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Sector { get; set; }

        [DataMember]
        public string Industry { get; set; }

        [DataMember]
        public string SubIndustry { get; set; }

        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public double WeightCountry { get; set; }

        [DataMember]
        public double WeightIndustry { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public long Shares { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public double PriceCurrency { get; set; }

        [DataMember]
        public double FXPriceCurrency { get; set; }

        [DataMember]
        public double ForeignInclusionFactor { get; set; }

        [DataMember]
        public double DailyReturnUSD { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// DataContract class for ChartExtension Gadget, Holdings 4.4.1
    /// </summary>
    [DataContract]
    public class ChartExtensionData
    {
        [DataMember]
        public DateTime? ToDate { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Sector { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public decimal AdjustedDollarPrice { get; set; }

        [DataMember]
        public decimal PriceReturn { get; set; }

        [DataMember]
        public decimal? AmountTraded { get; set; }

        [DataMember]
        public decimal? SectorReturn { get; set; }

        [DataMember]
        public decimal? CountryReturn { get; set; }
    }
}
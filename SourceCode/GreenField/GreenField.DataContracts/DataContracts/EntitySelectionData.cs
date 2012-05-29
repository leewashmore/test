﻿using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class EntitySelectionData
    {
        [DataMember]
        public int SortOrder { get; set; }

        [DataMember]
        public String Type { get; set; }

        [DataMember]
        public String SecurityType { get; set; }

        [DataMember]
        public String ShortName { get; set; }

        [DataMember]
        public String LongName { get; set; }

        [DataMember]
        public String InstrumentID { get; set; }        
    }


    public static class EntityTypeSortOrder
    {
        public static int Currency = 1;
        public static int Commodity = 2;
        public static int Index = 3;
        public static int Security = 4;
        public static int Benchmark = 5;

        public static int GetSortOrder(String type)
        {
            switch (type.ToUpper())
            {
                case "CURRENCY":
                    return Currency;
                case "COMMODITY":
                    return Commodity;
                case "INDEX":
                    return Index;
                case "SECURITY":
                    return Security;
                case "BENCHMARK":
                    return Benchmark;
                default:
                    return 0;
            }
        }
    }
}
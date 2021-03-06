﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CarPostsServer.ClientModels
{   
    public class ClientCarModelAutoTestV1
    {
        public int DVIG { get; set; }
        public decimal MIN_TAH { get; set; }
        public decimal DEL_MIN { get; set; }
        public decimal MAX_TAH { get; set; }
        public decimal DEL_MAX { get; set; }
        public decimal MIN_CO { get; set; }
        public decimal MAX_CO { get; set; }
        public decimal MIN_CH { get; set; }
        public decimal MAX_CH { get; set; }
        public decimal L_MIN { get; set; }
        public decimal L_MAX { get; set; }
        public decimal K_SVOB { get; set; }
        public decimal K_MAX { get; set; }
        public string MODEL { get; set; }
        public int Version { get; set; }
    }

    public class ClientCarModelAutoTestV2
    {
        public int Index { get; set; }
        public decimal SaveSystem { get; set; }
        public string Category { get; set; }
        public decimal MinTax { get; set; }
        public decimal MinTaxD { get; set; }
        public decimal MinCO { get; set; }
        public decimal MinCH { get; set; }
        public decimal MinCO2 { get; set; }
        public decimal MinO2 { get; set; }
        public decimal MinNOx { get; set; }
        public decimal MinLambda { get; set; }
        public decimal MaxTax { get; set; }
        public decimal MaxTaxD { get; set; }
        public decimal MaxCO { get; set; }
        public decimal MaxCH { get; set; }
        public decimal MaxCO2 { get; set; }
        public decimal MaxO2 { get; set; }
        public decimal MaxNOx { get; set; }
        public decimal MaxLambda { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
    }
}

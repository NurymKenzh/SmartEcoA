﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class CarModelSmokeMeter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TypeEcoClassId { get; set; }
        public string Category { get; set; }
        public decimal? EngineType { get; set; }
        public decimal? MIN_TAH { get; set; }
        public decimal? DEL_MIN { get; set; }
        public decimal? MAX_TAH { get; set; }
        public decimal? DEL_MAX { get; set; }
        public decimal? MIN_CO { get; set; }
        public decimal? MAX_CO { get; set; }
        public decimal? MIN_CH { get; set; }
        public decimal? MAX_CH { get; set; }
        public decimal? L_MIN { get; set; }
        public decimal? L_MAX { get; set; }
        public decimal? K_SVOB { get; set; }
        public decimal? K_MAX { get; set; }

        public int CarPostId { get; set; }
        public int? ParadoxId { get; set; }
    }
}

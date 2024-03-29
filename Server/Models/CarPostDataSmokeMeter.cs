﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
    public class CarPostDataSmokeMeter
    {
        public long Id { get; set; }
        public DateTime? DateTime { get; set; }
        public string Number { get; set; }
        public string DOPOL1 { get; set; }
        public string DOPOL2 { get; set; }
        public decimal? MIN_TAH { get; set; }
        public decimal? MIN_CO { get; set; }
        public decimal? MIN_CH { get; set; }
        public decimal? MIN_CO2 { get; set; }
        public decimal? MIN_O2 { get; set; }
        public decimal? MIN_L { get; set; }
        public decimal? MAX_TAH { get; set; }
        public decimal? MAX_CO { get; set; }
        public decimal? MAX_CH { get; set; }
        public decimal? MAX_CO2 { get; set; }
        public decimal? MAX_O2 { get; set; }
        public decimal? MAX_L { get; set; }
        public decimal? ZAV_NOMER { get; set; }
        public decimal? K_1 { get; set; }
        public decimal? K_2 { get; set; }
        public decimal? K_3 { get; set; }
        public decimal? K_4 { get; set; }
        public decimal? K_SVOB { get; set; }
        public decimal? K_MAX { get; set; }
        public decimal? MIN_NO { get; set; }
        public decimal? MAX_NO { get; set; }

        public int? CarModelSmokeMeterId { get; set; }


        //Дополнительные поля из таблицы dop_info
        public decimal? Temperature { get; set; }
        public decimal? Pressure { get; set; }
        public decimal? GasSerialNumber { get; set; }
        public DateTime? GasCheckDate { get; set; }
        public decimal? MeteoSerialNumber { get; set; }
        public DateTime? MeteoCheckDate { get; set; }
        public decimal? TestNumber { get; set; }
        public int? TesterId { get; set; }
    }
}

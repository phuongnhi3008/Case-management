using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NienLuan2.Models
{
    public class MenuModel
    {
        public string Ten_Main { get; set; }

        public int ID_Main { get; set; }
     
        public string Ten_SUB { get; set; }

        public int ID_SUB { get; set; }

        public string Controller_SUB { get; set; }

        public string Action_SUB { get; set; }

        public string MA_QNSD { get; set; }

        public string Ten_QNSD { get; set; }
    }
}
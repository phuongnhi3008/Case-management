using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NienLuan2.Models
{
    public class DuongSuModel
    {
        public List<CHITIET_DS> listChiTietDuongSu { get; set; }

        public IEnumerable<HOSO_VUAN> listHoSoVuAn{ get; set; }
        public IEnumerable<XETXU> listXetXu { get; set; }
    }
}
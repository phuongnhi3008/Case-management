using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NienLuan2.Models
{
    public class LichXetXuModel
    {
        public IEnumerable<XETXU> listXetXu { get; set; }

        public List<CHITIET_XX> listChiTietXetXu { get; set; }
    }
}
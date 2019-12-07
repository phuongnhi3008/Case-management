using NienLuan2.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NienLuan2.Controllers
{
    public class NguoiDungController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: NguoiDung
        public ActionResult MauDon(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            IEnumerable<MAUDON> model = db.MAUDONs;
            return View(model.OrderByDescending(x => x.TenMauDon).ToPagedList(page, pageSize));
        }

        public ActionResult LichXetXu(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            IEnumerable<XETXU> model = db.XETXUs;
            return View(model.OrderByDescending(x => x.Ngay_XetXu).ToPagedList(page, pageSize));
        }

        public ActionResult KetQua(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            IEnumerable<KETQUA_XX> model = db.KETQUA_XX;
            return View(model.OrderByDescending(x => x.XETXU.Ngay_XetXu).ToPagedList(page, pageSize));
        }
    }
}
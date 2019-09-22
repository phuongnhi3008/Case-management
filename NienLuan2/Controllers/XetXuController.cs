using NienLuan2.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NienLuan2.Controllers
{
    public class XetXuController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: XetXu
        public ActionResult ListXX(string searcxxtring, int? error, int page = 1, int pageSize = 10)
        {
            var listXetXu = from s in db.XETXUs select s;

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<XETXU> model = db.XETXUs;

            if (!string.IsNullOrEmpty(searcxxtring))
            {
                model = model.Where(x => x.DUONGSU.HoTen_DS.Contains(searcxxtring) || x.MA_HoSo.Contains(searcxxtring)).OrderByDescending(x => x.DUONGSU.HoTen_DS);
            }

            ViewBag.Searcxxtring = searcxxtring;
            return View(model.OrderByDescending(x => x.DUONGSU.HoTen_DS).ToPagedList(page, pageSize));   
        }
        public ActionResult them_xx()
        {
            ViewBag.nv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.dd = new SelectList(db.DIADIEM_XX.OrderBy(x => x.Ten_DiaDiem), "MA_DiaDiem", "Ten_DiaDiem");
            return View();
        }

        [HttpPost, ActionName("themLXX")]
        public ActionResult themLXX(XETXU xx, FormCollection form)
        {
            ViewBag.nv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.dd = new SelectList(db.DIADIEM_XX.OrderBy(x => x.Ten_DiaDiem), "MA_DiaDiem", "Ten_DiaDiem");
            ViewBag.hs = new SelectList(db.HOSO_VUAN.OrderBy(x => x.MA_HoSo), "MA_HoSo", "MA_HoSo");
            

            if (db.XETXUs.Any(x => x.STT_XX == xx.STT_XX))
            {

                return RedirectToAction("ListXX", new { error = 1 });
            }

            xx.MA_DiaDiem = form["dd"].ToString();
            xx.HOSO_VUAN.MA_NhanVien = form["nv"].ToString();

            if (!ModelState.IsValid)
                return View(xx);

            db.XETXUs.Add(xx);
            db.SaveChanges();
            return RedirectToAction("ListXX");
        }
    }
}
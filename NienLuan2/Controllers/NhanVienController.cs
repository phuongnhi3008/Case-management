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
    public class NhanVienController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: NhanVien
        public ActionResult ListNV(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            ViewBag.cv = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            ViewBag.qsd = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.pb = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<NHANVIEN> model = db.NHANVIENs;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.HoTen_NV.Contains(searchString) || x.MA_NhanVien.Contains(searchString)).OrderByDescending(x => x.HoTen_NV);
            }

            ViewBag.SearchString = searchString;
            return View(model.OrderByDescending(x => x.HoTen_NV).ToPagedList(page, pageSize));
           // return View(db.NHANVIENs);
        }

        [HttpGet]
        public ActionResult chitiet_NV(string id)
        {
            if (id == null)
                return HttpNotFound();

            var chitietNV = db.NHANVIENs.Find(id);

            return View(chitietNV);
        }

        public ActionResult them_NV()
        {
            ViewBag.cv = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            ViewBag.qsd = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.pb = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");

            return View();
        }

        [HttpPost, ActionName("them_NV")]
        public ActionResult them_NV(NHANVIEN Nv, FormCollection form)
        {
            if (db.NHANVIENs.Any(x => x.MA_NhanVien == Nv.MA_NhanVien))
            {
                ViewBag.error = "Mã nhân viên này đã tồn tại!!!";
                return View(Nv);
            }

            Nv.MA_ChucVu = form["cv"].ToString(); ;
            Nv.MA_PhongBan = form["pb"].ToString();
            Nv.MA_QNSD = form["qsd"].ToString();

            if (!ModelState.IsValid)
                return View(Nv);

            db.NHANVIENs.Add(Nv);
            db.SaveChanges();

            //ViewBag.cv = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            //ViewBag.qsd = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            //ViewBag.pb = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");

            //var list_NV = from s in db.NHANVIENs select s;
            return RedirectToAction("ListNV");
        }

        public JsonResult Edit_NV(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var nv = db.NHANVIENs.Find(id);

            return Json(nv, JsonRequestBehavior.AllowGet);
        }
        public ActionResult sua_NV(string id)
        {
            ViewBag.cv = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            ViewBag.qsd = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.pb = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nhanvien = db.NHANVIENs.Find(id);
            return View(nhanvien);
        }

        [HttpPost, ActionName("sua_NV")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult sua_NV1(NHANVIEN nv, FormCollection form, string id)
        {
            ViewBag.cv = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            ViewBag.qsd = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.pb = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");

            nv.MA_ChucVu = form["cv"].ToString(); ;
            nv.MA_QNSD = form["qsd"].ToString();
            nv.MA_PhongBan = form["pb"].ToString();

            //hs.MA_HoSo = id;

            if (!ModelState.IsValid)
                return View(nv);


            if (TryUpdateModel(nv, "", new string[] { "MA_NhanVien", "HoTen_NV" }))
            {
                try
                {
                    db.Entry(nv).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Lỗi");
                }

            }
            return RedirectToAction("ListNV");
        }
        
        public ActionResult xoa_NV(string id)
        {
            NHANVIEN nhanvien = db.NHANVIENs.SingleOrDefault(s => s.MA_NhanVien == id);
            if (nhanvien == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nhanvien);
        }
        //[HttpPost, ActionName("xoa_NV")]
        public ActionResult xoa_NV1(string id)
        {
            NHANVIEN nhanvien = db.NHANVIENs.SingleOrDefault(s => s.MA_NhanVien == id);
            db.NHANVIENs.Remove(nhanvien);
            db.SaveChanges();
            return RedirectToAction("ListNV");
        }
        public JsonResult detail_NV(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var nv = db.NHANVIENs.Find(id);
         
            return Json(nv, JsonRequestBehavior.AllowGet);
        }

    }
}
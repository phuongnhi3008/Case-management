using NienLuan2.Helper;
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
            ViewBag.cv1 = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            ViewBag.qsd1 = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.pb1 = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");

            IEnumerable<NHANVIEN> model = db.NHANVIENs;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.HoTen_NV.Contains(searchString) || x.MA_NhanVien.Contains(searchString)).OrderBy(x => x.HoTen_NV);
            }
            ViewBag.SearchString = searchString;
            return View(model.OrderBy(x => x.HoTen_NV).ToPagedList(page, pageSize));
        }

        [HttpGet]
        public ActionResult chitiet_NV(string id)
        {
            var chitiet = db.NHANVIENs.Find(id);
            return View(chitiet);
        }

        [HttpPost, ActionName("them_NV")]
        public ActionResult them_NV(NHANVIEN Nv, FormCollection form)
        {
            Nv.MA_NhanVien = UUID.GetUUID(5);
            Nv.MA_ChucVu = form["cv"].ToString(); ;
            Nv.MA_PhongBan = form["pb"].ToString();
            Nv.MA_QNSD = form["qsd"].ToString();
            if (form["GioiTinh_NV"] != null)
            {
                if ((form["GioiTinh_NV"].ToString() == "on"))
                    Nv.GioiTinh_NV = true;
                else
                    Nv.GioiTinh_NV = false;
            }
            else Nv.GioiTinh_NV = false;
            db.NHANVIENs.Add(Nv);
            db.SaveChanges();
            return RedirectToAction("ListNV");
        }

        public JsonResult Edit_NV(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var nv = db.NHANVIENs.Find(id);

            return Json(nv, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Sua_NV1")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult Sua_NV1(NHANVIEN nv, FormCollection form, string id)
        {
            ViewBag.cv1 = new SelectList(db.CHUCVUs.OrderBy(x => x.TEN_ChucVu), "MA_ChucVu", "TEN_ChucVu");
            ViewBag.qsd1 = new SelectList(db.QUYEN_NSD.OrderBy(x => x.Ten_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.pb1 = new SelectList(db.PHONGBANs.OrderBy(x => x.Ten_PB), "MA_PhongBan", "Ten_PB");

            nv.MA_ChucVu = form["cv1"].ToString(); ;
            nv.MA_QNSD = form["qsd1"].ToString();
            nv.MA_PhongBan = form["pb1"].ToString();

            if (form["GioiTinh_NV"] != null)
            {
                if ((form["GioiTinh_NV"].ToString() == "on"))
                    nv.GioiTinh_NV = true;
                else
                    nv.GioiTinh_NV = false;
            }
            else nv.GioiTinh_NV = false;

            NHANVIEN newNhanVien = db.NHANVIENs.FirstOrDefault(nnv => nnv.MA_NhanVien == nv.MA_NhanVien);
            newNhanVien.MA_ChucVu = nv.MA_ChucVu;
            newNhanVien.MA_QNSD = nv.MA_QNSD;
            newNhanVien.MA_PhongBan = nv.MA_PhongBan;
            newNhanVien.HoTen_NV = nv.HoTen_NV;
            newNhanVien.NamSinh_NV = nv.NamSinh_NV;
            newNhanVien.GioiTinh_NV = nv.GioiTinh_NV;
            newNhanVien.QueQuan_NV = nv.QueQuan_NV;
            newNhanVien.CMND_NV = nv.CMND_NV;
            newNhanVien.SoDienThoai_NV = nv.SoDienThoai_NV;
            newNhanVien.MatKhau = nv.MatKhau;
            newNhanVien.Avatar = nv.Avatar;
            db.SaveChanges();
            return RedirectToAction("ListNV");
        }
        public JsonResult CheckXoa(string id)
        {
            try
            {
                CHITIET_XX chiTietXetXu = db.CHITIET_XX.SingleOrDefault(s => s.MA_NhanVien == id);
                HOSO_VUAN hoSo = db.HOSO_VUAN.SingleOrDefault(s => s.MA_NhanVien == id);
                if (chiTietXetXu == null && hoSo == null)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

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
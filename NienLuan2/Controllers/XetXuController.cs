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
    public class XetXuController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: XetXu
        public ActionResult ListXX(string searcxxtring, int? error, int page = 1, int pageSize = 10)
        {
            ViewBag.hs = new SelectList(db.HOSO_VUAN.OrderBy(x => x.Ten_VuAn), "MA_HoSo", "Ten_VuAn");
            ViewBag.dd = new SelectList(db.DIADIEM_XX.OrderBy(x => x.Ten_DiaDiem), "MA_DiaDiem", "Ten_DiaDiem");
            ViewBag.ks = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.tk = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            ViewBag.hd = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            //List<SelectListItem> selectsHoiDong

            //ViewBag.selectsHoiDong = null;
            ViewBag.cxx = new SelectList(db.CAPXETXUs.OrderBy(x => x.MA_CapXetXu), "MA_CapXetXu", "TenCapXetXu");

            var listXetXu = from s in db.XETXUs select s;

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<XETXU> model = db.XETXUs;

            if (!string.IsNullOrEmpty(searcxxtring))
            {
                model = model.Where(x => x.HOSO_VUAN.Ten_VuAn.Contains(searcxxtring) || x.HOSO_VUAN.DUONGSU.HoTen_DS.Contains(searcxxtring)).OrderByDescending(x => x.HOSO_VUAN.Ten_VuAn);
            }

            ViewBag.Searcxxtring = searcxxtring;

            LichXetXuModel lichXetXuModel = new LichXetXuModel();
            lichXetXuModel.listXetXu = model.OrderByDescending(x => x.STT_XX).ToPagedList(page, pageSize);
            lichXetXuModel.listChiTietXetXu = db.CHITIET_XX.ToList();
            return View(lichXetXuModel);
            //return View(model.OrderByDescending(x => x.STT_XX).ToPagedList(page, pageSize));   
        }
        public ActionResult them_xx()
        {
          return View();
        }

        [HttpPost, ActionName("themLXX")]
        public ActionResult themLXX(XETXU xx, FormCollection form)
        {
            ViewBag.dd = new SelectList(db.DIADIEM_XX.OrderBy(x => x.Ten_DiaDiem), "MA_DiaDiem", "Ten_DiaDiem");
            ViewBag.hs = new SelectList(db.HOSO_VUAN.OrderBy(x => x.MA_HoSo), "MA_HoSo", "Ten_VuAn");
            //ViewBag.hs = new SelectList(db.h.OrderBy(x => x.MA_HoSo), "MA_HoSo", "Ten_VuAn");
            ViewBag.hd = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.ks = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.tk = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.cxx = new SelectList(db.CAPXETXUs.OrderBy(x => x.MA_CapXetXu), "MA_CapXetXu", "TenCapXetXu");
  
          




            if (db.XETXUs.Any(x => x.STT_XX == xx.STT_XX))
            {

                return RedirectToAction("ListXX", new { error = 1 });
            }
            xx.MA_CapXetXu = form["cxx"].ToString();
            xx.MA_DiaDiem = form["dd"].ToString();
            xx.MA_HoSo = form["hs"].ToString();

            if (!ModelState.IsValid)
                return View(xx);
        
            db.XETXUs.Add(xx);
            db.SaveChanges();

            List<string> selectedHoiDongList = form["hd"].Split(',').ToList();

            for(int i = 0; i < selectedHoiDongList.Count; i++)
            {
                CHITIET_XX hoidong = new CHITIET_XX
                {

                    MA_NhanVien = selectedHoiDongList[i],
                    MA_VaiTro = "C4",
                    STT_XX = xx.STT_XX,
                    MA_ChiTietXX = UUID.GetUUID(5)
                };
                themChiTietXetXu(hoidong);
            }
            CHITIET_XX kiemSat = new CHITIET_XX
            {

                MA_NhanVien = form["ks"].ToString(),
                MA_VaiTro = "C3",
                STT_XX = xx.STT_XX,
                MA_ChiTietXX = UUID.GetUUID(5)
            };
            themChiTietXetXu(kiemSat);
            CHITIET_XX thuky = new CHITIET_XX
            {

                MA_NhanVien = form["tk"].ToString(),
                MA_VaiTro = "C1",
                STT_XX = xx.STT_XX,
                MA_ChiTietXX = UUID.GetUUID(5)
            };
            themChiTietXetXu(thuky);
            return RedirectToAction("ListXX");
        }

        public List<DUONGSU> Get_DS(string ds_ma)
        {
            NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
            List<DUONGSU> duongsu = db.DUONGSUs.ToList();
            return duongsu;
        }

        public void themChiTietXetXu(CHITIET_XX ctxx)
        {
            db.CHITIET_XX.Add(ctxx);
            db.SaveChanges();
        }

        public NHANVIEN getNhanVien(XETXU xx, VAITRO_NV vt)
        {
            try
            {
                CHITIET_XX chiTietXetXu = db.CHITIET_XX.Single(x => x.STT_XX.Equals(xx.STT_XX) && x.MA_VaiTro.Equals(vt.MA_VaiTro));
                NHANVIEN nhanVien = db.NHANVIENs.Single(x => x.MA_NhanVien.Equals(chiTietXetXu.MA_NhanVien));
                return nhanVien;
            }
            catch(Exception)
            {
                return null;
            }
           

        }
    }
}
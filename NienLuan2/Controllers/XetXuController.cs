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
            ViewBag.ds = new SelectList(db.DUONGSUs, "MA_DuongSu", "Hoten_DS");
            //List<SelectListItem> selectsHoiDong

            //ViewBag.selectsHoiDong = null;
            ViewBag.cxx = new SelectList(db.CAPXETXUs.OrderBy(x => x.MA_CapXetXu), "MA_CapXetXu", "TenCapXetXu");
   

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<XETXU> model = db.XETXUs;
            if (!string.IsNullOrEmpty(searcxxtring))
            {
                model = model.Where(x => x.HOSO_VUAN.Ten_VuAn.Contains(searcxxtring) || x.HOSO_VUAN.CHITIET_DS.Contains(db.CHITIET_DS.Where(ds => ds.DUONGSU.HoTen_DS.Equals(searcxxtring)).FirstOrDefault())).OrderByDescending(x => x.HOSO_VUAN.Ten_VuAn);
            }

            ViewBag.Searcxxtring = searcxxtring;

            LichXetXuModel lichXetXuModel = new LichXetXuModel();
            lichXetXuModel.listXetXu = model.OrderByDescending(x => x.STT_XX).ToPagedList(page, pageSize);
            lichXetXuModel.listChiTietXetXu = db.CHITIET_XX.ToList();
            //duongsuModel.listXetXu = model.OrderByDescending(x => x.STT_XX).ToPagedList(page, pageSize);
            lichXetXuModel.listXetXu = model.OrderBy(x => x.MA_HoSo).ToPagedList(page, pageSize);
            lichXetXuModel.listChiTietDuongSu = db.CHITIET_DS.ToList();
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

            for (int i = 0; i < selectedHoiDongList.Count; i++)
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

        public ActionResult xoaLXX(int? id)
        {
            XETXU xetxu = db.XETXUs.SingleOrDefault(s => s.STT_XX == id);
                if(xetxu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View();
        }

        //[HttpPost, ActionName("xoaLXX")]
        public ActionResult xoaLXX1(XETXU xx,int? id)
        {
            XETXU xetxu = db.XETXUs.SingleOrDefault(x => x.STT_XX == id);
            db.XETXUs.Remove(xetxu);
            db.SaveChanges();     
            // xóa mấy thằng nhân viên trong bảng chi tiết xét xử trước (xóa data trong bang ChiTietXetXu)
            // select list bảng ChiTietXetXu, check STT_XX (thằng nào bằng xx.STT_XX thì xóa hết)
            // sau đó mới xóa xetxu
            return RedirectToAction("ListXX");
        }

        [HttpPost, ActionName("capNhatLXX")]
        public ActionResult capNhatLXX(XETXU xx, FormCollection form)
        {
            //Tương tự xóa
            //Nếu có cập nhật nhân viên thì chỉ cập nhật trong bảng ChiTietXetXu
            //Còn ko thì cập nhật trong XETXU

           

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
    }
}
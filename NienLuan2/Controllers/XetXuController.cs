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
        public ActionResult ListXX(string searchString, DateTime? tungay, DateTime? denngay, int? error, int page = 1, int pageSize = 10)
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
       
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.HOSO_VUAN.Ten_VuAn.Contains(searchString) || x.HOSO_VUAN.CHITIET_DS.Contains(db.CHITIET_DS.Where(ds => ds.DUONGSU.HoTen_DS.Equals(searchString)).FirstOrDefault())).OrderBy(x => x.HOSO_VUAN.Ten_VuAn);
            }
            ViewBag.SearchString = searchString;


            LichXetXuModel lichXetXuModel = new LichXetXuModel();
         
            lichXetXuModel.listChiTietXetXu = db.CHITIET_XX.ToList();

            if (tungay == null || denngay == null)
            {
                lichXetXuModel.listXetXu = model.OrderBy(x => x.MA_HoSo).ToPagedList(page, pageSize);
                ViewBag.tuNgay = db.XETXUs.Min(hs => hs.Ngay_XetXu).Value;
                ViewBag.denNgay = DateTime.Now;
            }
            else
            {
                List<XETXU> newList = new List<XETXU>();
                foreach (var item in model)
                {
                    if (item.Ngay_XetXu >= tungay && item.Ngay_XetXu <= denngay)
                        newList.Add(item);
                }
                lichXetXuModel.listXetXu = newList.OrderByDescending(x => x.MA_XetXu).ToPagedList(page, pageSize);
                ViewBag.tuNgay = tungay;
                ViewBag.denNgay = denngay;
            }
            lichXetXuModel.listChiTietDuongSu = db.CHITIET_DS.ToList();
            return View(lichXetXuModel);

        }
        public JsonResult GetTuNgay()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var tuNgay = db.XETXUs.Min(xx => xx.Ngay_XetXu).Value;
            return Json(tuNgay, JsonRequestBehavior.AllowGet);
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
            if (db.XETXUs.Any(x => x.MA_XetXu == xx.MA_XetXu))
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

            HOSO_VUAN hoSo = db.HOSO_VUAN.Where(hs => hs.MA_HoSo == xx.MA_HoSo).FirstOrDefault();
            hoSo.MA_TrangThai = "02";
            db.SaveChanges();
            List<string> selectedHoiDongList = form["hd"].Split(',').ToList();

            for (int i = 0; i < selectedHoiDongList.Count; i++)
            {
                CHITIET_XX hoidong = new CHITIET_XX
                {

                    MA_NhanVien = selectedHoiDongList[i],
                    MA_VaiTro = "C4",
                    MA_XetXu = xx.MA_XetXu,
                    MA_ChiTietXX = UUID.GetUUID(5)
                };
                themChiTietXetXu(hoidong);
            }
            CHITIET_XX kiemSat = new CHITIET_XX
            {

                MA_NhanVien = form["ks"].ToString(),
                MA_VaiTro = "C3",
                MA_XetXu = xx.MA_XetXu,
                MA_ChiTietXX = UUID.GetUUID(5)
            };
            themChiTietXetXu(kiemSat);
            CHITIET_XX thuky = new CHITIET_XX
            {

                MA_NhanVien = form["tk"].ToString(),
                MA_VaiTro = "C1",
                MA_XetXu = xx.MA_XetXu,
                MA_ChiTietXX = UUID.GetUUID(5)
            };
            themChiTietXetXu(thuky);
            return RedirectToAction("ListXX");
        }

        public JsonResult GetNgayNhanHoSo(string maHoSo)
        {
            HOSO_VUAN hoSo = db.HOSO_VUAN.FirstOrDefault(ks => ks.MA_HoSo == maHoSo);
            return Json(hoSo.NgayNhan_HS, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckXoa(string id)
        {
            try
            {
                int IntId = Convert.ToInt32(id);
                KETQUA_XX ketQua = db.KETQUA_XX.SingleOrDefault(s => s.MA_XetXu == IntId);
                if (ketQua == null)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

        }
        //[HttpPost, ActionName("xoaLXX")]
        public ActionResult xoaLXX1(XETXU xx,int? id)
        {
            try
            {
                XETXU xetxu = db.XETXUs.SingleOrDefault(x => x.MA_XetXu == id);
                db.XETXUs.Remove(xetxu);
                db.SaveChanges();
                // xóa mấy thằng nhân viên trong bảng chi tiết xét xử trước (xóa data trong bang ChiTietXetXu)
                // select list bảng ChiTietXetXu, check MA_XetXu (thằng nào bằng xx.MA_XetXu thì xóa hết)
                // sau đó mới xóa xetxu
                return RedirectToAction("ListXX");
            }
            catch
            {
                return RedirectToAction("ListXX");
            }
           
        }

        public JsonResult CheckChinhSua(string id)
        {
            try
            {
                int IntId = Convert.ToInt32(id);
                KETQUA_XX ketQua = db.KETQUA_XX.SingleOrDefault(s => s.MA_XetXu == IntId);
                if (ketQua == null)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult Edit_XX(string id)
        {
            int intMaXetXu = Convert.ToInt32(id);
            db.Configuration.ProxyCreationEnabled = false;
            var xetxu = db.XETXUs.Find(intMaXetXu);
            return Json(xetxu, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListHoiDong(string maXetXu)
        {
            int intMaXetXu = Convert.ToInt32(maXetXu);
            IEnumerable<SelectListItem> listHoiDong = db.CHITIET_XX.AsNoTracking()
                                    .OrderBy(n => n.MA_XetXu)
                                    .Where(n => n.MA_XetXu == intMaXetXu && n.MA_VaiTro == "C4")
                                    .Select(n =>
                                       new SelectListItem
                                       {
                                           Value = n.MA_NhanVien,
                                           Text = n.NHANVIEN.HoTen_NV
                                       }).ToList();
            return Json(new SelectList(listHoiDong, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetThuKy(string maXetXu)
        {
            int intMaXetXu = Convert.ToInt32(maXetXu);
            CHITIET_XX thuKy = db.CHITIET_XX.FirstOrDefault(ks => ks.MA_XetXu == intMaXetXu && ks.MA_VaiTro == "C1");
            return Json(thuKy.MA_NhanVien, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKiemSat(string maXetXu)
        {
            int intMaXetXu = Convert.ToInt32(maXetXu);
            CHITIET_XX kiemSat = db.CHITIET_XX.FirstOrDefault(ks => ks.MA_XetXu == intMaXetXu && ks.MA_VaiTro == "C3");
            return Json(kiemSat.MA_NhanVien, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Edit_XX1")]
        public ActionResult Edit_XX1(XETXU xx, FormCollection form)
        {

            //Tương tự xóa
            //Nếu có cập nhật nhân viên thì chỉ cập nhật trong bảng ChiTietXetXu
            //Còn ko thì cập nhật trong XETXU
            XETXU newXetXu = db.XETXUs.FirstOrDefault(nxx => nxx.MA_XetXu == xx.MA_XetXu);
            newXetXu.Ngay_XetXu = xx.Ngay_XetXu;
            newXetXu.MA_DiaDiem = form["dd"].ToString();
 


            CHITIET_XX newThuKy = db.CHITIET_XX.FirstOrDefault(ntk => ntk.MA_XetXu == xx.MA_XetXu && ntk.MA_VaiTro == "C1");
            newThuKy.MA_NhanVien = form["tk"].ToString();

            CHITIET_XX newKiemSat = db.CHITIET_XX.FirstOrDefault(nks => nks.MA_XetXu == xx.MA_XetXu && nks.MA_VaiTro == "C3");
            newKiemSat.MA_NhanVien = form["ks"].ToString();

            ClearHoiDong(xx.MA_XetXu);
            List<string> selectedHoiDongList = form["hd"].Split(',').ToList();

            for (int i = 0; i < selectedHoiDongList.Count; i++)
            {
                CHITIET_XX hoidong = new CHITIET_XX
                {

                    MA_NhanVien = selectedHoiDongList[i],
                    MA_VaiTro = "C4",
                    MA_XetXu = xx.MA_XetXu,
                    MA_ChiTietXX = UUID.GetUUID(5)
                };
                themChiTietXetXu(hoidong);
            }
            db.SaveChanges();
            return RedirectToAction("ListXX");

        }
        public void ClearHoiDong(int maXetXu)
        {
            foreach (var item in db.CHITIET_XX.ToList())
            {
                if (item.MA_XetXu == maXetXu && item.MA_VaiTro == "C4")
                {
                    db.CHITIET_XX.Remove(item);
                    db.SaveChanges();
                }
            }
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
        [HttpGet]
        public ActionResult ChiTiet_XX(string id)
        {
            int intId = Convert.ToInt32(id);
            var chitiet = db.XETXUs.Find(intId);
            return View(chitiet);
        }
    }
}
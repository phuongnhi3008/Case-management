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
    public class HoSoVuAnController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: HoSoVuAn

        //public JsonResult ListHoSo(String q)
        //{
        //    var
        //}
        public ActionResult ListHS(string searchString, int? error, int page= 1, int pageSize = 10)
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
            ViewBag.vtnv = new SelectList(db.VAITRO_NV.OrderBy(x => x.Ten_VT), "MA_VaiTro", "Ten_VT");
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<HOSO_VUAN> model = db.HOSO_VUAN;
           
            if(!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Ten_VuAn.Contains(searchString) || x.MA_HoSo.Contains(searchString)).OrderByDescending(x => x.Ten_VuAn);
            }

            ViewBag.SearchString = searchString;
            return View(model.OrderByDescending(x=> x.Ten_VuAn).ToPagedList(page, pageSize));
        }
        public ActionResult them_HS()
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
  
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            return View();
        }

        [HttpPost, ActionName("them_HS")]
        public ActionResult them_HS(HOSO_VUAN hs, FormCollection form)
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
            
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");


            if(db.HOSO_VUAN.Any(x => x.MA_HoSo == hs.MA_HoSo))
            {
                //ViewBag.error = "Mã hồ sơ này đã tồn tại!!!";
                return RedirectToAction("ListHS", new { error = 1 });
            }

            hs.MA_LoaiVA = form["lva"].ToString(); ;
            hs.MA_TrangThai = form["tths"].ToString();
          
            hs.MA_NhanVien = form["mnv"].ToString();

            if (!ModelState.IsValid)
                return View(hs);

            db.HOSO_VUAN.Add(hs);
            db.SaveChanges();
            var list_HS = from s in db.HOSO_VUAN select s;
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
        
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            return RedirectToAction("ListHS");
        }
        
        public JsonResult Edit_HS(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var hoso = db.HOSO_VUAN.Find(id);

            return Json(hoso, JsonRequestBehavior.AllowGet);
        }


        public ActionResult sua_HS(string id)
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
   
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSO_VUAN hoso = db.HOSO_VUAN.Find(id);
            return View(hoso);
        }

        [HttpPost, ActionName("sua_HS1")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult sua_HS1(HOSO_VUAN hs, FormCollection form, string id)
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
          
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            hs.MA_LoaiVA = form["lva"].ToString(); ;
            hs.MA_TrangThai = form["tths"].ToString();
          
            hs.MA_NhanVien = form["mnv"].ToString();
            //hs.MA_HoSo = id;

            if (!ModelState.IsValid)
                return View(hs);

            if (TryUpdateModel(hs, "", new string[] { "MA_HoSo", "Ten_VuAn" }))
            {
                try
                {
                    db.Entry(hs).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Lỗi");
                }

            }
            return RedirectToAction("ListHS");
        }
        public ActionResult xoa_HS(string id)
        {
            HOSO_VUAN hoso = db.HOSO_VUAN.SingleOrDefault(s => s.MA_HoSo == id);
            if (hoso == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hoso);
        }
        //[HttpPost, ActionName("xoa_HS")]
        public ActionResult xoa_HS1(string id)
        {
            HOSO_VUAN hoso = db.HOSO_VUAN.SingleOrDefault(s => s.MA_HoSo == id);
            db.HOSO_VUAN.Remove(hoso);
            db.SaveChanges();
            return RedirectToAction("ListHS");
        }



        public ActionResult ChiTiet_HS(string id)
        {
            var model = (from hsva in db.HOSO_VUAN
                         join xx in db.XETXUs on hsva.MA_HoSo equals xx.MA_HoSo
                         join ds in db.DUONGSUs on xx.MA_DuongSu equals ds.MA_DuongSu
                         join nv in db.NHANVIENs on hsva.MA_NhanVien equals nv.MA_NhanVien
                         join tt in db.TRANGTHAI_HS on hsva.MA_TrangThai equals tt.MA_TrangThai
                         select new ModelsXetXu
                         {
                             MA_HoSo = xx.MA_HoSo,
                             MA_NhanVien = hsva.MA_NhanVien,
                             MA_TrangThai = hsva.MA_TrangThai,
                             MA_LoaiVA = hsva.MA_LoaiVA,
                             Ten_VuAn = hsva.Ten_VuAn,
                             NoiDung_VA = hsva.NoiDung_VA,
                             Ten_TT = tt.Ten_TT,
                             Loai_HS = hsva.Loai_HS,
                             NgayNhan_HS = hsva.NgayNhan_HS,
                             HoTen_NV = nv.HoTen_NV,
                             Ngay_XetXu = xx.Ngay_XetXu,
                             Lan_XetXu = xx.Lan_XetXu,
                             MA_DiaDiem = xx.MA_DiaDiem,
                             MA_DuongSu = xx.MA_DuongSu,
                             KetQua_XX = xx.KetQua_XX,
                             MA_LoaiDS = ds.MA_LoaiDS,
                             CMND = ds.CMND,
                             HoTen_DS = ds.HoTen_DS,
                             NamSinh_DS = ds.NamSinh_DS,
                             QueQuan_DS = ds.QueQuan_DS,
                             DiaChi_DS = ds.DiaChi_DS,
                             SoDienThoai_DS = ds.SoDienThoai_DS,
                             GioiTinh_DS = ds.GioiTinh_DS,
                         }).SingleOrDefault(m => m.MA_HoSo == id);
            return View(model);
        }
    }
}
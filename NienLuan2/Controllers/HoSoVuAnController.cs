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
    public class HoSoVuAnController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: HoSoVuAn

        //public JsonResult ListHoSo(String q)
        //{
        //    var
        //}
        public ActionResult ListHS(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");        
            ViewBag.vtnv = new SelectList(db.VAITRO_NV.OrderBy(x => x.Ten_VT), "MA_VaiTro", "Ten_VT");
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.nd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            ViewBag.bd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<HOSO_VUAN> model = db.HOSO_VUAN;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Ten_VuAn.Contains(searchString) || x.MA_HoSo.Contains(searchString)).OrderByDescending(x => x.Ten_VuAn);
            }        
            ViewBag.SearchString = searchString;
            DuongSuModel duongSuModel = new DuongSuModel();
            duongSuModel.listHoSoVuAn = model.OrderByDescending(x => x.MA_HoSo).ToPagedList(page, pageSize);
            duongSuModel.listChiTietDuongSu = db.CHITIET_DS.ToList();
       
            return View(duongSuModel);
        }
        public void themChiTietDuongSu(CHITIET_DS ctds)
        {
            db.CHITIET_DS.Add(ctds);
            db.SaveChanges();
        }
        public ActionResult them_HS()
        {
            return View();
        }

        [HttpPost, ActionName("them_HS")]
        public ActionResult them_HS(HOSO_VUAN hs, FormCollection form)
        {
            if (db.HOSO_VUAN.Any(x => x.MA_HoSo == hs.MA_HoSo))
            {
                //ViewBag.error = "Mã hồ sơ này đã tồn tại!!!";
                return RedirectToAction("ListHS", new { error = 1 });
            }

            hs.MA_LoaiVA = form["lva"].ToString();
            hs.MA_TrangThai = "01";
            hs.MA_NhanVien = form["mnv"].ToString();

            if (!ModelState.IsValid)
                return View(hs);

            //Tao ma tu dong
            hs.MA_HoSo = UUID.GetUUID(5);

            db.HOSO_VUAN.Add(hs);
            db.SaveChanges();
            List<string> selectedNguyenDonList = form["nd"].Split(',').ToList();

            for (int i = 0; i < selectedNguyenDonList.Count; i++)
            {
                CHITIET_DS nguyendon = new CHITIET_DS
                {
                    MA_DuongSu = selectedNguyenDonList[i],
                    MA_LoaiDS = "ND",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(nguyendon);
            }
            List<string> selectedBiDonList = form["bd"].Split(',').ToList();

            for (int i = 0; i < selectedBiDonList.Count; i++)
            {
                CHITIET_DS bidon = new CHITIET_DS
                {
                    MA_DuongSu = selectedBiDonList[i],
                    MA_LoaiDS = "BD",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(bidon);
            }

            var list_HS = from s in db.HOSO_VUAN select s;
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
            ViewBag.nd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            ViewBag.bd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            return RedirectToAction("ListHS");
        }

        public JsonResult Edit_HS(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var hoso = db.HOSO_VUAN.Find(id);

            return Json(hoso, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetListNguyenDon(string maHoSo)
        {
            IEnumerable<SelectListItem> listNguyenDon = db.CHITIET_DS.AsNoTracking()
                                    .OrderBy(n => n.DUONGSU.HoTen_DS)
                                    .Where(n => n.MA_HoSo == maHoSo && n.MA_LoaiDS == "nd")
                                    .Select(n =>
                                       new SelectListItem
                                       {
                                           Value = n.MA_DuongSu,
                                           Text = n.DUONGSU.HoTen_DS
                                       }).ToList();
            return Json(new SelectList(listNguyenDon, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
            
        public JsonResult GetListBiDon(string maHoSo)
        {
            IEnumerable<SelectListItem> listBiDon = db.CHITIET_DS.AsNoTracking()
                                    .OrderBy(n => n.DUONGSU.HoTen_DS)
                                    .Where(n => n.MA_HoSo == maHoSo && n.MA_LoaiDS == "bd")
                                    .Select(n =>
                                       new SelectListItem
                                       {
                                           Value = n.MA_DuongSu,
                                           Text = n.DUONGSU.HoTen_DS
                                       }).ToList();
            return Json(new SelectList(listBiDon, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("sua_HS1")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult sua_HS1(HOSO_VUAN hs, FormCollection form, string id)
        {
            //ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            //ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");

            //ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            hs.MA_LoaiVA = form["lva"].ToString(); ;
            hs.MA_TrangThai = form["tths"].ToString();

            hs.MA_NhanVien = form["mnv"].ToString();
            //hs.MA_HoSo = id;

            if (!ModelState.IsValid)
                return View(hs);

            if (TryUpdateModel(hs, "", new string[] { "MA_HoSo", "NgayNhan_HS", "Ten_VuAn", "MA_NhanVien", "MA_LoaiVuAn", "NoiDung_VA", "Loai_HS", "MA_TrangThai" }))
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
            ClearNguyenDonBiDon(hs.MA_HoSo);

            List<string> selectedNguyenDonList = form["nd"].Split(',').ToList();

            for (int i = 0; i < selectedNguyenDonList.Count; i++)
            {
                CHITIET_DS nguyendon = new CHITIET_DS
                {
                    MA_DuongSu = selectedNguyenDonList[i],
                    MA_LoaiDS = "ND",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(nguyendon);
            }
            List<string> selectedBiDonList = form["bd"].Split(',').ToList();

            for (int i = 0; i < selectedBiDonList.Count; i++)
            {
                CHITIET_DS bidon = new CHITIET_DS
                {
                    MA_DuongSu = selectedBiDonList[i],
                    MA_LoaiDS = "BD",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(bidon);
            }
            return RedirectToAction("ListHS");
        }

        public void ClearNguyenDonBiDon(string maHoSo)
        {
            foreach(var item in db.CHITIET_DS.ToList())
            {
                if (item.MA_HoSo == maHoSo)
                {
                    db.CHITIET_DS.Remove(item);
                    db.SaveChanges();
                }
            }
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
        public JsonResult CheckXoa(string id)
        {
            try {
                XETXU hoso = db.XETXUs.SingleOrDefault(s => s.MA_HoSo == id);
                if (hoso == null)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
           
        }
        //[HttpPost, ActionName("xoa_HS")]
        public ActionResult xoa_HS1(string id)
        {
            try
            {
                HOSO_VUAN hoso = db.HOSO_VUAN.SingleOrDefault(s => s.MA_HoSo == id);
                ClearNguyenDonBiDon(hoso.MA_HoSo);
                db.HOSO_VUAN.Remove(hoso);
                db.SaveChanges();
                return RedirectToAction("ListHS");
            }
            catch
            {
                return RedirectToAction("ListHS");
            }

            
    }



        public ActionResult ChiTiet_HS(string id)
        {
            var model = (from hsva in db.HOSO_VUAN
                         join xx in db.XETXUs on hsva.MA_HoSo equals xx.MA_HoSo
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
                             NgayNhan_HS = hsva.NgayNhan_HS.Value.ToString("dd/MM/yyyy"),
                             HoTen_NV = nv.HoTen_NV,
                             Ngay_XetXu = xx.Ngay_XetXu.Value.ToString("dd/MM/yyyy"),
                             Lan_XetXu = xx.Lan_XetXu,
                             MA_DiaDiem = xx.MA_DiaDiem,
                         }).SingleOrDefault(m => m.MA_HoSo == id);
            return View(model);
        }

        public JsonResult GetThongKe_HS(string id)
        {
            using (NL2_QLVAEntities1 db = new NL2_QLVAEntities1())
            {
                var a = (from hs in db.HOSO_VUAN            
                         join nv in db.NHANVIENs on hs.MA_NhanVien equals nv.MA_NhanVien
                         join lva in db.LOAI_VUAN on hs.MA_LoaiVA equals lva.MA_LoaiVA
                         join tt in db.TRANGTHAI_HS on hs.MA_TrangThai equals tt.MA_TrangThai
                         select new ModelsXetXu
                         {
                             MA_HoSo = hs.MA_HoSo,
                             Ten_VuAn = hs.Ten_VuAn,
                             Loai_HS = hs.Loai_HS,
                             NgayNhan_HS = hs.NgayNhan_HS.Value.ToString("dd/MM/yyyy"),
                             MA_TrangThai = hs.MA_TrangThai,
                             MA_NhanVien = hs.MA_NhanVien,
                             Ten_TT = tt.Ten_TT,
                             MA_LoaiVA = hs.MA_LoaiVA,                  
                             HoTen_NV = nv.HoTen_NV,                 
                         }).ToList();
                return Json(a, JsonRequestBehavior.AllowGet);
            }
        }
        public List<LOAI_VUAN> GetThongKe_HS()
        {
            NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
            List<LOAI_VUAN> loaiva = db.LOAI_VUAN.OrderByDescending(x => x.MA_LoaiVA).ToList();
            return loaiva;
        }
        public ActionResult ThongKe_HS()
        {
            ViewBag.loaiva = new SelectList(GetThongKe_HS(), "MA_LoaiVA", "Ten_LoaiVA");
            ModelsXetXu xx = new ModelsXetXu();
            return View();
        }
    }
}
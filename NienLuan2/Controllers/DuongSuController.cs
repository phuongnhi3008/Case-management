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
    public class DuongSuController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: HoSoVuAn

        //public JsonResult ListHoSo(String q)
        //{
        //    var
        //}
        public ActionResult ListDS(string searchString, int? error, int page = 1, int pageSize = 5)
        {
            
           if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<DUONGSU> model = db.DUONGSUs;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.HoTen_DS.Contains(searchString) || x.MA_DuongSu.Contains(searchString)).OrderByDescending(x => x.HoTen_DS);
            }

            ViewBag.SearchString = searchString;
            return View(model.OrderByDescending(x => x.HoTen_DS).ToPagedList(page, pageSize));
        }
        public ActionResult them_DS()
        {          
            return View();
        }

        [HttpPost, ActionName("them_DS")]
        public ActionResult them_DS(DUONGSU ds, FormCollection form)
        {               
            ds.MA_DuongSu = UUID.GetUUID(5);
            ds.MA_LoaiDS = "ND";
            if (form["GioiTinh_DS"] != null)
            {
                if ((form["GioiTinh_DS"].ToString() == "on"))
                    ds.GioiTinh_DS = true;
                else
                    ds.GioiTinh_DS = false;
            }
            db.DUONGSUs.Add(ds);
            db.SaveChanges();    
            return RedirectToAction("ListDS");
        }

        public JsonResult Edit_DS(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var duongsu = db.DUONGSUs.Find(id);

            return Json(duongsu, JsonRequestBehavior.AllowGet);
        }


        public ActionResult sua_DS(string id)
        {
            ViewBag.lds = new SelectList(db.LOAI_DS.OrderBy(x => x.Ten_LoaiDS), "MA_LoaiDS", "Ten_LoaiDS");
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DUONGSU duongsu = db.DUONGSUs.Find(id);
            return View(duongsu);
        }

        [HttpPost, ActionName("sua_DS1")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult sua_DS1(DUONGSU ds, FormCollection form, string id)
        {
            if (form["GioiTinh_DS"] != null)
            {
                if ((form["GioiTinh_DS"].ToString() == "on"))
                    ds.GioiTinh_DS = true;
                else
                    ds.GioiTinh_DS = false;
            }
            else ds.GioiTinh_DS = false;

            DUONGSU newDuongSu = db.DUONGSUs.FirstOrDefault(nnv => nnv.MA_DuongSu == ds.MA_DuongSu);
     
            newDuongSu.CMND = ds.CMND;
            newDuongSu.HoTen_DS = ds.HoTen_DS;
            newDuongSu.NamSinh_DS = ds.NamSinh_DS;
            newDuongSu.QueQuan_DS = ds.QueQuan_DS;
            newDuongSu.DiaChi_DS = ds.DiaChi_DS;
            newDuongSu.SoDienThoai_DS = ds.SoDienThoai_DS;
            newDuongSu.GioiTinh_DS = ds.GioiTinh_DS;

            db.SaveChanges();



            return RedirectToAction("ListDS");
        }
        public JsonResult CheckXoa(string id)
        {
            try
            {
                CHITIET_DS chiTietDuongSu = db.CHITIET_DS.SingleOrDefault(s => s.MA_DuongSu == id);
                if (chiTietDuongSu == null)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult xoa_DS1(string id)
        {
            DUONGSU duongsu = db.DUONGSUs.SingleOrDefault(s => s.MA_DuongSu == id);
            db.DUONGSUs.Remove(duongsu);
            db.SaveChanges();
            return RedirectToAction("ListDS");
        }
    }
}
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
            ViewBag.lds = new SelectList(db.LOAI_DS.OrderBy(x => x.Ten_LoaiDS), "MA_LoaiDS", "Ten_LoaiDS");
            

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
            ViewBag.lds = new SelectList(db.LOAI_DS.OrderBy(x => x.Ten_LoaiDS), "MA_LoaiDS", "Ten_LoaiDS");
            
            return View();
        }

        [HttpPost, ActionName("them_DS")]
        public ActionResult them_DS(DUONGSU ds, FormCollection form)
        {
            ViewBag.lds = new SelectList(db.LOAI_DS.OrderBy(x => x.Ten_LoaiDS), "MA_LoaiDS", "Ten_LoaiDS");
            


            if (db.DUONGSUs.Any(x => x.MA_DuongSu == ds.MA_DuongSu))
            {
                //ViewBag.error = "Mã hồ sơ này đã tồn tại!!!";
                return RedirectToAction("ListDS", new { error = 1 });
            }

            ds.MA_LoaiDS = form["lds"].ToString(); ;
          

            if (!ModelState.IsValid)
                return View(ds);

            db.DUONGSUs.Add(ds);
            db.SaveChanges();
            var list_DS = from s in db.DUONGSUs select s;
            ViewBag.lds = new SelectList(db.LOAI_DS.OrderBy(x => x.Ten_LoaiDS), "MA_LoaiDS", "Ten_LoaiDS");
            
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
            ViewBag.lds = new SelectList(db.LOAI_DS.OrderBy(x => x.Ten_LoaiDS), "MA_LoaiDS", "Ten_LoaiDS");
            

            ds.MA_LoaiDS = form["lds"].ToString(); ;
            
            //hs.MA_HoSo = id;

            if (!ModelState.IsValid)
                return View(ds);

            if (TryUpdateModel(ds, "", new string[] { "MA_DuongSu", "HoTen_DS" }))
            {
                try
                {

                    db.Entry(ds).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Lỗi");
                }

            }
            return RedirectToAction("ListDS");
        }
        public ActionResult xoa_DS(string id)
        {
            DUONGSU duongsu = db.DUONGSUs.SingleOrDefault(s => s.MA_DuongSu == id);
            if (duongsu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(duongsu);
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
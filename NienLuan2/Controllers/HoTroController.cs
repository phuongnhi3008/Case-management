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
using static System.Collections.Specialized.BitVector32;

namespace NienLuan2.Controllers
{
    public class HoTroController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: HoTro
        public ActionResult ListHT( int? error, int page = 1, int pageSize = 10)
        {
            ViewBag.tt = new SelectList(db.TRANGTHAI_HOTRO.OrderBy(x => x.Ten_TT_HT), "MA_TT_HT", "Ten_TT_HT");
            var listHoTro = from s in db.HOTROes select s;

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<HOTRO> model = db.HOTROes;

            
            return View(model.OrderByDescending(x => x.TRANGTHAI_HOTRO.MA_TT_HT).ToPagedList(page, pageSize));
            
        }
        public ActionResult xoa_HT(int? id)
        {
            HOTRO hotro = db.HOTROes.SingleOrDefault(s => s.MA_HT == id);
            if (hotro == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hotro);
        }
        //[HttpPost, ActionName("xoa_HT")]
        public ActionResult xoa_HT1(int? id)
        {
            HOTRO hotro = db.HOTROes.SingleOrDefault(s => s.MA_HT == id);
            db.HOTROes.Remove(hotro);
            db.SaveChanges();
            return RedirectToAction("ListHT");
        }
        public JsonResult Edit_TT_HT(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var hotro = db.HOTROes.SingleOrDefault(s => s.MA_TT_HT == id);

            return Json(hotro, JsonRequestBehavior.AllowGet);
        }


        public ActionResult sua_TT_HT(string id)
        {
            ViewBag.tt = new SelectList(db.TRANGTHAI_HOTRO.OrderBy(x => x.Ten_TT_HT), "MA_TT_HT", "Ten_TT_HT");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOTRO hotro = db.HOTROes.Find(id);
            return View(hotro);
        }

        [HttpPost, ActionName("sua_TT_HT1")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult sua_TT_HT1(HOTRO ht, FormCollection form, string id)
        {
            ViewBag.tt = new SelectList(db.TRANGTHAI_HOTRO.OrderBy(x => x.Ten_TT_HT), "MA_TT_HT", "Ten_TT_HT");
            ht.MA_TT_HT = form["tt"].ToString();
            //hs.MA_hotro = id;

            if (!ModelState.IsValid)
                return View(ht);

            if (TryUpdateModel(ht, "", new string[] { "MA_TT_HT", "Ten_TT_HT" }))
            {
                try
                {
                    db.Entry(ht).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Lỗi");
                }

            }
            return RedirectToAction("ListHT");
        }
    }
}


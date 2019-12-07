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
        public ActionResult ListHT(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                ViewBag.tt = new SelectList(db.TRANGTHAI_HOTRO.OrderBy(x => x.Ten_TT_HT), "MA_TT_HT", "Ten_TT_HT");
                var listHoTro = from s in db.HOTROes select s;

                IEnumerable<HOTRO> model = db.HOTROes;


                return View(model.OrderByDescending(x => x.TRANGTHAI_HOTRO.MA_TT_HT).ToPagedList(page, pageSize));
            }

            else
            {
                ViewBag.tt = new SelectList(db.TRANGTHAI_HOTRO.OrderBy(x => x.Ten_TT_HT), "MA_TT_HT", "Ten_TT_HT");
                var listHoTro = from s in db.HOTROes select s;

                IEnumerable<HOTRO> model = db.HOTROes.Where(ht => ht.SDT_HT.Contains(searchString));
                ViewBag.SearchString = searchString;

                return View(model.OrderByDescending(x => x.TRANGTHAI_HOTRO.MA_TT_HT).ToPagedList(page, pageSize));
            }
           
            
        }

        public ActionResult xoa_HT1(int? id)
        {
            HOTRO hotro = db.HOTROes.SingleOrDefault(s => s.MA_HT == id);
            db.HOTROes.Remove(hotro);
            db.SaveChanges();
            return RedirectToAction("ListHT");
        }
        
        [HttpGet]
        public ActionResult EditTTHT(string id)
        {
            try
            {
                int idInt = Convert.ToInt32(id);
                HOTRO hotro = db.HOTROes.SingleOrDefault(ht => ht.MA_HT == idInt);
                if (hotro.MA_TT_HT == "01")
                    hotro.MA_TT_HT = "02";
                else
                    hotro.MA_TT_HT = "01";
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);  
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}


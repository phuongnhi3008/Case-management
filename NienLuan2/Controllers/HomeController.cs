using NienLuan2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NienLuan2.Controllers
{
   
    public class HomeController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult them_HT()
        {
            return View();
        }

        [HttpPost, ActionName("them_HT")]
        public ActionResult them_HT(HOTRO ht, FormCollection form)
        {
            if (!ModelState.IsValid)
                return View(ht);

            db.HOTROes.Add(ht);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
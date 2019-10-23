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
    public class MenuController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        public ActionResult ListMenu(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            ViewBag.vt = new SelectList(db.QUYEN_NSD.OrderBy(x => x.MA_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.main = new SelectList(db.MAIN_MENU.OrderBy(x => x.ID_Main), "ID_Main", "Ten_Main");
            ViewBag.vt1 = new SelectList(db.QUYEN_NSD.OrderBy(x => x.MA_QNSD), "MA_QNSD", "Ten_QNSD");
            ViewBag.main1 = new SelectList(db.MAIN_MENU.OrderBy(x => x.ID_Main), "ID_Main", "Ten_Main");
            ViewBag.controller = new SelectList(db.CONTROLLERs.OrderBy(x => x.MA_Controller), "MA_Controller", "Ten_Controller_VietSub");

            ViewBag.action = new SelectList(db.ACTIONs.OrderBy(x => x.MA_Action), "MA_Action", "Ten_Action_Viet");

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<SUB_MENU> model = db.SUB_MENU;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.ACTION.CONTROLLER.Ten_Controller_code.Contains(searchString) || x.ACTION.Ten_Action_code.Contains(searchString) || x.MAIN_MENU.Ten_Main.Contains(searchString) || x.QUYEN_NSD.Ten_QNSD.Contains(searchString)).OrderBy(x => x.MA_QNSD);
            }

            ViewBag.SearchString = searchString;
            return View(model.ToList());
        }

        public ActionResult them_Menu()
        {
            //ViewBag.vt = new SelectList(db.QUYEN_NSD.OrderBy(x => x.MA_QNSD), "MA_QNSD", "Ten_QNSD");
            //ViewBag.main = new SelectList(db.MAIN_MENU.OrderBy(x => x.ID_Main), "ID_Main", "Ten_Main");
            return View();
        }

        [HttpPost, ActionName("them_Menu")]
        public ActionResult them_VT(SUB_MENU sub, FormCollection form)
        {
            //if (db.SUB_MENU.Any(x => x.ID_SUB == sub.ID_SUB))
            //{
            //    return View(sub);
            //}
            sub.MA_QNSD = form["vt"].ToString();
            sub.ID_Main = int.Parse(form["main"].ToString());
            sub.MA_Action = form["action"].ToString();
            //if (!ModelState.IsValid)
            //    return View(sub);
            db.SUB_MENU.Add(sub);
            db.SaveChanges();
            return RedirectToAction("ListMenu");
        }

        public ActionResult xoa_Menu(int? id)
        {
            SUB_MENU menu = db.SUB_MENU.SingleOrDefault(s => s.ID_SUB == id);
            if (menu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(menu);
        }
        //[HttpPost, ActionName("xoa_NV")]
        public ActionResult xoa_Menu1(int? id)
        {
            SUB_MENU menu = db.SUB_MENU.SingleOrDefault(s => s.ID_SUB == id);
            db.SUB_MENU.Remove(menu);
            db.SaveChanges();
            return RedirectToAction("ListMenu");
        }

        public JsonResult suaMenu(int? id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var submenu = db.SUB_MENU.Find(id);

            return Json(submenu, JsonRequestBehavior.AllowGet);
        }
        public ActionResult sua_Menu(int? id)
        {
            ViewBag.vt1 = new SelectList(db.QUYEN_NSD.OrderBy(x => x.MA_QNSD), "MA_QNSD", "Ten_NSD");
            ViewBag.main1 = new SelectList(db.MAIN_MENU.OrderBy(x => x.ID_Main), "ID_Main", "Ten_Main");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SUB_MENU submenu = db.SUB_MENU.Find(id);
            return View(submenu);
        }

        [HttpPost, ActionName("sua_Menu1")]
        public ActionResult sua_Menu1(SUB_MENU sub, FormCollection form, int? id)
        {
            ViewBag.vt1 = new SelectList(db.QUYEN_NSD.OrderBy(x => x.MA_QNSD), "MA_QNSD", "Ten_NSD");
            ViewBag.main1 = new SelectList(db.MAIN_MENU.OrderBy(x => x.ID_Main), "ID_Main", "Ten_Main");
            sub.MA_QNSD = form["vt1"].ToString();
            sub.ID_Main = int.Parse(form["main1"].ToString());


            if (!ModelState.IsValid)
                return View(sub);
            if (TryUpdateModel(sub, "", new string[] { "ID_SUB", "Ten_SUB" }))
            {
                try
                {
                    db.Entry(sub).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Lỗi");
                }

            }
            return RedirectToAction("ListMenu");
        }


        public ActionResult ListVT(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<QUYEN_NSD> model = db.QUYEN_NSD;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.MA_QNSD.Contains(searchString) || x.Ten_QNSD.Contains(searchString)).OrderBy(x => x.MA_QNSD);
            }

            ViewBag.SearchString = searchString;
            return View(model.OrderBy(x => x.MA_QNSD).ToPagedList(page, pageSize));
            //return View(db.LO_TRINH);
        }

        public ActionResult AddVT()
        {
            return View();
        }

        [HttpPost, ActionName("AddVT")]
        public ActionResult AddVT(QUYEN_NSD vt, FormCollection form)
        {
            if (db.QUYEN_NSD.Any(x => x.MA_QNSD == vt.MA_QNSD))
            {
                return View(vt);
            }

            if (!ModelState.IsValid)
                return View(vt);
            db.QUYEN_NSD.Add(vt);
            db.SaveChanges();
            return RedirectToAction("ListVT");
        }

        public ActionResult xoa_VT(string id)
        {
            QUYEN_NSD vaitro = db.QUYEN_NSD.SingleOrDefault(s => s.MA_QNSD == id);
            if (vaitro == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(vaitro);
        }
        //[HttpPost, ActionName("xoa_NV")]
        public ActionResult xoa_VT1(string id)
        {
            QUYEN_NSD vaitro = db.QUYEN_NSD.SingleOrDefault(s => s.MA_QNSD == id);
            if (db.NHANVIENs.Any(x => x.MA_QNSD == id))
            {
                //ViewBag.error = "Vui lòng xóa nhân viên thuộc phòng ban trước !!";
                int error = 1;
                return RedirectToAction("ListVT", new { error });
            }
            db.QUYEN_NSD.Remove(vaitro);
            db.SaveChanges();
            return RedirectToAction("ListVT");
        }



        public ActionResult ListMenuChinh(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<MAIN_MENU> model = db.MAIN_MENU;

            return View(model.OrderBy(x => x.ID_Main).ToPagedList(page, pageSize));

        }

        public ActionResult them_Main()
        {
            return View();
        }

        [HttpPost, ActionName("them_Main")]
        public ActionResult them_Main(MAIN_MENU main, FormCollection form)
        {
            if (db.MAIN_MENU.Any(x => x.ID_Main == main.ID_Main))
            {
                return View(main);
            }

            if (!ModelState.IsValid)
                return View(main);
            db.MAIN_MENU.Add(main);
            db.SaveChanges();
            return RedirectToAction("ListMenuChinh");
        }

        public ActionResult xoa_Main(int? id)
        {
            MAIN_MENU main = db.MAIN_MENU.SingleOrDefault(s => s.ID_Main == id);
            if (main == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(main);
        }
        //[HttpPost, ActionName("xoa_NV")]
        public ActionResult xoa_MAIN1(int? id)
        {
            MAIN_MENU main = db.MAIN_MENU.SingleOrDefault(s => s.ID_Main == id);
            if (db.SUB_MENU.Any(x => x.ID_Main == id))
            {
                //ViewBag.error = "Vui lòng xóa nhân viên thuộc phòng ban trước !!";
                int error = 1;
                return RedirectToAction("ListMenuChinh", new { error });
            }
            db.MAIN_MENU.Remove(main);
            db.SaveChanges();
            return RedirectToAction("ListMenuChinh");
        }

        [HttpGet]
        public ActionResult GetActions(string iso3)
        {
            try
            {
                db.ACTIONs.Where(x => x.MA_Controller.Equals(iso3)).ToList();
                IEnumerable<SelectListItem> actions = db.ACTIONs.AsNoTracking()
                        .OrderBy(n => n.Ten_Action_Viet)
                        .Where(n => n.MA_Controller == iso3)
                        .Select(n =>
                           new SelectListItem
                           {
                               Value = n.MA_Action,
                               Text = n.Ten_Action_Viet
                           }).ToList();
                return Json(new SelectList(actions, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
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

    public class PhongBanController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: PhongBan
        public ActionResult ListPB(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            var listPhongBan = from s in db.PHONGBANs select s;

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<PHONGBAN> model = db.PHONGBANs;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Ten_PB.Contains(searchString) || x.MA_PhongBan.Contains(searchString)).OrderByDescending(x => x.Ten_PB);
            }

            ViewBag.SearchString = searchString;
            return View(model.OrderByDescending(x => x.Ten_PB).ToPagedList(page, pageSize));
        }
  
        /* public ActionResult sua_PB(string id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             PHONGBAN phongban = db.PHONGBANs.SingleOrDefault(s => s.MA_PhongBan == id);
             return View(phongban);
         }*/

        public JsonResult Edit_PB(string id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            var phongBan = db.PHONGBANs.Find(id);

            return Json(phongBan, JsonRequestBehavior.AllowGet);
        }

        public ActionResult sua_PB1()
        {
            return View();
        }

        [HttpPost]
        public ActionResult sua_PB1(PHONGBAN pb)
        {
            var phongban = db.PHONGBANs.Find(pb.MA_PhongBan);

            if (phongban == null)
                return HttpNotFound();

            if (TryUpdateModel(phongban, "", new string[] { "MA_PhongBan", "Ten_PB" }))
            {
                try
                {
                    phongban.Ten_PB = pb.Ten_PB;
                    db.Entry(phongban).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "loi");
                }

            }
            var listPB = from s in db.PHONGBANs select s;
            return View("ListPB", listPB);
        }
        [HttpGet]
        public ActionResult xoa_PB(string id)
        {
            PHONGBAN phongban = db.PHONGBANs.SingleOrDefault(s => s.MA_PhongBan == id);
            if (phongban == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(phongban);
        }
        
        public ActionResult xoa_PB1(string id)
        {
            PHONGBAN phongban = db.PHONGBANs.SingleOrDefault(s => s.MA_PhongBan == id);

            //kiem tra khoa ngoai o bang nhan vien
            if(db.NHANVIENs.Any(x => x.MA_PhongBan == id))
            {
                //ViewBag.error = "Vui lòng xóa nhân viên thuộc phòng ban trước !!";
                int error = 1;
                return RedirectToAction("ListPB", new { error });
            }

            db.PHONGBANs.Remove(phongban);
            db.SaveChanges();
            return RedirectToAction("ListPB");
        }
        public ActionResult them_PBhhh()
        {
            return View();
        }
        [HttpPost, ActionName("them_PBhhh")]
        [ValidateInput(false)]
        public ActionResult them_PBhhh2(PHONGBAN pb)
        {
            if (db.PHONGBANs.Any(x => x.MA_PhongBan == pb.MA_PhongBan))
            {
                ViewBag.error = "Mã phòng ban này đã tồn tại!!!";
                return View(pb);
            }
            if (!ModelState.IsValid)
                return RedirectToAction("ListPB");
            db.PHONGBANs.Add(pb);
            db.SaveChanges();
            var listPB = from s in db.PHONGBANs select s;
            return View("ListPB", listPB);
        }

    }
}
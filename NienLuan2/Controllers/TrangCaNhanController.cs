using NienLuan2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NienLuan2.Controllers
{
    public class TrangCaNhanController : Controller
    {
        // GET: TrangCaNhan
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();

        [HttpGet]
        public ActionResult Index1()
        {
            NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];

            return View(nv);
        }

        [HttpPost]
        public ActionResult ChangeAvatar(HttpPostedFileBase file)
        {
            NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
            //string[] filename = file.FileName.Split('.');
            string newPath = Path.Combine(Server.MapPath("~/Content/images" + nv.MA_NhanVien + ".png"));
            file.SaveAs(newPath);
            NHANVIEN nv1 = db.NHANVIENs.FirstOrDefault(avt => avt.MA_NhanVien == nv.MA_NhanVien);
            nv1.Avatar = nv.MA_NhanVien + ".png";
            db.SaveChanges();
            return RedirectToAction("Index1", "TrangCaNhan");
        }

        public JsonResult ChangePassword(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var nhanvien = db.NHANVIENs.Find(id);

            return Json(nhanvien, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Change_Password(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NHANVIEN nhanvien = db.NHANVIENs.Find(id);
            return View(nhanvien);
        }

        [HttpPost, ActionName("Change_Password1")]
        public ActionResult Change_Password1(string id, string password)
        {
            NHANVIEN nv = new NHANVIEN();
            if (!ModelState.IsValid)
                return View(nv);

            if (TryUpdateModel(nv, "", new string[] { "MA_NhanVien", "MatKhau" }))
            {
                try
                {
                    NL2_QLVAEntities1 NL2_QLVAEntities = new NL2_QLVAEntities1();
                    NL2_QLVAEntities.NHANVIENs.Attach(nv);
                    NL2_QLVAEntities.Entry(nv).Property(x => x.MatKhau).IsModified = true;
                    NL2_QLVAEntities.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Lỗi");
                }

            }
            return RedirectToAction("Index1", "TrangCaNhan");
        }
    }
}
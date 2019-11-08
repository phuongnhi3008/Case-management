using NienLuan2.Helper;
using NienLuan2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
            if (nv == null)
                return RedirectToAction("DangNhap", "DangNhap");
            else
                return View(nv);
        }
       
        [HttpPost]
        public ActionResult ChangeAvatar(HttpPostedFileBase file)
        {
            NHANVIEN nv = (NHANVIEN)Session["TaiKhoan"];
            //string[] filename = file.FileName.Split('.');
            string newPath = Path.Combine(Server.MapPath("~/Content/images" + file.FileName + ".png"));
            file.SaveAs(newPath);
            NHANVIEN nv1 = db.NHANVIENs.FirstOrDefault(avt => avt.MA_NhanVien == nv.MA_NhanVien);
            nv1.Avatar = file.FileName + ".png";
            db.SaveChanges();
            Session["TaiKhoan"] = nv1;
            return RedirectToAction("Index1",  "TrangCaNhan" );
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

        [HttpPost, ActionName("Change_Password1"),AllowAnonymous]
        public ActionResult Change_Password1(string id, string password)
        {
            NHANVIEN nv = new NHANVIEN();
            if (!ModelState.IsValid)
                return View(nv);

            if (TryUpdateModel(nv, "", new string[] { "MA_NhanVien", "MatKhau" }))
            {
                try
                {
                    NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
                    var nhanVienCu = db.NHANVIENs.FirstOrDefault(nvc => nvc.MA_NhanVien == nv.MA_NhanVien);
                    nhanVienCu.MatKhau = nv.MatKhau;
                    db.SaveChanges();
 
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
using NienLuan2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NienLuan2.Controllers
{
    public class DangNhapController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: DangNhap
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string maNhanVien, string matKhau)
        {
            var nhanvien = db.NHANVIENs.SingleOrDefault(s => s.MA_NhanVien == maNhanVien && s.MatKhau == matKhau);
            using (NL2_QLVAEntities1 db = new NL2_QLVAEntities1())
                if (nhanvien != null)
                {
                    Session["MaNV"] = nhanvien.MA_NhanVien;
                    Session["TenNV"] = nhanvien.HoTen_NV;
                    Session["MatKhau"] = nhanvien.MatKhau;
                    Session["QuyenSD"] = nhanvien.MA_QNSD;

                    //gán session Tài khoản
                    Session["online"] = nhanvien;
                    Session["TaiKhoan"] = nhanvien as NHANVIEN;
                    List<MenuModel> _menus = db.SUB_MENU.Where(x => x.MA_QNSD == nhanvien.MA_QNSD).Select(x => new MenuModel
                    {
                        ID_Main = x.MAIN_MENU.ID_Main,
                        Ten_Main = x.MAIN_MENU.Ten_Main,
                        ID_SUB = x.ID_SUB,
                        Ten_SUB = x.CHUCNANG.Ten_Action_Viet,
                        Controller_SUB = x.CHUCNANG.DANHMUC_CHUCNANG.Ten_Controller_code,
                        Action_SUB = x.CHUCNANG.Ten_Action_code,
                        MA_QNSD = x.MA_QNSD,
                        Ten_QNSD = x.QUYEN_NSD.Ten_QNSD
                    }).ToList(); //Get the Menu details from entity and bind it in MenuModels list.  
                    FormsAuthentication.SetAuthCookie(nhanvien.HoTen_NV, false); // set the formauthentication cookie  
                    Session["LoginCredentials"] = nhanvien; // Bind the _logincredentials details to "LoginCredentials" session  
                    Session["MenuMaster"] = _menus; //Bind the _menus list to MenuMaster session  
                    Session["UserName"] = nhanvien.MA_NhanVien;
                    if (nhanvien.MA_QNSD == "Q01")
                    {
                        return RedirectToAction("ListNV", "NhanVien");
                    }
                    return RedirectToAction("ListHS", "HoSoVuAn");
                }
            ViewBag.message = "Sai tài khoản hoặc mật khẩu";


            return View();

        }
        public JsonResult CheckDangNhap(string maNhanVien, string matKhau)
        {
            try
            {
                var nhanvien = db.NHANVIENs.SingleOrDefault(s => s.MA_NhanVien == maNhanVien);


                if (nhanvien != null)
                {
                    if (nhanvien.MatKhau == matKhau)
                    {
                        return Json("thanhcong", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("saimatkhau", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("saitendangnhap", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult DangXuat()
        {
            Session["MaNV"] = null;
            Session["TenNV"] = null;
            Session["MatKhau"] = null;
            Session["QuyenSD"] = null;

            //gán session Tài khoản
            Session["online"] = null;
            Session["TaiKhoan"] = null;
            Session["LoginCredentials"] = null;
            Session["MenuMaster"] = null;
            Session["UserName"] = null;

            FormsAuthentication.SignOut();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            return RedirectToAction("Index", "Home");
        }

    }
}
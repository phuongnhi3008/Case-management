using NienLuan2.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NienLuan2.Controllers
{
    public class NguoiDungController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: NguoiDung
        public ActionResult MauDon(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            IEnumerable<MAUDON> model = db.MAUDONs;
            return View(model.OrderByDescending(x => x.TenMauDon).ToPagedList(page, pageSize));
        }

        public ActionResult LichXetXu(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            IEnumerable<XETXU> model = db.XETXUs;
            return View(model.OrderByDescending(x => x.Ngay_XetXu).ToPagedList(page, pageSize));
        }

        public ActionResult KetQua(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            IEnumerable<KETQUA_XX> model = db.KETQUA_XX;
            return View(model.OrderByDescending(x => x.XETXU.Ngay_XetXu).ToPagedList(page, pageSize));
        }

        [HttpGet]
        public FileResult DowloadDon(int id)
        {
            MAUDON maudon = db.MAUDONs.Where(md => md.MA_MauDon == id).FirstOrDefault();
            string path = AppDomain.CurrentDomain.BaseDirectory + "/Content/MauDon/";
            string fileName = maudon.Link_MD;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path + fileName);
            string contentType = MimeMapping.GetMimeMapping(path + fileName);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };
            string header = "attachment; filename=\"" + fileName +"\"";
            Response.ContentType = "application/ms-word";
            Response.AppendHeader("Content-Disposition", header);
            Response.TransmitFile(Server.MapPath("~/Content/MauDon/" + fileName));
            Response.End();
            MemoryStream mStream = new MemoryStream();
            mStream.Write(fileBytes, 0, fileBytes.Length);
            return File(mStream, contentType, fileName);
        }
    }
}
using NienLuan2.Helper;
using NienLuan2.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NienLuan2.Controllers
{
    public class KetQuaXetXuController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        // GET: KetQuaXetXu
        public ActionResult ListKQ(string searchString, int? error, int page = 1, int pageSize = 10)
        {
            List<HOSO_VUAN> ListVA = new List<HOSO_VUAN>();
            foreach (var item in db.XETXUs.ToList())
            {
                ListVA.Add(item.HOSO_VUAN);
                foreach (var itemKQ in db.KETQUA_XX.ToList())
                {
                    if (item.Equals(itemKQ.XETXU))
                        ListVA.Remove(item.HOSO_VUAN);
                }         
            }
            IEnumerable<HOSO_VUAN> result = ListVA.GroupBy(x => x.Ten_VuAn).Select(x => x.First());                       
            ViewBag.tva = new SelectList(result, "MA_HoSo", "Ten_VuAn");
            HOSO_VUAN hoso = result.First();
            List<XETXU> listXetXu = db.XETXUs.Where(xx => xx.MA_HoSo == hoso.MA_HoSo).ToList();
            List<XETXU> newListXetXu = listXetXu;
            foreach (var itemXX in listXetXu.ToList())
            {
                foreach (var itemKQ in db.KETQUA_XX.ToList())
                {
                    if (itemXX.Equals(itemKQ.XETXU))
                        newListXetXu.Remove(itemXX);
                }
            }


             ViewBag.lxx = new SelectList(newListXetXu.AsEnumerable(), "MA_XetXu", "Lan_XetXu");
            var ngayxetxu = listXetXu.First().Ngay_XetXu;
            ViewBag.nxx = ngayxetxu;
            var listKetQua = from s in db.KETQUA_XX select s;

            if (error == 1)
                ViewBag.Loi = 1;

            IEnumerable<KETQUA_XX> model = db.KETQUA_XX;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.XETXU.HOSO_VUAN.Ten_VuAn.Contains(searchString)).OrderByDescending(x => x.XETXU.HOSO_VUAN.Ten_VuAn);
            }

            ViewBag.SearchString = searchString;
            return View(model.OrderByDescending(x => x.XETXU.HOSO_VUAN.Ten_VuAn).ToPagedList(page, pageSize));
        }

        [HttpPost, ActionName("them_KQ")]
        public ActionResult them_KQ(KETQUA_XX kqxx, FormCollection form)
        {
            var listKetQua = from s in db.KETQUA_XX select s;
            if (db.KETQUA_XX.Any(x => x.MA_KetQuaXX == kqxx.MA_KetQuaXX))
            {

                return RedirectToAction("ListKQ", new { error = 1 });
            }
            kqxx.MA_XetXu = Convert.ToInt32(form["lxx"]);
            if (!ModelState.IsValid)
                return View(kqxx);

            kqxx.MA_KetQuaXX = UUID.GetUUID(5);

            db.KETQUA_XX.Add(kqxx);
            db.SaveChanges();


            return RedirectToAction("ListKQ");
        }

        public JsonResult Edit_KQ(string id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            var kq = db.KETQUA_XX.Find(id);

            return Json(new {
                MA_KetQuaXX = kq.MA_KetQuaXX,
                Ten_VuAn = kq.XETXU.HOSO_VUAN.Ten_VuAn,
                Lan_XetXu = kq.XETXU.Lan_XetXu,
                Ngay_XetXu = kq.XETXU.Ngay_XetXu,
                KetQua_XX = kq.KetQua_XX
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult sua_KQ1()
        {
            return View();
        }

        [HttpPost, ActionName("sua_KQ1")]
        public ActionResult sua_KQ1(KETQUA_XX kq)
        {
            var ketqua = db.KETQUA_XX.Find(kq.MA_KetQuaXX);

            if (ketqua == null)
                return HttpNotFound();

            if (TryUpdateModel(ketqua, "", new string[] { "MA_KetQuaXX", "KetQua_XX" }))
            {
                try
                {
                    ketqua.KetQua_XX = kq.KetQua_XX;
                    db.Entry(ketqua).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "loi");
                }

            }
            var listKQ = from s in db.KETQUA_XX select s;
            return RedirectToAction("ListKQ");
        }



        [HttpGet]
        public ActionResult GetSoLanXetXu(string iso3)
        {
            try
            {
                db.XETXUs.Where(x => x.MA_HoSo.Equals(iso3)).ToList();
                IEnumerable<SelectListItem> actions = db.XETXUs.AsNoTracking()
                        .OrderBy(n => n.Lan_XetXu)
                        .Where(n => n.MA_HoSo == iso3)
                        .Select(n =>
                           new SelectListItem
                           {
                               Value = n.MA_XetXu.ToString(),
                               Text = n.Lan_XetXu.ToString()
                           }).ToList();
                var result = actions.ToList();
                foreach (var lanxetxu in actions)
                {
                    foreach (var itemKQ in db.KETQUA_XX.ToList())
                    {
                        if (lanxetxu.Value.Equals(itemKQ.XETXU.MA_XetXu.ToString()))
                            result.Remove(lanxetxu);
                    }
                }
                return Json(new SelectList(result, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetNgayXetXu(string iso3)
        {

            if (iso3 != null)
            {
                int intMaXetXu = Convert.ToInt32(iso3);
                var ngayXetXu = db.XETXUs.Where(xx => xx.MA_XetXu == intMaXetXu).FirstOrDefault().Ngay_XetXu;

                return Json(ngayXetXu, JsonRequestBehavior.AllowGet);
            }

            else   {
                string str = "Có bug";
                return Json(str, JsonRequestBehavior.AllowGet);
            }


        }
    }

}

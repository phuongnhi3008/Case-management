﻿using NienLuan2.Helper;
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
        public ActionResult ListKQ(string searchString, string maHoSo, DateTime? tungay, DateTime? denngay, int? error, int page = 1, int pageSize = 10)
        {

            List<HOSO_VUAN> ListVA = new List<HOSO_VUAN>();
            foreach (var item in db.XETXUs.ToList())
            {
                ListVA.Add(item.HOSO_VUAN);
                //Xoa vu an da duoc xet xet xu khoi danh sach
                foreach (var itemKQ in db.KETQUA_XX.ToList())
                {
                    //Kiem tra vu an da duoc xet xu chua
                    if (item.Equals(itemKQ.XETXU))
                        ListVA.Remove(item.HOSO_VUAN);
                }
            }
            try
            {
                IEnumerable<HOSO_VUAN> result = ListVA.GroupBy(x => x.Ten_VuAn).Select(x => x.First());
                ViewBag.tva = new SelectList(result, "MA_HoSo", "Ten_VuAn");


                HOSO_VUAN hoso = result.First();
                List<XETXU> listXetXu = db.XETXUs.Where(xx => xx.MA_HoSo == hoso.MA_HoSo).ToList();
                List<XETXU> newListXetXu = listXetXu;
                foreach (var itemXX in listXetXu.ToList())
                {
                    //Kiem tra vu an da duoc xet xu chua(theo lan)
                    foreach (var itemKQ in db.KETQUA_XX.ToList())
                    {
                        if (itemXX.Equals(itemKQ.XETXU))
                            newListXetXu.Remove(itemXX);
                    }
                }
                ViewBag.lxx = new SelectList(newListXetXu.AsEnumerable(), "MA_XetXu", "Lan_XetXu");

                var ngayxetxu = listXetXu.First().Ngay_XetXu;
                ViewBag.nxx = ngayxetxu;

                //loc
                IEnumerable<KETQUA_XX> model = db.KETQUA_XX;
                if (tungay == null || denngay == null)
                {
                    ViewBag.tuNgay = db.KETQUA_XX.Min(hs => hs.XETXU.Ngay_XetXu).Value;
                    ViewBag.denNgay = DateTime.Now;
                }
                else
                {
                    List<KETQUA_XX> newList = new List<KETQUA_XX>();
                    foreach (var item in model)
                    {
                        if (item.XETXU.Ngay_XetXu >= tungay && item.XETXU.Ngay_XetXu <= denngay)
                            newList.Add(item);
                    }
                    model = newList.OrderByDescending(x => x.MA_XetXu).ToPagedList(page, pageSize);
                    ViewBag.tuNgay = tungay;
                    ViewBag.denNgay = denngay;
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    model = model.Where(x => x.XETXU.HOSO_VUAN.Ten_VuAn.Contains(searchString)).OrderByDescending(x => x.XETXU.HOSO_VUAN.Ten_VuAn);
                }
                
                if (!String.IsNullOrEmpty(maHoSo))
                {
                    model = model.Where(kqxx => kqxx.XETXU.HOSO_VUAN.MA_HoSo == maHoSo);
                }
       


                ViewBag.SearchString = searchString;
                return View(model.OrderByDescending(x => x.XETXU.HOSO_VUAN.Ten_VuAn).ToPagedList(page, pageSize));
            }
            catch
            {
                List<XETXU> newListXetXu = new List<XETXU>();
                XETXU xetxu = new XETXU
                {
                    MA_XetXu = 0,
                    MA_HoSo = "Không có vụ án"
                };
                HOSO_VUAN hosoGia = new HOSO_VUAN
                {
                    MA_HoSo = "00000",
                    Ten_VuAn = "Không có vụ án"
                };
                newListXetXu.Add(xetxu);
                List<HOSO_VUAN> newListHoSo = new List<HOSO_VUAN>();
                newListHoSo.Add(hosoGia);
                ViewBag.lxx = new SelectList(newListXetXu.AsEnumerable(), "MA_XetXu", "Lan_XetXu");
                ViewBag.tva = new SelectList(newListHoSo.AsEnumerable(), "MA_HoSo", "Ten_VuAn");

                IEnumerable<KETQUA_XX> model = db.KETQUA_XX;
                if (tungay == null || denngay == null)
                {
                    ViewBag.tuNgay = db.KETQUA_XX.Min(kq => kq.XETXU.Ngay_XetXu).Value;
                    ViewBag.denNgay = DateTime.Now;
                }
                else
                {
                    List<KETQUA_XX> newList = new List<KETQUA_XX>();
                    foreach (var item in model)
                    {
                        if (item.XETXU.Ngay_XetXu >= tungay && item.XETXU.Ngay_XetXu <= denngay)
                            newList.Add(item);
                    }
                    model = newList.OrderByDescending(x => x.MA_XetXu).ToPagedList(page, pageSize);
                    ViewBag.tuNgay = tungay;
                    ViewBag.denNgay = denngay;
                }
                if (!string.IsNullOrEmpty(searchString))
                {
                    model = model.Where(x => x.XETXU.HOSO_VUAN.Ten_VuAn.Contains(searchString)).OrderByDescending(x => x.XETXU.HOSO_VUAN.Ten_VuAn);
                }

                if (!String.IsNullOrEmpty(maHoSo))
                {
                    model = model.Where(kqxx => kqxx.XETXU.HOSO_VUAN.MA_HoSo == maHoSo);
                }
                ViewBag.SearchString = searchString;
                return View(model.OrderByDescending(x => x.XETXU.HOSO_VUAN.Ten_VuAn).ToPagedList(page, pageSize));
            }
        }

        public JsonResult GetTuNgay()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var tuNgay = db.KETQUA_XX.Min(xx => xx.XETXU.Ngay_XetXu).Value;
            return Json(tuNgay, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("them_KQ")]
        public ActionResult them_KQ(KETQUA_XX kqxx, FormCollection form, string troVe)
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

            KETQUA_XX newKQXX = db.KETQUA_XX.Where(newKQ => newKQ.MA_KetQuaXX == kqxx.MA_KetQuaXX).FirstOrDefault();
            XETXU newXX = db.XETXUs.Where(xx => xx.MA_XetXu == newKQXX.MA_XetXu).FirstOrDefault();
            string maHoSo = newXX.MA_HoSo;
            HOSO_VUAN hoSo = db.HOSO_VUAN.Where(hs => hs.MA_HoSo == maHoSo).FirstOrDefault();
            hoSo.MA_TrangThai = "03";
            db.SaveChanges();

            if (troVe == "troVe")
                return RedirectToAction("ListHS", "HoSoVuAn");
            return RedirectToAction("ListKQ");
        }

        public JsonResult Edit_KQ(string id)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            var kq = db.KETQUA_XX.Find(id);

            return Json(new
            {
                MA_KetQuaXX = kq.MA_KetQuaXX,
                Ten_VuAn = kq.XETXU.HOSO_VUAN.Ten_VuAn,
                Lan_XetXu = kq.XETXU.Lan_XetXu,
                Ngay_XetXu = kq.XETXU.Ngay_XetXu,
                KetQua_XX = kq.KetQua_XX1
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("sua_KQ1")]
        public ActionResult sua_KQ1(KETQUA_XX kq)
        {
            var ketqua = db.KETQUA_XX.Find(kq.MA_KetQuaXX);

            if (ketqua == null)
                return HttpNotFound();

            if (TryUpdateModel(ketqua, "", new string[] { "MA_KetQuaXX", "KetQua_XX1" }))
            {
                try
                {
                    ketqua.KetQua_XX1 = kq.KetQua_XX1;
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
        public ActionResult GetSoLanXetXu(string maHoSo)
        {
            try
            {
                db.XETXUs.Where(x => x.MA_HoSo.Equals(maHoSo)).ToList();
                IEnumerable<SelectListItem> actions = db.XETXUs.AsNoTracking()
                        .OrderBy(n => n.Lan_XetXu)
                        .Where(n => n.MA_HoSo == maHoSo)
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
        public ActionResult GetNgayXetXu(string maXetXu)
        {

            if (maXetXu != null)
            {
                if (maXetXu != "0")
                {
                    int intMaXetXu = Convert.ToInt32(maXetXu);
                    var ngayXetXu = db.XETXUs.Where(xx => xx.MA_XetXu == intMaXetXu).FirstOrDefault().Ngay_XetXu;
                    return Json(ngayXetXu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(DateTime.Now, JsonRequestBehavior.AllowGet);
                }
            
            }

            else
            {
                string str = "Có bug";
                return Json(str, JsonRequestBehavior.AllowGet);
            }


        }
    }

}

using NienLuan2.Helper;
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
    public class HoSoVuAnController : Controller
    {
        NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
        
        public ActionResult ListHS(string searchString, DateTime? tungay, DateTime? denngay, int? error, int page = 1, int pageSize = 25)
        {
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.vtnv = new SelectList(db.VAITRO_NV.OrderBy(x => x.Ten_VT), "MA_VaiTro", "Ten_VT");
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.nd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            ViewBag.bd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");

            //
            ViewBag.hs = new SelectList(db.HOSO_VUAN.OrderBy(x => x.Ten_VuAn), "MA_HoSo", "Ten_VuAn");
            ViewBag.dd = new SelectList(db.DIADIEM_XX.OrderBy(x => x.Ten_DiaDiem), "MA_DiaDiem", "Ten_DiaDiem");
            ViewBag.ks = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.tk = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.hd = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.ds = new SelectList(db.DUONGSUs, "MA_DuongSu", "Hoten_DS");
            ViewBag.cxx = new SelectList(db.CAPXETXUs.OrderBy(x => x.MA_CapXetXu), "MA_CapXetXu", "TenCapXetXu");

            //
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

         
            IEnumerable<HOSO_VUAN> result = ListVA.GroupBy(x => x.Ten_VuAn).Select(x => x.First());
            if(result.Count() > 0)
            {
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
            }
            else
            {
                HOSO_VUAN hosoGia = new HOSO_VUAN
                {
                    MA_HoSo = "00000",
                    Ten_VuAn = "Không có vụ án"
                };
                List<HOSO_VUAN> newListHoSo = new List<HOSO_VUAN>();
                newListHoSo.Add(hosoGia);
                ViewBag.tva = new SelectList(newListHoSo.AsEnumerable(), "MA_HoSo", "Ten_VuAn");
                ViewBag.lxx = new SelectList(newListHoSo.AsEnumerable(), "MA_HoSo", "Ten_VuAn");
                var ngayxetxu = DateTime.Now;
                ViewBag.nxx = ngayxetxu;
            }
           




            IEnumerable<HOSO_VUAN> model = db.HOSO_VUAN;

            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Ten_VuAn.Contains(searchString) || x.MA_HoSo.Contains(searchString)).OrderByDescending(x => x.Ten_VuAn);
            }
            ViewBag.SearchString = searchString;
            DuongSuModel duongSuModel = new DuongSuModel();
            if(tungay == null || denngay == null)
            {
                duongSuModel.listHoSoVuAn = model.OrderByDescending(x => x.MA_HoSo).ToPagedList(page, pageSize);
                if (db.HOSO_VUAN.Count() > 0)
                ViewBag.tuNgay = db.HOSO_VUAN.Min(hs => hs.NgayNhan_HS).Value;
                else
                {
                    ViewBag.tuNgay = DateTime.Now;
                }
                ViewBag.denNgay = DateTime.Now;
            }        
            else
            {
                List<HOSO_VUAN> newList = new List<HOSO_VUAN>();
                foreach(var item in model)
                {
                    if (item.NgayNhan_HS >= tungay && item.NgayNhan_HS <= denngay)
                        newList.Add(item);
                }
                duongSuModel.listHoSoVuAn = newList.OrderByDescending(x => x.MA_HoSo).ToPagedList(page, pageSize);
                ViewBag.tuNgay = tungay;
                ViewBag.denNgay = denngay;
            }

            duongSuModel.listChiTietDuongSu = db.CHITIET_DS.ToList();
            
            return View(duongSuModel);
        }
        public void themChiTietDuongSu(CHITIET_DS ctds)
        {
            db.CHITIET_DS.Add(ctds);
            db.SaveChanges();
        }

        [HttpPost, ActionName("them_HS")]
        public ActionResult them_HS(HOSO_VUAN hs, FormCollection form)
        {
            //Tao ma tu dong
            hs.MA_HoSo = UUID.GetUUID(5);

            hs.MA_LoaiVA = form["lva"].ToString();
            hs.MA_TrangThai = "01";
            hs.MA_NhanVien = form["mnv"].ToString();

            db.HOSO_VUAN.Add(hs);
            db.SaveChanges();
            List<string> selectedNguyenDonList = form["nd"].Split(',').ToList();

            for (int i = 0; i < selectedNguyenDonList.Count; i++)
            {
                CHITIET_DS nguyendon = new CHITIET_DS
                {
                    MA_DuongSu = selectedNguyenDonList[i],
                    MA_LoaiDS = "ND",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(nguyendon);
            }
            List<string> selectedBiDonList = form["bd"].Split(',').ToList();

            for (int i = 0; i < selectedBiDonList.Count; i++)
            {
                CHITIET_DS bidon = new CHITIET_DS
                {
                    MA_DuongSu = selectedBiDonList[i],
                    MA_LoaiDS = "BD",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(bidon);
            }

            var list_HS = from s in db.HOSO_VUAN select s;
            ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");
            ViewBag.nd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            ViewBag.bd = new SelectList(db.DUONGSUs.OrderBy(x => x.HoTen_DS), "MA_DuongSu", "HoTen_DS");
            ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            return RedirectToAction("ListHS");
        }

        public JsonResult Edit_HS(string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var hoso = db.HOSO_VUAN.Find(id);

            return Json(hoso, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetListNguyenDon(string maHoSo)
        {
            IEnumerable<SelectListItem> listNguyenDon = db.CHITIET_DS.AsNoTracking()
                                    .OrderBy(n => n.DUONGSU.HoTen_DS)
                                    .Where(n => n.MA_HoSo == maHoSo && n.MA_LoaiDS == "nd")
                                    .Select(n =>
                                       new SelectListItem
                                       {
                                           Value = n.MA_DuongSu,
                                           Text = n.DUONGSU.HoTen_DS
                                       }).ToList();
            return Json(new SelectList(listNguyenDon, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListBiDon(string maHoSo)
        {
            IEnumerable<SelectListItem> listBiDon = db.CHITIET_DS.AsNoTracking()
                                    .OrderBy(n => n.DUONGSU.HoTen_DS)
                                    .Where(n => n.MA_HoSo == maHoSo && n.MA_LoaiDS == "bd")
                                    .Select(n =>
                                       new SelectListItem
                                       {
                                           Value = n.MA_DuongSu,
                                           Text = n.DUONGSU.HoTen_DS
                                       }).ToList();
            return Json(new SelectList(listBiDon, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("sua_HS1")]
        //[ValidateInput(false)]
        ///[ValidateAntiForgeryToken]
        public ActionResult sua_HS1(HOSO_VUAN hs, FormCollection form, string id)
        {
            //ViewBag.lva = new SelectList(db.LOAI_VUAN.OrderBy(x => x.Ten_LoaiVA), "MA_LoaiVA", "Ten_LoaiVA");
            //ViewBag.tths = new SelectList(db.TRANGTHAI_HS.OrderBy(x => x.Ten_TT), "MA_TrangThai", "Ten_TT");

            //ViewBag.mnv = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");

            hs.MA_LoaiVA = form["lva"].ToString(); ;

            hs.MA_NhanVien = form["mnv"].ToString();
            //hs.MA_HoSo = id;

            HOSO_VUAN hoSo = db.HOSO_VUAN.Where(newHoSo => newHoSo.MA_HoSo == hs.MA_HoSo).FirstOrDefault();
            hoSo.MA_NhanVien = hs.MA_NhanVien;
            hoSo.MA_LoaiVA = hs.MA_LoaiVA;
            hoSo.Ten_VuAn = hs.Ten_VuAn;
            hoSo.NoiDung_VA = hs.NoiDung_VA;
            hoSo.Loai_HS = hs.Loai_HS;
            hoSo.NgayNhan_HS = hs.NgayNhan_HS;

                 //   db.Entry(hs).State = EntityState.Modified;
            db.SaveChanges();



            ClearNguyenDonBiDon(hs.MA_HoSo);

            List<string> selectedNguyenDonList = form["nd"].Split(',').ToList();

            for (int i = 0; i < selectedNguyenDonList.Count; i++)
            {
                CHITIET_DS nguyendon = new CHITIET_DS
                {
                    MA_DuongSu = selectedNguyenDonList[i],
                    MA_LoaiDS = "ND",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(nguyendon);
            }
            List<string> selectedBiDonList = form["bd"].Split(',').ToList();

            for (int i = 0; i < selectedBiDonList.Count; i++)
            {
                CHITIET_DS bidon = new CHITIET_DS
                {
                    MA_DuongSu = selectedBiDonList[i],
                    MA_LoaiDS = "BD",
                    MA_HoSo = hs.MA_HoSo,
                    MA_ChiTietDS = UUID.GetUUID(5)
                };
                themChiTietDuongSu(bidon);
            }
            return RedirectToAction("ListHS");
        }

        public void ClearNguyenDonBiDon(string maHoSo)
        {
            foreach (var item in db.CHITIET_DS.ToList())
            {
                if (item.MA_HoSo == maHoSo)
                {
                    db.CHITIET_DS.Remove(item);
                    db.SaveChanges();
                }
            }
        }

        public ActionResult xoa_HS(string id)
        {
            HOSO_VUAN hoso = db.HOSO_VUAN.SingleOrDefault(s => s.MA_HoSo == id);
            if (hoso == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hoso);
        }
        public JsonResult CheckXoa(string id)
        {
            try
            {
                XETXU hoso = db.XETXUs.SingleOrDefault(s => s.MA_HoSo == id);
                if (hoso == null)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }

        }
        //[HttpPost, ActionName("xoa_HS")]
        public ActionResult xoa_HS1(string id)
        {
            try
            {
                HOSO_VUAN hoso = db.HOSO_VUAN.SingleOrDefault(s => s.MA_HoSo == id);
                ClearNguyenDonBiDon(hoso.MA_HoSo);
                db.HOSO_VUAN.Remove(hoso);
                db.SaveChanges();
                return RedirectToAction("ListHS");
            }
            catch
            {
                return RedirectToAction("ListHS");
            }


        }

        public JsonResult GetThongKe_HS(string id)
        {
            using (NL2_QLVAEntities1 db = new NL2_QLVAEntities1())
            {
                var a = (from hs in db.HOSO_VUAN
                         join nv in db.NHANVIENs on hs.MA_NhanVien equals nv.MA_NhanVien
                         join lva in db.LOAI_VUAN on hs.MA_LoaiVA equals lva.MA_LoaiVA
                         join tt in db.TRANGTHAI_HS on hs.MA_TrangThai equals tt.MA_TrangThai
                         select new ModelsXetXu
                         {
                             MA_HoSo = hs.MA_HoSo,
                             Ten_VuAn = hs.Ten_VuAn,
                             Loai_HS = hs.Loai_HS,
                             NgayNhan_HS = hs.NgayNhan_HS.Value.ToString("dd/MM/yyyy"),
                             MA_TrangThai = hs.MA_TrangThai,
                             MA_NhanVien = hs.MA_NhanVien,
                             Ten_TT = tt.Ten_TT,
                             MA_LoaiVA = hs.MA_LoaiVA,
                             HoTen_NV = nv.HoTen_NV,
                         }).ToList();
                return Json(a, JsonRequestBehavior.AllowGet);
            }
        }
        public List<LOAI_VUAN> GetThongKe_HS()
        {
            NL2_QLVAEntities1 db = new NL2_QLVAEntities1();
            List<LOAI_VUAN> loaiva = db.LOAI_VUAN.OrderByDescending(x => x.MA_LoaiVA).ToList();
            return loaiva;
        }
        public ActionResult ThongKe_HS()
        {
            ViewBag.loaiva = new SelectList(GetThongKe_HS(), "MA_LoaiVA", "Ten_LoaiVA");
            ModelsXetXu xx = new ModelsXetXu();
            return View();
        }

        public JsonResult GetTuNgay()
        {
            db.Configuration.ProxyCreationEnabled = false;

            DateTime tuNgay;
            if (db.HOSO_VUAN.Count() > 0)
            {
                tuNgay = db.HOSO_VUAN.Min(hs => hs.NgayNhan_HS).Value;
            }
            else
            {
                tuNgay = DateTime.Now;
            }
                
            return Json(tuNgay, JsonRequestBehavior.AllowGet);
        }



        //

        [HttpPost, ActionName("themLXX")]
        public ActionResult themLXX(XETXU xx, FormCollection form)
        {
            ViewBag.dd = new SelectList(db.DIADIEM_XX.OrderBy(x => x.Ten_DiaDiem), "MA_DiaDiem", "Ten_DiaDiem");
            ViewBag.hs = new SelectList(db.HOSO_VUAN.OrderBy(x => x.MA_HoSo), "MA_HoSo", "Ten_VuAn");
            ViewBag.hd = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.ks = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.tk = new SelectList(db.NHANVIENs.OrderBy(x => x.HoTen_NV), "MA_NhanVien", "HoTen_NV");
            ViewBag.cxx = new SelectList(db.CAPXETXUs.OrderBy(x => x.MA_CapXetXu), "MA_CapXetXu", "TenCapXetXu");
            if (db.XETXUs.Any(x => x.MA_XetXu == xx.MA_XetXu))
            {

                return RedirectToAction("ListXX", new { error = 1 });
            }
            xx.MA_CapXetXu = form["cxx"].ToString();
            xx.MA_DiaDiem = form["dd"].ToString();
            xx.MA_HoSo = form["hs"].ToString();

            if (!ModelState.IsValid)
                return View(xx);

            db.XETXUs.Add(xx);
            db.SaveChanges();

            HOSO_VUAN hoSo = db.HOSO_VUAN.Where(hs => hs.MA_HoSo == xx.MA_HoSo).FirstOrDefault();
            hoSo.MA_TrangThai = "02";
            db.SaveChanges();
            List<string> selectedHoiDongList = form["hd"].Split(',').ToList();

            for (int i = 0; i < selectedHoiDongList.Count; i++)
            {
                CHITIET_XX hoidong = new CHITIET_XX
                {

                    MA_NhanVien = selectedHoiDongList[i],
                    MA_VaiTro = "C4",
                    MA_XetXu = xx.MA_XetXu,
                    MA_ChiTietXX = UUID.GetUUID(5)
                };
                themChiTietXetXu(hoidong);
            }
            CHITIET_XX kiemSat = new CHITIET_XX
            {

                MA_NhanVien = form["ks"].ToString(),
                MA_VaiTro = "C3",
                MA_XetXu = xx.MA_XetXu,
                MA_ChiTietXX = UUID.GetUUID(5)
            };
            themChiTietXetXu(kiemSat);
            CHITIET_XX thuky = new CHITIET_XX
            {

                MA_NhanVien = form["tk"].ToString(),
                MA_VaiTro = "C1",
                MA_XetXu = xx.MA_XetXu,
                MA_ChiTietXX = UUID.GetUUID(5)
            };
            themChiTietXetXu(thuky);
            return RedirectToAction("ListXX");
        }
        public void themChiTietXetXu(CHITIET_XX ctxx)
        {
            db.CHITIET_XX.Add(ctxx);
            db.SaveChanges();
        }

        public JsonResult GetNgayNhanHoSo(string maHoSo)
        {
            if (db.HOSO_VUAN.Count() > 0)
            {
                HOSO_VUAN hoSo = db.HOSO_VUAN.FirstOrDefault(ks => ks.MA_HoSo == maHoSo);
                return Json(hoSo.NgayNhan_HS, JsonRequestBehavior.AllowGet);
            }             
            else
            {
                var ngayNhan = DateTime.Now.ToString();
                return Json(ngayNhan, JsonRequestBehavior.AllowGet);
            }


          
        }

        public JsonResult GetLanXetXu(string maHoSo)
        {
            try
            {
                var ListLanXetXu = db.XETXUs.Where(xx => xx.MA_HoSo == maHoSo).ToList();
                var result = (ListLanXetXu.Count + 1).ToString();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
          
        }

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
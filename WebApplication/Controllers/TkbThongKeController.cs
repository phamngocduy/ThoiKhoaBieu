using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TkbThongKeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new TkbEntities())
            {
                ViewBag.GiangVien = new SelectList(db.TkbGiangViens.Select(gv => new
                {
                    gv.MaGV, HoTen = gv.HoTen + " - " + gv.MaGV
                }).ToArray(), "MaGV", "HoTen");
                return View();
            }
        }

        public JsonResult GetGiangVienHocPhan(string maGVs)
        {
            var result = new List<object>();
            using (var db = new TkbEntities())
            {
                foreach (var maGV in maGVs.Split(','))
                {
                    var list = new List<object>[8];
                    for (int i = 2; i <= 7; i++)
                        list[i] = new List<object>();
                    var thongKes = db.TkbThongKes.Where(tk => tk.MaGV.ToUpper() == maGV.ToUpper());
                    foreach (var thongKe in thongKes.GroupBy(tk => tk.Thu))
                    {
                        var offset = 1;
                        foreach (var model in thongKe.OrderBy(tk => tk.TietBatDau))
                        {
                            var hocPhan = db.TkbHocPhans.Find(model.MaHP) ?? new TkbHocPhan();
                            list[thongKe.Key].Add(new
                            {
                                Offset = model.TietBatDau - offset,
                                Length = model.SoTiet,
                                hocPhan.TenHocPhan, hocPhan.VietTat, hocPhan.NhomTo
                            });
                            offset = model.TietBatDau + model.SoTiet;
                        }
                    }
                    var giangVien = db.TkbGiangViens.Single(gv => gv.MaGV.ToUpper() == maGV.ToUpper());
                    var ho_va_ten = giangVien.HoTen.Split();
                    var hoTen = String.Empty;
                    for (int i = 0; i < ho_va_ten.Length - 1; i++)
                        hoTen = hoTen + ho_va_ten[i][0];
                    result.Add(new
                    {
                        giangVien.MaGV,
                        HoVaTen = hoTen + ho_va_ten.Last(),
                        ThoiKhoaBieu = list.Skip(2).Take(6).Select(tkb => tkb.ToArray()).ToArray()
                    });
                }
            }
            return Json(result.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public string SetGiangVienHocPhan(int pk, string value)
        {
            try
            {
                using (var scope = new TransactionScope())
                using (var db = new TkbEntities())
                {
                    var hocPhan = db.TkbHocPhans.Find(pk);
                    var maGV = value.Split('-').Last().Trim().ToUpper();
                    db.TkbThongKes.RemoveRange(db.TkbThongKes.Where(tk => tk.MaHP == pk));
                    db.SaveChanges();
                    if (!String.IsNullOrEmpty(value) && !String.IsNullOrWhiteSpace(value))
                        if (0 == db.TkbThongKes.Count(gv => gv.MaGV.ToUpper() == maGV.ToUpper() && gv.Thu == hocPhan.Thu
                            && (gv.TietBatDau <= hocPhan.TietBatDau && hocPhan.TietBatDau < gv.TietBatDau + gv.SoTiet)
                            && (hocPhan.TietBatDau <= gv.TietBatDau && gv.TietBatDau < hocPhan.TietBatDau + hocPhan.SoTiet)))
                            db.TkbThongKes.Add(new TkbThongKe
                            {
                                MaHP = pk,
                                MaGV = maGV,
                                Thu = hocPhan.Thu,
                                TietBatDau = hocPhan.TietBatDau,
                                SoTiet = hocPhan.SoTiet
                            });
                        else throw new Exception("Bị trùng giờ!");
                    db.SaveChanges();
                    scope.Complete();
                    return null;
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return e.GetBaseException().Message;
            }
        }
    }
}
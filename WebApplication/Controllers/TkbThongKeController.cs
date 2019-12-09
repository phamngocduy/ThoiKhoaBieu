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
            return View();
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
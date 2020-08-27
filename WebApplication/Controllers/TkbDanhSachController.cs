using ExcelDataReader;
using LINQtoCSV;
using System;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Transactions;
using WebApplication.Models;
using System.Collections.Generic;
using DataRow = System.Data.DataRow;

namespace WebApplication.Controllers
{
    public class TkbDanhSachController : Controller
    {
        private readonly TkbEntities db = new TkbEntities();

        // GET: TkbDanhSach
        public ActionResult Index()
        {
            return View(db.TkbDanhSaches.ToList());
        }

        // GET: TkbDanhSach/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TkbDanhSach model = db.TkbDanhSaches.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.ThongKe = db.TkbThongKes.ToList();
            ViewBag.GiangVien = db.TkbGiangViens.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            var reader = ExcelReaderFactory.CreateOpenXmlReader(Request.Files[0].InputStream);
            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
            });

            using (var scope = new TransactionScope())
            {
                foreach (DataTable table in result.Tables)
                {
                    var danhSach = db.TkbDanhSaches.FirstOrDefault(ds => ds.TenGoi == table.TableName) ?? new TkbDanhSach { TenGoi = table.TableName };
                    danhSach.NgayTao = DateTime.Now;
                    if (danhSach.id == 0)
                        db.TkbDanhSaches.Add(danhSach);
                    else
                        db.Entry(danhSach).State = EntityState.Modified;
                    db.SaveChanges();

                    var hocPhans = db.TkbHocPhans.Where(hp => hp.Tkb_id == danhSach.id).Select(hp => hp.id).ToList();

                    foreach (DataRow row in table.Rows)
                    {
                        var hocPhan = new TkbHocPhan
                        {
                            Tkb_id = danhSach.id,
                            MaHP = row["Mã HP"].ToString(),
                            TenHocPhan = row["Tên học phần"].ToString(),
                            TinChi = row["Tín chỉ"].ToInteger(),
                            NhomTo = row["Nhóm/Tổ"].ToString(),
                            Thu = row["Thứ"].ToInteger(),
                            Phong = row["Phòng"].ToString(),
                            TietBatDau = row["Tiết bắt đầu"].ToInteger(),
                            SoTiet = row["Số tiết"].ToInteger(),
                            SoSV = row["Số SV"].ToInteger(),
                            TuanBatDau = row["Tuần bắt đầu"].ToInteger(),
                            TuanKetThuc = row["Tuần kết thúc"].ToInteger(),
                            Nganh = row["Ngành"].ToString(),
                            MaKhoa = row["Mã khoa"].ToString(),
                        };
                        hocPhan.VietTat = db.TkbHocPhans.FirstOrDefault(hp => hp.MaHP == hocPhan.MaHP)?.VietTat;
                        var hocPhanCu = db.TkbHocPhans.FirstOrDefault(hp => hp.Tkb_id == danhSach.id && hp.MaHP == hocPhan.MaHP && hp.NhomTo == hocPhan.NhomTo && hp.Thu == hocPhan.Thu && hp.TietBatDau == hocPhan.TietBatDau && hp.Phong == hocPhan.Phong);
                        if (hocPhanCu != null)
                        {
                            hocPhanCu.TenHocPhan = hocPhan.TenHocPhan;
                            hocPhanCu.TinChi = hocPhan.TinChi;
                            hocPhanCu.SoTiet = hocPhan.SoTiet;
                            hocPhanCu.SoSV = hocPhan.SoSV;
                            hocPhanCu.TuanBatDau = hocPhan.TuanBatDau;
                            hocPhanCu.TuanKetThuc = hocPhan.TuanKetThuc;
                            hocPhanCu.Nganh = hocPhan.Nganh;
                            hocPhanCu.MaKhoa = hocPhan.MaKhoa;
                            db.Entry(hocPhanCu).State = EntityState.Modified;
                            hocPhans.Remove(hocPhanCu.id);
                        }
                        else
                            db.TkbHocPhans.Add(hocPhan);
                    }
                    foreach (var id in hocPhans)
                        db.TkbHocPhans.Remove(db.TkbHocPhans.Find(id));
                    db.SaveChanges();
                }
                scope.Complete();
            }
            return RedirectToAction("Index");
        }

        // GET: TkbDanhSach/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TkbDanhSach tkbDanhSach = db.TkbDanhSaches.Find(id);
            if (tkbDanhSach == null)
            {
                return HttpNotFound();
            }
            return View(tkbDanhSach);
        }

        // POST: TkbDanhSach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,NgayTao,TenGoi")] TkbDanhSach tkbDanhSach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tkbDanhSach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tkbDanhSach);
        }

        // GET: TkbDanhSach/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.TkbDanhSaches.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: TkbDanhSach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var model = db.TkbDanhSaches.Find(id);
            db.TkbHocPhans.RemoveRange(model.TkbHocPhans);
            db.TkbDanhSaches.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public FileResult Download(int id)
        {
            var file = Path.GetTempFileName();
            var model = db.TkbDanhSaches.Find(id);
            var data = from hp in model.TkbHocPhans
                join tk in db.TkbThongKes on hp.id equals tk.MaHP
                join gv in db.TkbGiangViens on tk.MaGV equals gv.MaGV
                select new
                {
                    hp.MaHP, hp.TenHocPhan, hp.TinChi, hp.NhomTo, hp.Thu, hp.Phong,
                    GiangVien = gv.HoTen, gv.MaGV,
                    hp.TietBatDau, hp.SoTiet, hp.SoSV,
                    hp.TuanBatDau, hp.TuanKetThuc, hp.Nganh, hp.MaKhoa
                };
            var csv = new CsvContext();
            csv.Write(data, file);
            return File(file, "application/vnd.ms-excel", model.TenGoi + ".csv");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

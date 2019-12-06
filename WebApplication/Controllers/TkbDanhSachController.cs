using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using WebApplication.Models;

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
            TkbDanhSach tkbDanhSach = db.TkbDanhSaches.Find(id);
            if (tkbDanhSach == null)
            {
                return HttpNotFound();
            }
            return View(tkbDanhSach);
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
            var table = result.Tables[0];

            using (var scope = new TransactionScope())
            {
                var model = new TkbDanhSach
                {
                    TenGoi = table.TableName
                };
                db.TkbDanhSaches.Add(model);
                db.SaveChanges();

                foreach (DataRow row in table.Rows)
                {
                    db.TkbHocPhans.Add(new TkbHocPhan
                    {
                        Tkb_id = model.id,
                        MaHP = row["Mã HP"].ToString(),
                        TenHocPhan = row["Tên học phần"].ToString(),
                        TinChi = byte.Parse(row["Tín chỉ"].ToString()),
                        NhomTo = row["Nhóm/Tổ"].ToString(),
                        Thu = byte.Parse(row["Thứ"].ToString()),
                        Phong = row["Phòng"].ToString(),
                        TietBatDau = byte.Parse(row["Tiết bắt đầu"].ToString()),
                        SoTiet = byte.Parse(row["Số tiết"].ToString())
                    });
                }
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
            TkbDanhSach tkbDanhSach = db.TkbDanhSaches.Find(id);
            if (tkbDanhSach == null)
            {
                return HttpNotFound();
            }
            return View(tkbDanhSach);
        }

        // POST: TkbDanhSach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TkbDanhSach tkbDanhSach = db.TkbDanhSaches.Find(id);
            db.TkbDanhSaches.Remove(tkbDanhSach);
            db.SaveChanges();
            return RedirectToAction("Index");
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

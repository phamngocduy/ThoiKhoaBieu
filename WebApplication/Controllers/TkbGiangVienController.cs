using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TkbGiangVienController : Controller
    {
        private TkbEntities db = new TkbEntities();

        // GET: TkbGiangVien
        public ActionResult Index()
        {
            return View(db.TkbGiangViens.ToList());
        }

        // GET: TkbGiangVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TkbGiangVien tkbGiangVien = db.TkbGiangViens.Find(id);
            if (tkbGiangVien == null)
            {
                return HttpNotFound();
            }
            return View(tkbGiangVien);
        }

        // GET: TkbGiangVien/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TkbGiangVien/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,MaGV,HoTen")] TkbGiangVien tkbGiangVien)
        {
            if (ModelState.IsValid)
            {
                db.TkbGiangViens.Add(tkbGiangVien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tkbGiangVien);
        }

        // GET: TkbGiangVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TkbGiangVien tkbGiangVien = db.TkbGiangViens.Find(id);
            if (tkbGiangVien == null)
            {
                return HttpNotFound();
            }
            return View(tkbGiangVien);
        }

        // POST: TkbGiangVien/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,MaGV,HoTen")] TkbGiangVien tkbGiangVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tkbGiangVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tkbGiangVien);
        }

        // GET: TkbGiangVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TkbGiangVien tkbGiangVien = db.TkbGiangViens.Find(id);
            if (tkbGiangVien == null)
            {
                return HttpNotFound();
            }
            return View(tkbGiangVien);
        }

        // POST: TkbGiangVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TkbGiangVien tkbGiangVien = db.TkbGiangViens.Find(id);
            db.TkbGiangViens.Remove(tkbGiangVien);
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

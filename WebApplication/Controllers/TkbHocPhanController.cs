using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TkbHocPhanController : Controller
    {
        private readonly TkbEntities db = new TkbEntities();

        // GET: TkbHocPhan
        public ActionResult Index()
        {
            return View(db.TkbHocPhans.ToList());
        }

        // GET: TkbHocPhan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = db.TkbHocPhans.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: TkbHocPhan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TkbHocPhan model)
        {
            db.Database.ExecuteSqlCommand("UPDATE dbo.TkbHocPhan SET VietTat = @VietTat WHERE MaHP = @MaHP",
                new SqlParameter("@VietTat", model.VietTat as object ?? DBNull.Value), new SqlParameter("@MaHP", model.MaHP));
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

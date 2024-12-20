﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMKM_C_.Data;
using UMKM_C_.IRepository.Repository;
using UMKM_C_.Models;
using UMKM_C_.Models.ViewModels;

namespace UMKM_C_.Controllers
{
    public class PemasukanController : Controller
    {
        private readonly IUnitOfWork pemasukanRepo;
        public PemasukanController(IUnitOfWork db)
        {
            pemasukanRepo = db;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Now.ToString(format: "yyyy-MM-dd");
            var pemasukan = await pemasukanRepo.Pemasukan.GetPemasukanHarian(p => p.Created_at == today);
            return View(pemasukan);
        }

        public IActionResult Tambah()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Tambah(Pemasukan_harian model)
        {
            if (ModelState.IsValid)
            {
                await pemasukanRepo.Pemasukan.AddPemasukanHarian(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pemasukan = await pemasukanRepo.Pemasukan.GetPemasukanHarian(p => p.Id == id);
            if (pemasukan == null)
            {
                return NotFound();
            }
            return View(pemasukan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Pemasukan_harian model)
        {
            if (ModelState.IsValid)
            {
                pemasukanRepo.Pemasukan.Update(model);
                await pemasukanRepo.Save();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> PemasukanBulanan(string date, int pg = 1)
        {
            var month = DateTime.Now.Month;
            IQueryable<Pemasukan_bulanan> bulanan = pemasukanRepo.Pemasukan.GetAllPemasukanBulanan();
            if (!string.IsNullOrEmpty(date))
            {
                bulanan = bulanan.Where(p => p.Pemasukan_harian.Created_at == date);
            }
            const int pageSize = 5;
            if (pg < 1) { pg = 1; }
            int recsCount = await bulanan.CountAsync();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            List<Pemasukan_bulanan> pemasukan_bulanan = await bulanan.Skip(recSkip).Take(pageSize).ToListAsync();
            var pemasukan = new PemasukanViewModel()
            {
                Pemasukan_bulanan = pemasukan_bulanan,
            };
            this.ViewBag.Pager = pager;
            return View(pemasukan);
        }

        [HttpGet]
        public async Task<IActionResult> GetPemasukanBulanan()
        {
            var month = DateTime.Now.Month;
            IQueryable<Pemasukan_bulanan> bulanan = pemasukanRepo.Pemasukan.GetAllPemasukanBulanan();
            var pemasukanBulanan = await bulanan.Where(p => p.Bulan == month).Select(p => new { p.Pemasukan_harian.Total, p.Pemasukan_harian.Created_at }).ToListAsync();
            var total = pemasukanBulanan.Sum(p => p.Total);
            return Json(new
            {
                data = pemasukanBulanan,
                totalKeseluruhan = total
            });
        }
    }
}

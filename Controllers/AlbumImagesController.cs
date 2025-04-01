using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using PicPerfect.Models;

namespace PicPerfect.Controllers
{
    public class AlbumImagessController : Controller
    {
        private readonly AppDbContext _context;

        public AlbumImagessController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AlbumImagess
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AlbumImages.Include(a => a.Album).Include(a => a.Image);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AlbumImagess/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var AlbumImages = await _context.AlbumImages
                .Include(a => a.Album)
                .Include(a => a.Image)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (AlbumImages == null)
            {
                return NotFound();
            }

            return View(AlbumImages);
        }

        // GET: AlbumImagess/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId");
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId");
            return View();
        }

        // POST: AlbumImagess/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,ImageId")] AlbumImages AlbumImages)
        {
            if (ModelState.IsValid)
            {
                _context.Add(AlbumImages);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId", AlbumImages.AlbumId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", AlbumImages.ImageId);
            return View(AlbumImages);
        }

        // GET: AlbumImagess/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var AlbumImages = await _context.AlbumImages.FindAsync(id);
            if (AlbumImages == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId", AlbumImages.AlbumId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", AlbumImages.ImageId);
            return View(AlbumImages);
        }

        // POST: AlbumImagess/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,ImageId")] AlbumImages AlbumImages)
        {
            if (id != AlbumImages.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(AlbumImages);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumImagesExists(AlbumImages.AlbumId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId", AlbumImages.AlbumId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", AlbumImages.ImageId);
            return View(AlbumImages);
        }

        // GET: AlbumImagess/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var AlbumImages = await _context.AlbumImages
                .Include(a => a.Album)
                .Include(a => a.Image)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (AlbumImages == null)
            {
                return NotFound();
            }

            return View(AlbumImages);
        }

        // POST: AlbumImagess/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var AlbumImages = await _context.AlbumImages.FindAsync(id);
            if (AlbumImages != null)
            {
                _context.AlbumImages.Remove(AlbumImages);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumImagesExists(int id)
        {
            return _context.AlbumImages.Any(e => e.AlbumId == id);
        }
    }
}

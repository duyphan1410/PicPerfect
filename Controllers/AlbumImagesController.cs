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
    public class AlbumImagesController : Controller
    {
        private readonly AppDbContext _context;

        public AlbumImagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AlbumImages
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AlbumImage.Include(a => a.Album).Include(a => a.Image);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AlbumImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albumImage = await _context.AlbumImage
                .Include(a => a.Album)
                .Include(a => a.Image)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (albumImage == null)
            {
                return NotFound();
            }

            return View(albumImage);
        }

        // GET: AlbumImages/Create
        public IActionResult Create()
        {
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId");
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId");
            return View();
        }

        // POST: AlbumImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,ImageId")] AlbumImage albumImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(albumImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId", albumImage.AlbumId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", albumImage.ImageId);
            return View(albumImage);
        }

        // GET: AlbumImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albumImage = await _context.AlbumImage.FindAsync(id);
            if (albumImage == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId", albumImage.AlbumId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", albumImage.ImageId);
            return View(albumImage);
        }

        // POST: AlbumImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,ImageId")] AlbumImage albumImage)
        {
            if (id != albumImage.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(albumImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumImageExists(albumImage.AlbumId))
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
            ViewData["AlbumId"] = new SelectList(_context.Album, "AlbumId", "AlbumId", albumImage.AlbumId);
            ViewData["ImageId"] = new SelectList(_context.Images, "ImageId", "ImageId", albumImage.ImageId);
            return View(albumImage);
        }

        // GET: AlbumImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var albumImage = await _context.AlbumImage
                .Include(a => a.Album)
                .Include(a => a.Image)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (albumImage == null)
            {
                return NotFound();
            }

            return View(albumImage);
        }

        // POST: AlbumImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var albumImage = await _context.AlbumImage.FindAsync(id);
            if (albumImage != null)
            {
                _context.AlbumImage.Remove(albumImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumImageExists(int id)
        {
            return _context.AlbumImage.Any(e => e.AlbumId == id);
        }
    }
}

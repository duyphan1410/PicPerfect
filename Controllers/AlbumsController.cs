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
    public class AlbumsController : Controller
    {
        private readonly AppDbContext _context;

        public AlbumsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Albums
        public async Task<IActionResult> Index()
        {
            string username = HttpContext.Session.GetString("username");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(username));
            if (username != null)
            {
                ViewBag.Username = username;

                // Get the user ID based on the username
                int userId = await _context.Users
                    .Where(u => u.Username == username)
                    .Select(u => u.UserId)
                    .FirstOrDefaultAsync();

                if (userId != 0)
                {
                    // Get albums created by the user
                    var userAlbums = await _context.Album
                        .Where(a => a.CreatorUserId == userId)
                        .ToListAsync();

                    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(userAlbums));
                    // Get album images for each album
                    foreach (var album in userAlbums)
                    {
                        album.AlbumImages = await _context.AlbumImages
                            .Where(ai => ai.AlbumId == album.AlbumId)
                            .Include(ai => ai.Image)
                            .ToListAsync();
                    }

                    return View(userAlbums.ToList());
                }
            }
            return RedirectToAction("Login", "Login");
        }


        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            string username = HttpContext.Session.GetString("username");
            if (username != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.UserId = user.UserId;
                    // Lấy danh sách ảnh của user
                    var userImages = _context.Images
                        .Where(i => i.UserId == user.UserId)
                        .ToList();
                    ViewBag.UserImages = userImages;
                    return View();
                }
            }
            return RedirectToAction("Login", "Login");
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,AlbumName,Description,CreationDate,CreatorUserId,NumberOfImage,CoverImage")] Album album)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(album);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi tạo album: " + ex.Message);
                }
            }
            // Nếu có lỗi, lấy lại danh sách ảnh
            var user = _context.Users.FirstOrDefault(u => u.UserId == album.CreatorUserId);
            if (user != null)
            {
                ViewBag.UserImages = _context.Images
                    .Where(i => i.UserId == user.UserId)
                    .ToList();
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,AlbumName,Description,CreationDate,CreatorUserId,NumberOfImage,CoverImage")] Album album)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.AlbumId))
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
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Album.FindAsync(id);
            if (album != null)
            {
                _context.Album.Remove(album);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Album.Any(e => e.AlbumId == id);
        }
    }
}

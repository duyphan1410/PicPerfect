using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using PicPerfect.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

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
            var username = HttpContext.Session.GetString("username");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(username));
            if (username != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                ViewBag.FullName = (user != null) ? user.Fullname : username;
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


                    // Get album images for each album
                    foreach (var album in userAlbums)
                    {
                        album.AlbumImages = await _context.AlbumImages
                            .Where(ai => ai.AlbumId == album.AlbumId)
                            .Include(ai => ai.Image)
                            .ToListAsync();
                        album.NumberOfImage = album.AlbumImages.Count;
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
                .Include(a => a.AlbumImages)
                .ThenInclude(ai => ai.Image)
                .FirstOrDefaultAsync(m => m.AlbumId == id);

            if (album == null)
            {
                return NotFound();
            }
            album.NumberOfImage = album.AlbumImages?.Count ?? 0;

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            var username = HttpContext.Session.GetString("username");
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
                    Console.WriteLine(album.ToString());
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

            var album = await _context.Album
                .Include(a => a.AlbumImages)
                    .ThenInclude(ai => ai.Image)
                .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (album == null)
            {
                return NotFound();
            }

            // Lấy danh sách ảnh của user
            var username = HttpContext.Session.GetString("username");
            if (username != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    var userImages = await _context.Images
                        .Where(i => i.UserId == user.UserId)
                        .ToListAsync();
                    ViewBag.UserImages = userImages;
                }
            }

            album.NumberOfImage = album.AlbumImages?.Count ?? 0;
            return View(album);
        }

        // POST: Albums/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,AlbumName,Description,CreationDate,CreatorUserId,NumberOfImage,CoverImage")] Album album, IFormFile? coverImageFile, string? coverImageUrl)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAlbum = await _context.Album
                        .Include(a => a.AlbumImages)
                            .ThenInclude(ai => ai.Image)
                        .FirstOrDefaultAsync(a => a.AlbumId == id);

                    if (existingAlbum == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin cơ bản
                    existingAlbum.AlbumName = album.AlbumName;
                    existingAlbum.Description = album.Description;

                    // Xử lý ảnh bìa
                    if (coverImageFile != null && coverImageFile.Length > 0)
                    {
                        // Tạo tên file duy nhất
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(coverImageFile.FileName)}";
                        var filePath = Path.Combine("wwwroot", "uploads", "covers", fileName);

                        // Đảm bảo thư mục tồn tại
                        Directory.CreateDirectory(Path.Combine("wwwroot", "uploads", "covers"));

                        // Lưu file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await coverImageFile.CopyToAsync(stream);
                        }

                        existingAlbum.CoverImage = $"/uploads/covers/{fileName}";
                    }
                    else if (!string.IsNullOrEmpty(coverImageUrl))
                    {
                        // Kiểm tra xem URL có hợp lệ không
                        if (coverImageUrl.StartsWith("/") || coverImageUrl.StartsWith("http"))
                        {
                            existingAlbum.CoverImage = coverImageUrl;
                        }
                        else
                        {
                            ModelState.AddModelError("", "URL ảnh không hợp lệ");
                            return View(album);
                        }
                    }

                    _context.Update(existingAlbum);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = album.AlbumId });
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
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật album: " + ex.Message);
                }
            }

            // Nếu có lỗi, load lại dữ liệu
            var reloadedAlbum = await _context.Album
                .Include(a => a.AlbumImages)
                    .ThenInclude(ai => ai.Image)
                .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (reloadedAlbum != null)
            {
                reloadedAlbum.NumberOfImage = reloadedAlbum.AlbumImages?.Count ?? 0;
            }

            return View(reloadedAlbum);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.AlbumImages)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }
            album.NumberOfImage = album.AlbumImages?.Count ?? 0;

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

        // GET: Albums/ManageImages/5
        public async Task<IActionResult> ManageImages(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.AlbumImages)
                .ThenInclude(ai => ai.Image)
                .FirstOrDefaultAsync(m => m.AlbumId == id);

            if (album == null)
            {
                return NotFound();
            }

            var username = HttpContext.Session.GetString("username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Lấy danh sách ảnh của user
            var userImages = await _context.Images
                .Where(i => i.UserId == user.UserId)
                .ToListAsync();

            ViewBag.UserImages = userImages;
            ViewBag.Album = album;

            return View();
        }

        // POST: Albums/AddImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int id, List<int> selectedImages)
        {
            var album = await _context.Album.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }

            if (selectedImages != null && selectedImages.Any())
            {
                foreach (var imageId in selectedImages)
                {
                    // Kiểm tra xem ảnh đã tồn tại trong album chưa
                    var existingImage = await _context.AlbumImages
                        .FirstOrDefaultAsync(ai => ai.AlbumId == id && ai.ImageId == imageId);

                    if (existingImage == null)
                    {
                        var albumImage = new AlbumImages
                        {
                            AlbumId = id,
                            ImageId = imageId
                        };
                        _context.AlbumImages.Add(albumImage);
                    }
                }

                // Cập nhật số lượng ảnh trong album
                album.NumberOfImage = await _context.AlbumImages
                    .Where(ai => ai.AlbumId == id)
                    .CountAsync();

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm ảnh vào album thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một ảnh để thêm vào album.";
            }

            return RedirectToAction(nameof(ManageImages), new { id = id });
        }

        // POST: Albums/RemoveImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(int id, int imageId)
        {
            var albumImage = await _context.AlbumImages
                .FirstOrDefaultAsync(ai => ai.AlbumId == id && ai.ImageId == imageId);

            if (albumImage != null)
            {
                _context.AlbumImages.Remove(albumImage);
                await _context.SaveChangesAsync();

                // Cập nhật số lượng ảnh trong album
                var album = await _context.Album.FindAsync(id);
                if (album != null)
                {
                    album.NumberOfImage = await _context.AlbumImages
                        .Where(ai => ai.AlbumId == id)
                        .CountAsync();
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Đã xóa ảnh khỏi album thành công!";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        private bool AlbumExists(int id)
        {
            return _context.Album.Any(e => e.AlbumId == id);
        }
    }
}

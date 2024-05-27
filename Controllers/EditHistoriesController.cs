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
    public class EditHistoriesController : Controller
    {
        private readonly AppDbContext _context;

        public EditHistoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EditHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.EditHistory.ToListAsync());
        }

        // GET: EditHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editHistory = await _context.EditHistory
                .FirstOrDefaultAsync(m => m.EditId == id);
            if (editHistory == null)
            {
                return NotFound();
            }

            return View(editHistory);
        }

        // GET: EditHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EditHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EditId,ImageId,EditDescription,EditDatetime")] EditHistory editHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(editHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(editHistory);
        }

        // GET: EditHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editHistory = await _context.EditHistory.FindAsync(id);
            if (editHistory == null)
            {
                return NotFound();
            }
            return View(editHistory);
        }

        // POST: EditHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EditId,ImageId,EditDescription,EditDatetime")] EditHistory editHistory)
        {
            if (id != editHistory.EditId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EditHistoryExists(editHistory.EditId))
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
            return View(editHistory);
        }

        // GET: EditHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editHistory = await _context.EditHistory
                .FirstOrDefaultAsync(m => m.EditId == id);
            if (editHistory == null)
            {
                return NotFound();
            }

            return View(editHistory);
        }

        // POST: EditHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var editHistory = await _context.EditHistory.FindAsync(id);
            if (editHistory != null)
            {
                _context.EditHistory.Remove(editHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EditHistoryExists(int id)
        {
            return _context.EditHistory.Any(e => e.EditId == id);
        }
    }
}

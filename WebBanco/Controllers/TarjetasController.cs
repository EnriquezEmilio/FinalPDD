using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanco.Data;
using WebBanco.Models;

namespace WebBanco.Controllers
{
    public class TarjetasController : Controller
    {
        private readonly MyContext _context;

        public TarjetasController(MyContext context)
        {
            _context = context;
        }

        // GET: Tarjetas
        public async Task<IActionResult> Index()
        {
            var myContext = _context.tarjetas.Include(t => t.user);
            return View(await myContext.ToListAsync());
        }

        // GET: Tarjetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tarjeta == null)
            {
                return NotFound();
            }

            return View(tarjeta);
        }

        // GET: Tarjetas/Create
        public IActionResult Create()
        {
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido");
            return View();
        }

        // POST: Tarjetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,numero,codigoV,limite,consumos,num_usr")] Tarjeta tarjeta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarjeta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", tarjeta.num_usr);
            return View(tarjeta);
        }

        // GET: Tarjetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas.FindAsync(id);
            if (tarjeta == null)
            {
                return NotFound();
            }
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", tarjeta.num_usr);
            return View(tarjeta);
        }

        // POST: Tarjetas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,numero,codigoV,limite,consumos,num_usr")] Tarjeta tarjeta)
        {
            if (id != tarjeta.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarjeta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarjetaExists(tarjeta.id))
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
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", tarjeta.num_usr);
            return View(tarjeta);
        }

        // GET: Tarjetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjeta = await _context.tarjetas
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.id == id);
            if (tarjeta == null)
            {
                return NotFound();
            }

            return View(tarjeta);
        }

        // POST: Tarjetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tarjetas == null)
            {
                return Problem("Entity set 'MyContext.tarjetas'  is null.");
            }
            var tarjeta = await _context.tarjetas.FindAsync(id);
            if (tarjeta != null)
            {
                _context.tarjetas.Remove(tarjeta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarjetaExists(int id)
        {
            return _context.tarjetas.Any(e => e.id == id);
        }
    }
}

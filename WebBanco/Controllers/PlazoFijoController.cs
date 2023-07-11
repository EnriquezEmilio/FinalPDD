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
    public class PlazoFijoController : Controller
    {
        private readonly MyContext _context;
        private Usuario? uLogeado;


        public PlazoFijoController(MyContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            uLogeado = _context.usuarios.Where(u => u.num_usr == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
                  
        }

        // GET: PlazoFijo
        public async Task<IActionResult> Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(await _context.plazoFijos.ToListAsync());
        }

        // GET: PlazoFijo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.plazoFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazoFijos
                .Include(p => p.user)
                .FirstOrDefaultAsync(m => m.id == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // GET: PlazoFijo/Create
        public IActionResult Create()
        {
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido");
            return View();
        }

        // POST: PlazoFijo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,monto,fechaIni,fechaFin,tasa,pagado,num_usr")] PlazoFijo plazoFijo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plazoFijo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", plazoFijo.num_usr);
            return View(plazoFijo);
        }

        // GET: PlazoFijo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.plazoFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazoFijos.FindAsync(id);
            if (plazoFijo == null)
            {
                return NotFound();
            }
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", plazoFijo.num_usr);
            return View(plazoFijo);
        }

        // POST: PlazoFijo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,monto,fechaIni,fechaFin,tasa,pagado,num_usr")] PlazoFijo plazoFijo)
        {
            if (id != plazoFijo.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plazoFijo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlazoFijoExists(plazoFijo.id))
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
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", plazoFijo.num_usr);
            return View(plazoFijo);
        }

        // GET: PlazoFijo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.plazoFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazoFijos
                .Include(p => p.user)
                .FirstOrDefaultAsync(m => m.id == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }

        // POST: PlazoFijo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.plazoFijos == null)
            {
                return Problem("Entity set 'MyContext.plazoFijos'  is null.");
            }
            var plazoFijo = await _context.plazoFijos.FindAsync(id);
            if (plazoFijo != null)
            {
                _context.plazoFijos.Remove(plazoFijo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlazoFijoExists(int id)
        {
            return _context.plazoFijos.Any(e => e.id == id);
        }
    }
}

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
    public class MovimientoController : Controller
    {
        private readonly MyContext _context;
        private Usuario? uLogeado;


        public MovimientoController(MyContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _context.usuarios
                  .Include(u => u.misTarjetas)
                  .Include(u => u.Caja)
                  .Include(u => u.misPlazosFijos)
                  .Include(u => u.misPagos)
                  .Load();
            _context.cajas
                .Include(c => c.misMovimientos)
                .Include(c => c.UserCaja)
                .Load();
            _context.plazoFijos.Load();
            uLogeado = _context.usuarios.Where(u => u.num_usr == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();

        }

        // GET: Movimiento
        public async Task<IActionResult> Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(await _context.movimientos.ToListAsync());
        }

        // GET: Movimiento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos
                .Include(m => m.caja)
                .FirstOrDefaultAsync(m => m.idm == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // GET: Movimiento/Create
        public IActionResult Create()
        {
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu");
            return View();
        }

        // POST: Movimiento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idm,detalle,monto,fecha,id")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu", movimiento.id);
            return View(movimiento);
        }

        // GET: Movimiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu", movimiento.id);
            return View(movimiento);
        }

        // POST: Movimiento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idm,detalle,monto,fecha,id")] Movimiento movimiento)
        {
            if (id != movimiento.idm)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimientoExists(movimiento.idm))
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
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu", movimiento.id);
            return View(movimiento);
        }

        // GET: Movimiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos
                .Include(m => m.caja)
                .FirstOrDefaultAsync(m => m.idm == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // POST: Movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.movimientos == null)
            {
                return Problem("Entity set 'MyContext.movimientos'  is null.");
            }
            var movimiento = await _context.movimientos.FindAsync(id);
            if (movimiento != null)
            {
                _context.movimientos.Remove(movimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovimientoExists(int id)
        {
            return _context.movimientos.Any(e => e.idm == id);
        }
    }
}

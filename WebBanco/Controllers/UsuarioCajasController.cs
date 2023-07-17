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
    public class UsuarioCajasController : Controller
    {
        private readonly MyContext _context;
        private Usuario? uLogeado;


        public UsuarioCajasController(MyContext context, IHttpContextAccessor httpContextAccessor)
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

        // GET: UsuarioCajas
        public async Task<IActionResult> Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View(await _context.UsuarioCaja.ToListAsync());
        }

        // GET: UsuarioCajas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioCaja == null)
            {
                return NotFound();
            }

            var usuarioCaja = await _context.UsuarioCaja
                .Include(u => u.caja)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m.num_usr == id);
            if (usuarioCaja == null)
            {
                return NotFound();
            }

            return View(usuarioCaja);
        }

        // GET: UsuarioCajas/Create
        public IActionResult Create()
        {
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu");
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido");
            return View();
        }

        // POST: UsuarioCajas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,num_usr")] UsuarioCaja usuarioCaja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioCaja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu", usuarioCaja.id);
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", usuarioCaja.num_usr);
            return View(usuarioCaja);
        }

        // GET: UsuarioCajas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioCaja == null)
            {
                return NotFound();
            }

            var usuarioCaja = await _context.UsuarioCaja.FindAsync(id);
            if (usuarioCaja == null)
            {
                return NotFound();
            }
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu", usuarioCaja.id);
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", usuarioCaja.num_usr);
            return View(usuarioCaja);
        }

        // POST: UsuarioCajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,num_usr")] UsuarioCaja usuarioCaja)
        {
            if (id != usuarioCaja.num_usr)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioCaja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioCajaExists(usuarioCaja.num_usr))
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
            ViewData["id"] = new SelectList(_context.cajas, "id", "Cbu", usuarioCaja.id);
            ViewData["num_usr"] = new SelectList(_context.usuarios, "num_usr", "apellido", usuarioCaja.num_usr);
            return View(usuarioCaja);
        }

        // GET: UsuarioCajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioCaja == null)
            {
                return NotFound();
            }

            var usuarioCaja = await _context.UsuarioCaja
                .Include(u => u.caja)
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m.num_usr == id);
            if (usuarioCaja == null)
            {
                return NotFound();
            }

            return View(usuarioCaja);
        }

        // POST: UsuarioCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioCaja == null)
            {
                return Problem("Entity set 'MyContext.UsuarioCaja'  is null.");
            }
            var usuarioCaja = await _context.UsuarioCaja.FindAsync(id);
            if (usuarioCaja != null)
            {
                _context.UsuarioCaja.Remove(usuarioCaja);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioCajaExists(int id)
        {
            return _context.UsuarioCaja.Any(e => e.num_usr == id);
        }
    }
}

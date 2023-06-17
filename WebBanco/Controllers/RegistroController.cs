using WebBanco.Data;
using WebBanco.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    public class RegistroController : Controller
    {
        private readonly MyContext _context;
        private Usuario? uLogeado;

        public RegistroController(MyContext context, IHttpContextAccessor httpContextAccessor)
        { //Relaciones del context
            _context = context;
            _context.usuarios
                    .Include(u => u.misTarjetas)
                    .Include(u => u.Caja)
                    .Include(u => u.misPlazosFijos)
                    .Include(u => u.misPagos)
                    .Load();
            _context.cajas
                .Include(c => c.misMovimientos)
                .Load();
            _context.tarjetas.Load();
            _context.pagos.Load();
            _context.movimientos.Load();
            _context.plazoFijos.Load();
            uLogeado = _context.usuarios.Where(u => u.num_usr == httpContextAccessor.HttpContext.Session.GetInt32("Id")).FirstOrDefault();
        }

        // GET: RegistroController
        public ActionResult Index()
        {
            if (uLogeado != null)
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.logeado = "no";
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind("id,dni,nombre,apellido,mail,password")] Usuario usuario)
        {
            if (uLogeado != null)
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.logeado = "no";
            if (_context.usuarios.Any(us => us.dni == usuario.dni))
            {
                ViewBag.error = 0;
                return View();
            }
            if (ModelState.IsValid)
            {
                usuario.bloqueado = false;
                usuario.esADM = false;
                usuario.intentosFallidos = 0;
                _context.usuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index", "Login");
            }
            return View();

        }
    }

}

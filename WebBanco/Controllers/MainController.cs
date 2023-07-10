
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanco.Data;
using WebBanco.Models;

namespace Final.Controllers
{
    public class MainController : Controller
    {
        private readonly MyContext _context;
        private Usuario? uLogeado;

        public MainController(MyContext context, IHttpContextAccessor httpContextAccessor)
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
                .Load();
            _context.tarjetas.Load();
            _context.pagos.Load();
            _context.movimientos.Load();
            _context.plazoFijos.Load();
            uLogeado = _context.usuarios.Where(u => u.num_usr == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();
        }
        // GET: MainController
        public ActionResult Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (uLogeado.esADM)
            {
                ViewBag.Admin = true;
            }
            return View();
        }
    }
}
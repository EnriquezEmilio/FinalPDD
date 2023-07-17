
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
                 .Include(c => c.UserCaja)
                .Load();
            _context.tarjetas.Load();
            _context.pagos.Load();
            _context.movimientos.Load();
            _context.plazoFijos.Load();
            uLogeado = _context.usuarios.FirstOrDefault(u => u.num_usr == httpContextAccessor.HttpContext.Session.GetInt32("UserId"));
        }
        // GET: MainController

        public ActionResult Index()
        {
            if (uLogeado == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Obtén el número de usuario del usuario autenticado
            int userNum = uLogeado.num_usr;

            var usuario = _context.usuarios
                .Include(u => u.Caja)
                .Include(u => u.misTarjetas)
                .Include(u => u.misPagos)
                .Include(u => u.misPlazosFijos)
                .FirstOrDefault(u => u.num_usr == userNum);

            if (usuario == null)
            {
                // Maneja el caso en el que no se encuentre el usuario autenticado
                return RedirectToAction("Index", "Login");
            }

            if (usuario.esADM)
            {
                ViewBag.Admin = true;
            }

            return View(usuario);
        }


    }
}

using WebBanco.Data;
using WebBanco.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;


namespace WebBanco.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyContext _context;
        private Usuario? uLogeado;

        public LoginController(MyContext contexto, IHttpContextAccessor httpContextAccessor)
        {
            _context = contexto;
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
            uLogeado = _context.usuarios.Where(u => u.num_usr == httpContextAccessor.HttpContext.Session.GetInt32("UserId")).FirstOrDefault();

        }
        public IActionResult Index()
        {
            if (uLogeado != null)
            {
                return RedirectToAction("Index", "Main");
            }
            ViewBag.logeado = "no";
            return View();
        }
        [HttpPost]
        public IActionResult Index(int Dni, string password)
        {
            try
            {
                var usuario = _context.usuarios.Where(u => u.dni == Dni).FirstOrDefault();
                if (usuario == null)
                {
                    ViewBag.errorLogin = 1;
                    return View();
                }
                if (usuario.password != password)
                {
                    if (usuario.intentosFallidos == 3)
                    {
                        usuario.bloqueado = true;
                        _context.Update(usuario);
                        _context.SaveChanges();
                        ViewBag.errorLogin = 3;
                    }
                    else
                    {
                        ViewBag.errorLogin = 2;
                        usuario.intentosFallidos++;
                        _context.Update(usuario);
                        _context.SaveChanges();
                    }
                    return View();
                }
                if (usuario.bloqueado)
                {
                    ViewBag.errorLogin = 4;
                    return View();
                }

                uLogeado = usuario;

                HttpContext.Session.SetInt32("UserId", usuario.num_usr);

                return RedirectToAction("Index", "Main");
            }
            catch
            {

                return View();
            }
        }
        public IActionResult Logout()
        {
            uLogeado = null;
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


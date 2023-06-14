using Microsoft.AspNetCore.Mvc;
using WebBanco.Data;
using WebBanco.Models;

namespace WebBanco.Controllers
{
    public class AcessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Enter(string user, string password)
        {
           
            try{

                return Content("1");
            }
            catch (Exception ex)
            {
                return Content("Ocurrio un error" + ex.Message); 
            }
        }
    }
}

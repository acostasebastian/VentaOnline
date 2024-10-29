using Microsoft.AspNetCore.Mvc;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdministradorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

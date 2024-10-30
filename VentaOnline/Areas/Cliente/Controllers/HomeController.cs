using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;
using VentaOnline.Models.ViewModels;

namespace VentaOnline.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IContenedorTrabajo _contenedorTrabajo;

        public HomeController(ILogger<HomeController> logger, IContenedorTrabajo contenedorTrabajo)
        {
            _logger = logger;
            _contenedorTrabajo = contenedorTrabajo;
        }

        public async Task<IActionResult> Index()
        {
            HomeViewModel homeVM = new HomeViewModel()
            {
              
                ListaProductos = _contenedorTrabajo.Producto.GetAll(filter: e => e.Estado == true),
                ListaCategorias = _contenedorTrabajo.Categoria.GetAll(),
                ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetAll(),
            };

                       
            return View(homeVM);
        }

        [HttpGet]        
        public async Task<IActionResult> Shop(Dictionary<string, string>? parms)
        {
            HomeViewModel homeVM = new HomeViewModel();

            homeVM.ListaCategorias = _contenedorTrabajo.Categoria.GetAll();
            homeVM.ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetAll();


            if (parms.Count == 1 )
            {
                homeVM.ListaProductos = _contenedorTrabajo.Producto.GetAll(filter: e => e.Estado == true && e.CategoriaId.ToString() == parms["categoriaId"]);
            }

            else if (parms.Count > 1)
            {
                homeVM.ListaProductos = _contenedorTrabajo.Producto.GetAll(filter: e => e.Estado == true && e.CategoriaId.ToString() == parms["categoriaId"] && e.SubCategoriaId.ToString() == parms["subCategoriaId"]);
            }

            else
            {
                homeVM.ListaProductos = _contenedorTrabajo.Producto.GetAll(filter: e => e.Estado == true);
               
            }

            return View(homeVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HomeViewModel homeVM = new HomeViewModel()
            {

                ListaProductos = _contenedorTrabajo.Producto.GetAll(filter: e => e.Estado == true),
                ListaCategorias = _contenedorTrabajo.Categoria.GetAll(),
                ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetAll(),
            };


            if (id != null)
            {
                Producto producto = _contenedorTrabajo.Producto.GetFirstOrDefault(filter: e => e.Id == id, includeProperties: "Categoria,SubCategoria,Marca,Tamanio");

                if (producto == null)
                {
                    return NotFound();

                }
                homeVM = ToViewModel(producto, homeVM);
            }

            return View(homeVM);

        }

        public async Task<IActionResult> Cart()
        {
            HomeViewModel homeVM = new HomeViewModel()
            {

                ListaProductos = _contenedorTrabajo.Producto.GetAll(filter: e => e.Estado == true),
                ListaCategorias = _contenedorTrabajo.Categoria.GetAll(),
                ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetAll(),
            };


            return View(homeVM);

        }

        private HomeViewModel ToViewModel(Producto producto, HomeViewModel homeVM)
        {
           
               homeVM.Id = producto.Id;
               homeVM.CategoriaId = producto.CategoriaId;
               homeVM.SubCategoriaId = producto.SubCategoriaId;
               homeVM.MarcaId = producto.MarcaId;
               homeVM.TamanioId = producto.TamanioId;
               homeVM.Nombre = producto.Nombre;
               homeVM.Observaciones = producto.Observaciones;
               homeVM.Stock = producto.Stock;
               homeVM.Precio = producto.Precio;          
               homeVM.UrlImagen = producto.UrlImagen;
               homeVM.Estado = producto.Estado;
               homeVM.Categoria = producto.Categoria;
               homeVM.SubCategoria = producto.SubCategoria;
               homeVM.Marca = producto.Marca;
               homeVM.Tamanio = producto.Tamanio;

            return homeVM;
           
        }

        [HttpPost]
        public JsonResult InsertarCarrito(Carrito oCarrito)
        {
            // oCarrito.oUsuario = new Usuario() { IdUsuario = oUsuario.IdUsuario };
            int _respuesta = 0;
            //_respuesta = CarritoLogica.Instancia.Registrar(oCarrito);
            //return Json(new { respuesta = _respuesta }, JsonRequestBehavior.AllowGet);
            return Json(new { respuesta = _respuesta });
        }


      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

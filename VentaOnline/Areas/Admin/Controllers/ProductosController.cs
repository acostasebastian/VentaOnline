using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SqlServer.Server;
using NuGet.Packaging.Signing;
using System.Globalization;
using System.Security.Claims;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;
using VentaOnline.Models.ViewModels;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductosController : Controller
    {
        #region variables string

        //datatable - paginacion, ordenamiento y busquda

        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColum = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        //logger
        string informacion = "";

        ClaimsIdentity claimsIdentity;
        Claim usuarioActual;

        string emailUsuarioActual = "";

        //imagen

        string nombreArchivo = "";
        string subidas = "";
        string extension = "";

        string rutaImagen = "";
        string rutaImagenAntigua = "";


        #endregion

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ILogger<Producto> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment; // para acceder a las carpetas del proyecto, para guardar/editar la imagen      

        public ProductosController(IContenedorTrabajo contenedorTrabajo, ILogger<Producto> logger, ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _logger = logger;
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductoViewModel productoVM = new ProductoViewModel()
            {
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias(),
                ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas(),
                ListaTamanios = _contenedorTrabajo.Tamanio.GetListaTamanios(),     
                
                MarcaId = 1
            };

            return View(productoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel productoVM)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        claimsIdentity = (ClaimsIdentity)this.User.Identity;
                        usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                        emailUsuarioActual = usuarioActual.Subject.Name;

                        informacion = "Usuario: " + emailUsuarioActual;

                        _logger.LogInformation("CREACIÓN DE PRODUCTO \r\n Usuario registrado para el guardado: {Time} - {@informacion}", DateTime.Now, informacion);

                        Producto producto = ToProducto(productoVM);


                        #region Imagen
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivo = HttpContext.Request.Form.Files;

                        if (producto.Id == 0 && archivo.Count() > 0)
                        {
                            //Nuevo Producto
                            nombreArchivo = Guid.NewGuid().ToString();   //como es nuevo el producto, le pongo un Guid como nombre
                            subidas = Path.Combine(rutaPrincipal, @"imagenes\productos"); //accederá a la carpeta en wwwroot
                            extension = Path.GetExtension(archivo[0].FileName);


                            //guardo la ruta en la base de datos
                            producto.UrlImagen = @"\imagenes\productos\" + nombreArchivo + extension;


                            informacion = "URL Imagen: " + producto.UrlImagen;
                            _logger.LogInformation("CREACIÓN DE PRODUCTO \r\n Guardado de imagen {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        #endregion

                        producto.Estado = true;
                        var usCulture = CultureInfo.CreateSpecificCulture("es-ES");

                        //Logica para guardar en BD
                        _contenedorTrabajo.Producto.Add(producto);

                            informacion = "Agregado a BD. ";
                            _logger.LogInformation("CREACIÓN DE PRODUCTO \r\n Producto.Add {Time} - {@informacion}", DateTime.Now, informacion);

                            _contenedorTrabajo.Save();

                            if (archivo.Count() > 0) 
                            {

                                //recien guardo la imagen en la carpeta cuando se que todo fue bien
                                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                                {
                                    archivo[0].CopyTo(fileStreams);

                                    _logger.LogInformation("CREACIÓN DE PRODUCTO \r\n Guardado de Imagen en carpeta {Time}", DateTime.Now);
                                }
                            }

                            transaction.Commit();
                            _logger.LogInformation("CREACIÓN DE PRODUCTO \r\n Commit Exitoso {Time}", DateTime.Now);

                            return RedirectToAction(nameof(Index));

                        
                       
                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        if (ex.InnerException != null &&
                           ex.InnerException.Message != null)
                        {

                            if (ex.InnerException.Message.Contains("IX_Productos_CategoriaId_SubCategoriaId_MarcaId_Nombre"))
                            {
                                ModelState.AddModelError(string.Empty, "Ya existe un producto con el nombre ingresado.");
                            }

                            else if (ex.InnerException.Message.Contains("not in a correct format."))
                            {
                                ModelState.AddModelError(productoVM.sPrecio, "Error: El valor del precio debe ser númerico");
                            }

                            else
                            {
                                ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                            }

                            informacion = ex.InnerException.Message;
                            _logger.LogWarning("CREACIÓN DE PRODUCTO \r\n Error al querer guardar en Productos - InnerException {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        else
                        {
                            if (ex.Message.Contains("not in a correct format."))
                            {
                                ModelState.AddModelError(productoVM.sPrecio, "Error: El valor del precio debe ser númerico");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + ex.Message + "\r\n Contacte con el administrador si es necesario.");
                            }                            

                            informacion = ex.Message;
                            _logger.LogWarning("CREACIÓN DE PRODUCTO \r\n Error al querer guardar en Productos {Time} - {@informacion}", DateTime.Now, informacion);
                        }

                        var archivo = HttpContext.Request.Form.Files;


                        //para no perder los datos de las listas
                        productoVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                        productoVM.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                        productoVM.ListaTamanios = _contenedorTrabajo.Tamanio.GetListaTamanios();                        
                        productoVM.ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetListaSubCategoriasPorCategoria(productoVM.CategoriaId);


                        return View(productoVM);
                    }

                }

            }

            //para no perder los datos de las listas
            productoVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            productoVM.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
            productoVM.ListaTamanios = _contenedorTrabajo.Tamanio.GetListaTamanios();
            productoVM.ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetListaSubCategoriasPorCategoria(productoVM.CategoriaId);


            return View(productoVM);
        }

     

        [HttpGet]        
        public async Task<IActionResult> Edit(int? id)
        {
            ProductoViewModel productoVM = new ProductoViewModel();
         

            if (id != null)
            {
                Producto producto = _contenedorTrabajo.Producto.Get(id.GetValueOrDefault());

                if (producto == null)
                {
                    return NotFound();

                }

                productoVM = ToViewModel(producto);

                productoVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                productoVM.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                productoVM.ListaTamanios = _contenedorTrabajo.Tamanio.GetListaTamanios();                              
                productoVM.ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetListaSubCategoriasPorCategoria(producto.CategoriaId);                

            }

            return View(productoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductoViewModel productoVM)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        claimsIdentity = (ClaimsIdentity)this.User.Identity;
                        usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                        emailUsuarioActual = usuarioActual.Subject.Name;                        

                        informacion = "Usuario: " + emailUsuarioActual + " - Id: " + productoVM.Id;

                        _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Comienzo de la edición: {Time} - {@informacion}", DateTime.Now, informacion);

                        Producto producto = ToProducto(productoVM);

                        #region Imagen
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivos = HttpContext.Request.Form.Files;

                        var rutaImagenDesdeBd = _contenedorTrabajo.Producto.Get(productoVM.Id).UrlImagen;
                        

                        if (archivos.Count() > 0)
                        {
                            //Nuevo imagen para el artículo
                            nombreArchivo = Guid.NewGuid().ToString();
                            subidas = Path.Combine(rutaPrincipal, @"imagenes\productos");
                            extension = Path.GetExtension(archivos[0].FileName);


                            if (rutaImagenDesdeBd != null)
                            {
                                //guardamos la ruta de la imagen de este archivo, para que si luego todo va bien, antes del commit eliminemos la imagen
                                // la guardo porque sino se pierde en el UPDATE y no vuelve a atras con el rollback
                                rutaImagenAntigua = Path.Combine(rutaPrincipal, rutaImagenDesdeBd.TrimStart('\\'));

                                informacion = "URL Imagen Antigua: " + rutaImagenAntigua;
                                _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);
                            }

                            producto.UrlImagen = @"\imagenes\productos\" + nombreArchivo + extension;

                            informacion = "URL Imagen Nueva: " + producto.UrlImagen;
                            _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);


                        }
                        else
                        {
                            //Aquí sería cuando la imagen ya existe y se conserva
                            producto.UrlImagen = rutaImagenDesdeBd;

                            informacion = "Se conserva la misma imagen.";
                            _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        #endregion



                        _contenedorTrabajo.Producto.Update(producto);

                        informacion = "Movimiento editado.";

                        _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Producto.Update: {Time} - {@informacion}", DateTime.Now, informacion);

                        _contenedorTrabajo.Save();

                        //elimino la imagen vieja y agrego la nueva cuando se que todo en la transaccion esta bien

                        if (rutaImagenAntigua != "")
                        {

                            if (System.IO.File.Exists(rutaImagenAntigua))
                            {
                                System.IO.File.Delete(rutaImagenAntigua);
                            }
                        }

                        if (archivos.Count() > 0)
                        {
                            //Nuevamente subimos el archivo
                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivos[0].CopyTo(fileStreams);

                                _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Guardado de Imagen en carpeta {Time}", DateTime.Now);
                            }
                        }



                        transaction.Commit();

                        _logger.LogInformation("EDICIÓN DE PRODUCTO \r\n Commit Exitoso {Time}", DateTime.Now);

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        if (ex.InnerException != null &&
                            ex.InnerException.Message != null)
                        {

                            if (ex.InnerException.Message.Contains("IX_Productos_CategoriaId_SubCategoriaId_MarcaId_Nombre"))
                            {
                                ModelState.AddModelError(string.Empty, "Este Producto ya existe");
                            }

                            else if (ex.InnerException.Message.Contains("not in a correct format.") )
                            {
                                ModelState.AddModelError(productoVM.sPrecio, "Error: El valor del precio debe ser númerico");
                            }

                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + ex.InnerException.Message + "\r\n Contacte con el administrador si es necesario.");
                            }                           

                            informacion = ex.InnerException.Message;
                            _logger.LogWarning("EDICIÓN DE PRODUCTO \r\n Error al querer editar en Productos - InnerException {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        else
                        {
                            if (ex.Message.Contains("not in a correct format."))
                            {
                                ModelState.AddModelError(productoVM.sPrecio, "Error: El valor del precio debe ser númerico");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + ex.Message + "\r\n Contacte con el administrador si es necesario.");
                            }
                          
                            informacion = ex.Message;
                            _logger.LogWarning("EDICIÓN DE PRODUCTO \r\n Error al querer editar en Productos {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                    }
                    productoVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                    productoVM.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
                    productoVM.ListaTamanios = _contenedorTrabajo.Tamanio.GetListaTamanios();                    
                    productoVM.ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetListaSubCategoriasPorCategoria(productoVM.CategoriaId);
                    return View(productoVM);

                }

            }

            productoVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            productoVM.ListaMarcas = _contenedorTrabajo.Marca.GetListaMarcas();
            productoVM.ListaTamanios = _contenedorTrabajo.Tamanio.GetListaTamanios();            
            productoVM.ListaSubCategorias = _contenedorTrabajo.SubCategoria.GetListaSubCategoriasPorCategoria(productoVM.CategoriaId);
            return View(productoVM);

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductoViewModel productoViewModel = new ProductoViewModel();      


            if (id != null)
            {                
                Producto producto = _contenedorTrabajo.Producto.GetFirstOrDefault(filter: e => e.Id == id, includeProperties: "Categoria,SubCategoria,Marca,Tamanio");

                if (producto == null)
                {
                    return NotFound();

                }

                productoViewModel = ToViewModel(producto);
              

            }

            return View(productoViewModel);

        }


        private Producto ToProducto(ProductoViewModel productoVM)
        {                  
            return new Producto
            {
                Id = productoVM.Id,
                CategoriaId = productoVM.CategoriaId,
                SubCategoriaId = productoVM.SubCategoriaId,
                MarcaId = productoVM.MarcaId,   
                TamanioId = productoVM.TamanioId,
                Nombre = productoVM.Nombre,
                Observaciones = productoVM.Observaciones,
                Stock = productoVM.Stock,
                UrlImagen = productoVM.UrlImagen,
                Estado = productoVM.Estado,
                Precio = productoVM.sPrecio != null ? decimal.Parse(productoVM.sPrecio.Replace(".", ",")) : 0,

                Categoria = productoVM.Categoria,
                SubCategoria = productoVM.SubCategoria,
                Marca = productoVM.Marca,
                Tamanio = productoVM.Tamanio,
            };
        }

        private ProductoViewModel ToViewModel(Producto producto)
        {
            return new ProductoViewModel
            {
                Id = producto.Id,
                CategoriaId = producto.CategoriaId,                
                SubCategoriaId = producto.SubCategoriaId,                
                MarcaId = producto.MarcaId,                
                TamanioId = producto.TamanioId,                
                Nombre = producto.Nombre,
                Observaciones = producto.Observaciones,
                Stock = producto.Stock,
                // Precio = producto.Precio,
                sPrecio = producto.Precio.ToString(),
                UrlImagen = producto.UrlImagen,
                Estado = producto.Estado,
                Categoria = producto.Categoria,
                SubCategoria = producto.SubCategoria, 
                Marca = producto.Marca,
                Tamanio = producto.Tamanio,
                

            };
        }

        [HttpGet]
        public JsonResult ObtenerSubCategorias(int categoriaId) 
        {
            var subcategorias = _contenedorTrabajo.SubCategoria.GetAll(filter: c => c.CategoriaId == categoriaId);

            return Json(subcategorias);
        }


        #region Llamadas a la API

        [HttpPost]
        public IActionResult GetAll()
        {

            //logistica datatable           
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColum = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault(); //column por la que esta ordenado
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault(); //asc/desc
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;

            IEnumerable<Producto>? listaProductos;


            if (searchValue != "")
            {
                listaProductos = _contenedorTrabajo.Producto.GetAll(c => c.Id.ToString().Contains(searchValue) || c.Nombre.Contains(searchValue), includeProperties: "Categoria");

            }
            else
            {
                listaProductos = _contenedorTrabajo.Producto.GetAll(includeProperties: "Categoria,SubCategoria,Marca,Tamanio");
            };

            // este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Producto.GetLambda<Producto>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaProductos = listaProductos.OrderByDescending(getNombreColumnaLambda);
            }
            else
            {
                listaProductos = listaProductos.OrderBy(getNombreColumnaLambda);
            }


            recordsTotal = listaProductos.Count();

            listaProductos = listaProductos.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaProductos });

        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Producto.Get(id);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;


            _logger.LogInformation("BORRADO DE PRODUCTO \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (objFromDb == null)
            {
                informacion = "Producto no encontrado";
                _logger.LogWarning("BORRADO DE PRODUCTO \r\n Error al querer borrar el Producto {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = false, message = "Error borrando el Producto" });
            }

            //borrado de imagen si tiene
            if (objFromDb.UrlImagen != null)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;            
                rutaImagenAntigua = Path.Combine(rutaPrincipal, objFromDb.UrlImagen.TrimStart('\\'));


                if (System.IO.File.Exists(rutaImagenAntigua))
                {
                    System.IO.File.Delete(rutaImagenAntigua);
                }
            }
          


            _contenedorTrabajo.Producto.Remove(objFromDb);
            _contenedorTrabajo.Save();

            informacion = "Nombre: " + objFromDb.Nombre + " - Id: " + objFromDb.Id;
            _logger.LogInformation("BORRADO DE PRODUCTO \r\n Producto borrado correctamente {Time} - {@informacion}", DateTime.Now, informacion);

            return Json(new { success = true, message = "Producto Borrado Correctamente" });
        }

        public async Task<IActionResult> MostrarOcultarProducto(int id)
        {
            var productoDesdeBd = _contenedorTrabajo.Producto.Get(id);

            //var objFromDb = _userManager.FindByNameAsync(productoDesdeBd.Email);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;


            _logger.LogInformation("HABILITACION DE PRODUCTO \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (productoDesdeBd == null)
            {

                _logger.LogWarning("HABILITACION DE PRODUCTO \r\n Error al buscar el Producto. {Time}", DateTime.Now);

                return Json(new { success = false, message = "Error Deshabilitando/Habilitando el producto." });
            }

            //en un solo metodo, habilito o deshabilito el producto segun corresponda

            if (productoDesdeBd.Estado == true)
            {
                productoDesdeBd.Estado = false;
                _contenedorTrabajo.Save();


                informacion = "Producto DesHabilitado Correctamente";
                _logger.LogWarning("HABILITACION DE PRODUCTO \r\n DesHabilitación de Productos {Time} - {@informacion}", DateTime.Now, informacion);


                return Json(new { success = true, message = "Producto DesHabilitado Correctamente" });
            }
            else
            {

        
                productoDesdeBd.Estado = true;

                _contenedorTrabajo.Save();

                informacion = "Producto Habilitado Correctamente";
                _logger.LogWarning("HABILITACION DE PRODUCTO \r\n Habilitación de Productos {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = true, message = "Producto Habilitado Correctamente" });
            }

        }

        #endregion

    }
}

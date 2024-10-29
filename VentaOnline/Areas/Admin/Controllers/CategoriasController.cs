using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Packaging.Signing;
using System.Drawing.Printing;
using System.Security.Claims;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
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
        private readonly ILogger<Categoria> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment; // para acceder a las carpetas del proyecto, para guardar/editar la imagen   
        private readonly ApplicationDbContext _db;

        public CategoriasController(IContenedorTrabajo contenedorTrabajo, ILogger<Categoria> logger, IWebHostEnvironment hostingEnvironment, ApplicationDbContext db)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
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
                        //emailUsuarioActual = "FLAVIA";

                        _logger.LogInformation("CREACIÓN DE CATEGORIA \r\n Usuario registrado para el guardado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                        #region Imagen
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivo = HttpContext.Request.Form.Files;

                        if (categoria.Id == 0 && archivo.Count() > 0)
                        {
                            //Nueva categoría
                            nombreArchivo = Guid.NewGuid().ToString();   //como es nuevo el producto, le pongo un Guid como nombre
                            subidas = Path.Combine(rutaPrincipal, @"imagenes\categorias"); //accederá a la carpeta en wwwroot
                            extension = Path.GetExtension(archivo[0].FileName);


                            //guardo la ruta en la base de datos
                            categoria.UrlImagen = @"\imagenes\categorias\" + nombreArchivo + extension;


                            informacion = "URL Imagen: " + categoria.UrlImagen;
                            _logger.LogInformation("CREACIÓN DE CATEGORIA \r\n Guardado de imagen {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        #endregion

                        //Logica para guardar en BD
                        _contenedorTrabajo.Categoria.Add(categoria);

                        _contenedorTrabajo.Save();

                        if (archivo.Count() > 0)
                        {

                            //recien guardo la imagen en la carpeta cuando se que todo fue bien
                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivo[0].CopyTo(fileStreams);

                                _logger.LogInformation("CREACIÓN DE CATEGORIA \r\n Guardado de Imagen en carpeta {Time}", DateTime.Now);
                            }
                        }

                        transaction.Commit();
                        _logger.LogInformation("CREACIÓN DE PRODUCTO \r\n Commit Exitoso {Time}", DateTime.Now);


                        informacion = "CATEGORIA: " + categoria.Nombre;
                        _logger.LogInformation("CREACIÓN DE CATEGORIA \r\n Categoria guardada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                        {

                            if (ex.InnerException.Message.Contains("IX_Categorias_Nombre"))
                            {
                                ModelState.AddModelError(string.Empty, "Esta Categoría ya existe");
                            }

                            else
                            {
                                ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                            }

                            informacion = ex.InnerException.Message;
                            _logger.LogWarning("CREACIÓN DE CATEGORIA \r\n Error al querer guardar en Categoría - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);
                            informacion = ex.Message;
                            _logger.LogWarning("CREACIÓN DE CATEGORIA \r\n Error al querer guardar en Categoría {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        return View(categoria);
                    }
                }
            }
            return View(categoria);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            Categoria categoria = new Categoria();
            categoria = _contenedorTrabajo.Categoria.Get(id);


            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Categoria categoria)
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


                    _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Usuario registrado para la edición: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                        #region Imagen
                        string rutaPrincipal = _hostingEnvironment.WebRootPath;
                        var archivos = HttpContext.Request.Form.Files;

                        var rutaImagenDesdeBd = _contenedorTrabajo.Categoria.Get(categoria.Id).UrlImagen;


                        if (archivos.Count() > 0)
                        {
                            //Nuevo imagen para el artículo
                            nombreArchivo = Guid.NewGuid().ToString();
                            subidas = Path.Combine(rutaPrincipal, @"imagenes\categorias");
                            extension = Path.GetExtension(archivos[0].FileName);


                            if (rutaImagenDesdeBd != null)
                            {
                                //guardamos la ruta de la imagen de este archivo, para que si luego todo va bien, antes del commit eliminemos la imagen
                                // la guardo porque sino se pierde en el UPDATE y no vuelve a atras con el rollback
                                rutaImagenAntigua = Path.Combine(rutaPrincipal, rutaImagenDesdeBd.TrimStart('\\'));

                                informacion = "URL Imagen Antigua: " + rutaImagenAntigua;
                                _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);
                            }

                            categoria.UrlImagen = @"\imagenes\categorias\" + nombreArchivo + extension;

                            informacion = "URL Imagen Nueva: " + categoria.UrlImagen;
                            _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);


                        }
                        else
                        {
                            //Aquí sería cuando la imagen ya existe y se conserva
                            categoria.UrlImagen = rutaImagenDesdeBd;

                            informacion = "Se conserva la misma imagen.";
                            _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Región imagen {Time} - {@informacion}", DateTime.Now, informacion);
                        }
                        #endregion

                        //Logica para actualizar en BD
                        _contenedorTrabajo.Categoria.Update(categoria);

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

                                _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Guardado de Imagen en carpeta {Time}", DateTime.Now);
                            }
                        }



                        transaction.Commit();

                        _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Commit Exitoso {Time}", DateTime.Now);

                        informacion = "Nombre: " + categoria.Nombre + " - Id: " + categoria.Id;
                    _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Categoria editada correctamente {Time} - {@informacion}", DateTime.Now, informacion);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                        transaction.Rollback();

                        if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_Categorias_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Categoria ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);

                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("EDICIÓN DE CATEGORIA \r\n Error al querer editar la Categoria - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                        informacion = ex.Message;
                        _logger.LogWarning("EDICIÓN DE CATEGORIA \r\n Error al querer editar la Categoria {Time} - {@informacion}", DateTime.Now, informacion);
                    }
                        return View(categoria);
                    }

            }
        }
            return View(categoria);
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

            IEnumerable<Categoria>? listaCategorias;


            if (searchValue != "")
            {
                listaCategorias = _contenedorTrabajo.Categoria.GetAll(c => c.Id.ToString().Contains(searchValue) || c.Nombre.Contains(searchValue));

            }
            else
            {
                listaCategorias = _contenedorTrabajo.Categoria.GetAll();
            };

            // este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Categoria.GetLambda<Categoria>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaCategorias = listaCategorias.OrderByDescending(getNombreColumnaLambda);
            }
            else
            {
                listaCategorias = listaCategorias.OrderBy(getNombreColumnaLambda);
            }


            recordsTotal = listaCategorias.Count();

            listaCategorias = listaCategorias.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaCategorias });

        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var objFromDb = _contenedorTrabajo.Categoria.Get(id);

                claimsIdentity = (ClaimsIdentity)this.User.Identity;
                usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                emailUsuarioActual = usuarioActual.Subject.Name;


                _logger.LogInformation("BORRADO DE CATEGORIA \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                if (objFromDb == null)
                {
                    informacion = "Categoria no encontrada";
                    _logger.LogWarning("BORRADO DE CATEGORIA \r\n Error al querer borrar la Categoria {Time} - {@informacion}", DateTime.Now, informacion);
                    Thread.Sleep(250);
                    return Json(new { success = false, message = "Error borrando la Categoria" });
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

                _contenedorTrabajo.Categoria.Remove(objFromDb);
                _contenedorTrabajo.Save();

                informacion = "Nombre: " + objFromDb.Nombre + " - Id: " + objFromDb.Id;
                _logger.LogInformation("BORRADO DE CATEGORIA \r\n Categoria borrada correctamente {Time} - {@informacion}", DateTime.Now, informacion);
                Thread.Sleep(250);
                return Json(new { success = true, message = "Categoria Borrada Correctamente" });
            }
            catch (DbUpdateException ex)
            {


                if (ex.InnerException != null &&
                      ex.InnerException.Message != null)
                {

                    if (ex.InnerException.Message.Contains("FK_SubCategoria_Categoria_CategoriaId"))
                    {
                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("BORRADO DE CATEGORIA \r\n Error al querer borrar en Categoría - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                        //AGREGO UN SLEEP AL HILO PARA QUE PUEDAN SALIR CORRECTAMENTE LOS MENSAJES DE JAVASCRIPT
                        Thread.Sleep(250);
                        return Json(new { success = false, message = "No puede borrarse, existen subcategorías asociadas a esta categoría" });
                    }

                    else
                    {
                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("BORRADO DE CATEGORIA \r\n Error al querer borrar en Categoría - InnerException {Time} - {@informacion}", DateTime.Now, informacion);
                        Thread.Sleep(250);
                        return Json(new { success = false, message = "Error borrando la Categoría" });
                    }

                }

                else
                {
                    informacion = ex.Message;
                    _logger.LogWarning("BORRADO DE CATEGORIA \r\n Error al querer borrar en Categoría {Time} - {@informacion}", DateTime.Now, informacion);
                    Thread.Sleep(250);
                    return Json(new { success = false, message = "Error borrando la Categoría" });
                }
            }
        }

        #endregion

    }
}

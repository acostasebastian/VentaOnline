using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic;
using System.Drawing.Printing;
using System.Security.Claims;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
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


        #endregion

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ILogger<Categoria> _logger;

        public CategoriasController(IContenedorTrabajo contenedorTrabajo, ILogger<Categoria> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _logger = logger;
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

                try
                {
                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;
                    //emailUsuarioActual = "FLAVIA";

                    _logger.LogInformation("CREACIÓN DE CATEGORIA \r\n Usuario registrado para el guardado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para guardar en BD
                    _contenedorTrabajo.Categoria.Add(categoria);

                    _contenedorTrabajo.Save();


                    informacion = "CATEGORIA: " + categoria.Nombre;
                    _logger.LogInformation("CREACIÓN DE CATEGORIA \r\n Categoria guardada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

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

                try
                {

                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;


                    _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Usuario registrado para la edición: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para actualizar en BD
                    _contenedorTrabajo.Categoria.Update(categoria);

                    _contenedorTrabajo.Save();

                    informacion = "Nombre: " + categoria.Nombre + " - Id: " + categoria.Id;
                    _logger.LogInformation("EDICIÓN DE CATEGORIA \r\n Categoria editada correctamente {Time} - {@informacion}", DateTime.Now, informacion);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {


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
            var objFromDb = _contenedorTrabajo.Categoria.Get(id);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;


            _logger.LogInformation("BORRADO DE CATEGORIA \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (objFromDb == null)
            {
                informacion = "Categoria no encontrada";
                _logger.LogWarning("BORRADO DE CATEGORIA \r\n Error al querer borrar la Categoria {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = false, message = "Error borrando la Categoria" });
            }

            _contenedorTrabajo.Categoria.Remove(objFromDb);
            _contenedorTrabajo.Save();

            informacion = "Nombre: " + objFromDb.Nombre + " - Id: " + objFromDb.Id;
            _logger.LogInformation("BORRADO DE CATEGORIA \r\n Categoria borrada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

            return Json(new { success = true, message = "Categoria Borrada Correctamente" });
        }

        #endregion

    }
}

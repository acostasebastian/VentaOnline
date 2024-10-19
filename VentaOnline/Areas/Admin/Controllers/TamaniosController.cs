using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TamaniosController : Controller
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
        private readonly ILogger<Tamanio> _logger;

        public TamaniosController(IContenedorTrabajo contenedorTrabajo, ILogger<Tamanio> logger)
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
        public async Task<IActionResult> Create(Tamanio tamanio)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;
                    

                    _logger.LogInformation("CREACIÓN DE TAMAÑO \r\n Usuario registrado para el guardado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para guardar en BD
                    _contenedorTrabajo.Tamanio.Add(tamanio);

                    _contenedorTrabajo.Save();


                    informacion = "TAMAÑO: " + tamanio.Nombre;
                    _logger.LogInformation("CREACIÓN DE TAMAÑO \r\n Tamaño guardado correctamente {Time} - {@informacion}", DateTime.Now, informacion);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_Tamanios_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Este Tamaño ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("CREACIÓN DE TAMAÑO \r\n Error al querer guardar en Tamaño - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);
                        informacion = ex.Message;
                        _logger.LogWarning("CREACIÓN DE TAMAÑO \r\n Error al querer guardar en Tamaño {Time} - {@informacion}", DateTime.Now, informacion);
                    }

                }
            }
            return View(tamanio);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            Tamanio tamanio = new Tamanio();
            tamanio = _contenedorTrabajo.Tamanio.Get(id);


            if (tamanio == null)
            {
                return NotFound();
            }
            return View(tamanio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tamanio tamanio)
        {
            if (ModelState.IsValid)
            {

                try
                {

                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;


                    _logger.LogInformation("EDICIÓN DE TAMAÑO \r\n Usuario registrado para la edición: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para actualizar en BD
                    _contenedorTrabajo.Tamanio.Update(tamanio);

                    _contenedorTrabajo.Save();

                    informacion = "Nombre: " + tamanio.Nombre + " - Id: " + tamanio.Id;
                    _logger.LogInformation("EDICIÓN DE TAMAÑO \r\n Tamaño editado correctamente {Time} - {@informacion}", DateTime.Now, informacion);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {


                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_Tamanios_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Tamaño ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);

                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("EDICIÓN DE TAMAÑO \r\n Error al querer editar la Tamaño - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                        informacion = ex.Message;
                        _logger.LogWarning("EDICIÓN DE TAMAÑO \r\n Error al querer editar la Tamaño {Time} - {@informacion}", DateTime.Now, informacion);
                    }
                }


            }
            return View(tamanio);
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

            IEnumerable<Tamanio>? listaTamanios;


            if (searchValue != "")
            {
                listaTamanios = _contenedorTrabajo.Tamanio.GetAll(c => c.Id.ToString().Contains(searchValue) || c.Nombre.Contains(searchValue));

            }
            else
            {
                listaTamanios = _contenedorTrabajo.Tamanio.GetAll();
            };

            // este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Tamanio.GetLambda<Tamanio>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaTamanios = listaTamanios.OrderByDescending(getNombreColumnaLambda);
            }
            else
            {
                listaTamanios = listaTamanios.OrderBy(getNombreColumnaLambda);
            }


            recordsTotal = listaTamanios.Count();

            listaTamanios = listaTamanios.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaTamanios });

        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Tamanio.Get(id);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;


            _logger.LogInformation("BORRADO DE TAMAÑO \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (objFromDb == null)
            {
                informacion = "Tamaño no encontrado";
                _logger.LogWarning("BORRADO DE TAMAÑO \r\n Error al querer borrar la Tamaño {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = false, message = "Error borrando la Tamaño" });
            }

            _contenedorTrabajo.Tamanio.Remove(objFromDb);
            _contenedorTrabajo.Save();

            informacion = "Nombre: " + objFromDb.Nombre + " - Id: " + objFromDb.Id;
            _logger.LogInformation("BORRADO DE TAMAÑO \r\n Tamaño borrado correctamente {Time} - {@informacion}", DateTime.Now, informacion);

            return Json(new { success = true, message = "Tamaño Borrado Correctamente" });
        }

        #endregion
    }
}

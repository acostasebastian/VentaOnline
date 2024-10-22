using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcasController : Controller
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
        private readonly ILogger<Marca> _logger;

        public MarcasController(IContenedorTrabajo contenedorTrabajo, ILogger<Marca> logger)
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
        public async Task<IActionResult> Create(Marca marca)
        {
            if (ModelState.IsValid)
            {              

                try
                {
                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;


                    _logger.LogInformation("CREACIÓN DE MARCA \r\n Usuario registrado para el guardado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para guardar en BD
                    _contenedorTrabajo.Marca.Add(marca);

                    _contenedorTrabajo.Save();


                    informacion = "MARCA: " + marca.Nombre;
                    _logger.LogInformation("CREACIÓN DE MARCA \r\n Marca guardada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_Marcas_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Marca ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("CREACIÓN DE MARCA \r\n Error al querer guardar en Marca - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);
                        informacion = ex.Message;
                        _logger.LogWarning("CREACIÓN DE MARCA \r\n Error al querer guardar en Marca {Time} - {@informacion}", DateTime.Now, informacion);
                    }

                }
            }
            return View(marca);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            Marca marca = new Marca();
            marca = _contenedorTrabajo.Marca.Get(id);


            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Marca marca)
        {
            if (ModelState.IsValid)
            {
              

                try
                {
                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;


                    _logger.LogInformation("EDICIÓN DE MARCA \r\n Usuario registrado para la edición: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para actualizar en BD
                    _contenedorTrabajo.Marca.Update(marca);

                    _contenedorTrabajo.Save();

                    informacion = "Nombre: " + marca.Nombre + " - Id: " + marca.Id;
                    _logger.LogInformation("EDICIÓN DE MARCA \r\n Marca editada correctamente {Time} - {@informacion}", DateTime.Now, informacion);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {


                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_Marcas_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Marca ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);

                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("EDICIÓN DE MARCA \r\n Error al querer editar la Marca - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                        informacion = ex.Message;
                        _logger.LogWarning("EDICIÓN DE MARCA \r\n Error al querer editar la Marca {Time} - {@informacion}", DateTime.Now, informacion);
                    }
                }


            }
            return View(marca);
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

            IEnumerable<Marca>? listaMarcas;


            if (searchValue != "")
            {
                listaMarcas = _contenedorTrabajo.Marca.GetAll(c => c.Id.ToString().Contains(searchValue) || c.Nombre.Contains(searchValue));

            }
            else
            {
                listaMarcas = _contenedorTrabajo.Marca.GetAll();
            };

            // este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.Marca.GetLambda<Marca>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaMarcas = listaMarcas.OrderByDescending(getNombreColumnaLambda);
            }
            else
            {
                listaMarcas = listaMarcas.OrderBy(getNombreColumnaLambda);
            }


            recordsTotal = listaMarcas.Count();

            listaMarcas = listaMarcas.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaMarcas });
            
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var objFromDb = _contenedorTrabajo.Marca.Get(id);

                claimsIdentity = (ClaimsIdentity)this.User.Identity;
                usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                emailUsuarioActual = usuarioActual.Subject.Name;


                _logger.LogInformation("BORRADO DE MARCA \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                if (objFromDb == null)
                {
                    informacion = "Marca no encontrada";
                    _logger.LogWarning("BORRADO DE MARCA \r\n Error al querer borrar la Marca {Time} - {@informacion}", DateTime.Now, informacion);

                    Thread.Sleep(250);
                    return Json(new { success = false, message = "Error borrando la Marca" });
                }

                _contenedorTrabajo.Marca.Remove(objFromDb);
                _contenedorTrabajo.Save();

                informacion = "Nombre: " + objFromDb.Nombre + " - Id: " + objFromDb.Id;
                _logger.LogInformation("BORRADO DE MARCA \r\n Marca borrada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

                Thread.Sleep(250);
                return Json(new { success = true, message = "Marca Borrada Correctamente" });
            }
            catch (Exception ex)
            {


                if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                {

                    if (ex.InnerException.Message.Contains("FK_Producto_Marca_MarcaId"))
                    {
                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("BORRADO DE MARCA \r\n Error al querer borrar en Marca - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                        //AGREGO UN SLEEP AL HILO PARA QUE PUEDAN SALIR CORRECTAMENTE LOS MENSAJES DE JAVASCRIPT
                        Thread.Sleep(250);
                        return Json(new { success = false, message = "No puede borrarse, existen productos asociados a esta marca" });
                    }

                    else
                    {
                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("BORRADO DE MARCA \r\n Error al querer borrar en Marca - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                        Thread.Sleep(250);
                        return Json(new { success = false, message = "Error borrando la Marca" });
                    }

                }

                else
                {
                    informacion = ex.Message;
                    _logger.LogWarning("BORRADO DE MARCA \r\n Error al querer borrar en Marca {Time} - {@informacion}", DateTime.Now, informacion);
                    Thread.Sleep(250);
                    return Json(new { success = false, message = "Error borrando la Marca" });
                }
            }
        }

        #endregion
    }
}

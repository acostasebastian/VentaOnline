using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;
using VentaOnline.Models.ViewModels;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriasController : Controller
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
        private readonly ILogger<SubCategoria> _logger;

        public SubCategoriasController(IContenedorTrabajo contenedorTrabajo, ILogger<SubCategoria> logger)
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
            SubCategoriaViewModel subCategoriaVM = new SubCategoriaViewModel()
            {
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            return View(subCategoriaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoriaViewModel subCategoriaVM)
        {
            if (ModelState.IsValid)
            {              

                try
                {
                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;
                    //emailUsuarioActual = "FLAVIA";

                    _logger.LogInformation("CREACIÓN DE SUBCATEGORIA \r\n Usuario registrado para el guardado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para guardar en BD
                    _contenedorTrabajo.SubCategoria.Add(subCategoriaVM);

                    _contenedorTrabajo.Save();


                    informacion = "SUBCATEGORIA: " + subCategoriaVM.Nombre;
                    _logger.LogInformation("CREACIÓN DE SUBCATEGORIA \r\n SubCategoria guardada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_SubCategorias_CategoriaId_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta SubCategoría ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);
                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("CREACIÓN DE SUBCATEGORIA \r\n Error al querer guardar en SubCategoría - InnerException {Time} - {@informacion}", DateTime.Now, informacion);

                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);
                        informacion = ex.Message;
                        _logger.LogWarning("CREACIÓN DE SUBCATEGORIA \r\n Error al querer guardar en SubCategoría {Time} - {@informacion}", DateTime.Now, informacion);
                    }

                }
            }

            subCategoriaVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            return View(subCategoriaVM);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            SubCategoriaViewModel subCategoriaViewModel = new SubCategoriaViewModel();
        

            if (id != null)
            {
                SubCategoria subCategoria = _contenedorTrabajo.SubCategoria.Get(id.GetValueOrDefault());

                if (subCategoria == null)
                {
                    return NotFound();

                }

                subCategoriaViewModel = ToViewModel(subCategoria);
                subCategoriaViewModel.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();

            }
            return View(subCategoriaViewModel);
        }

        private SubCategoriaViewModel ToViewModel(SubCategoria subCategoria)
        {
            return new SubCategoriaViewModel
            { CategoriaId = subCategoria.CategoriaId,
              Nombre = subCategoria.Nombre,
              Categoria = subCategoria.Categoria

            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoriaViewModel subCategoriaVM)
        {
            if (ModelState.IsValid)
            {
              
                try
                {
                    claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    emailUsuarioActual = usuarioActual.Subject.Name;


                    _logger.LogInformation("EDICIÓN DE SUBCATEGORIA \r\n Usuario registrado para la edición: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

                    //Logica para actualizar en BD
                    _contenedorTrabajo.SubCategoria.Update(subCategoriaVM);


                    _contenedorTrabajo.Save();

                    informacion = "Nombre: " + subCategoriaVM.Nombre + " - Id: " + subCategoriaVM.Id;
                    _logger.LogInformation("EDICIÓN DE SUBCATEGORIA \r\n SubCategoria editada correctamente {Time} - {@informacion}", DateTime.Now, informacion);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {


                    if (ex.InnerException != null &&
                       ex.InnerException.Message != null)
                    {

                        if (ex.InnerException.Message.Contains("IX_SubCategorias_CategoriaId_Nombre"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta SubCategoría ya existe");
                        }

                        else
                        {
                            ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.InnerException.Message);

                        }

                        informacion = ex.InnerException.Message;
                        _logger.LogWarning("EDICIÓN DE SUBCATEGORIA \r\n Error al querer editar la SubCategoria - InnerException {Time} - {@informacion}", DateTime.Now, informacion);
                        subCategoriaVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador e indique el siguiente error >> Error: " + ex.Message);

                        informacion = ex.Message;
                        _logger.LogWarning("EDICIÓN DE SUBCATEGORIA \r\n Error al querer editar la SubCategoria {Time} - {@informacion}", DateTime.Now, informacion);
                        subCategoriaVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                    }
                }


            }

            return View(subCategoriaVM);
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

            IEnumerable<SubCategoria>? listaSubCategorias;


            if (searchValue != "")
            {
                listaSubCategorias = _contenedorTrabajo.SubCategoria.GetAll(c => c.Id.ToString().Contains(searchValue) || c.Nombre.Contains(searchValue), includeProperties: "Categoria"); 

            }
            else
            {
                listaSubCategorias = _contenedorTrabajo.SubCategoria.GetAll(includeProperties: "Categoria"); 
            };

            // este metodo al que llamo, me devuelve el resultado en una variable,
            //convierte el nombre de la columna que envia datatable en el formato necesario para el ordenamiento >> x=> x.Id por ejemplo            
            var getNombreColumnaLambda = _contenedorTrabajo.SubCategoria.GetLambda<SubCategoria>(sortColum);

            if (sortColumnDir == "desc")
            {
                listaSubCategorias = listaSubCategorias.OrderByDescending(getNombreColumnaLambda);
            }
            else
            {
                listaSubCategorias = listaSubCategorias.OrderBy(getNombreColumnaLambda);
            }


            recordsTotal = listaSubCategorias.Count();

            listaSubCategorias = listaSubCategorias.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaSubCategorias });

        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.SubCategoria.Get(id);

            claimsIdentity = (ClaimsIdentity)this.User.Identity;
            usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            emailUsuarioActual = usuarioActual.Subject.Name;


            _logger.LogInformation("BORRADO DE SUBCATEGORIA \r\n Usuario registrado: {Time} - {@emailUsuarioActual}", DateTime.Now, emailUsuarioActual);

            if (objFromDb == null)
            {
                informacion = "SubCategoria no encontrada";
                _logger.LogWarning("BORRADO DE SUBCATEGORIA \r\n Error al querer borrar la SubCategoria {Time} - {@informacion}", DateTime.Now, informacion);

                return Json(new { success = false, message = "Error borrando la SubCategoria" });
            }

            _contenedorTrabajo.SubCategoria.Remove(objFromDb);
            _contenedorTrabajo.Save();

            informacion = "Nombre: " + objFromDb.Nombre + " - Id: " + objFromDb.Id;
            _logger.LogInformation("BORRADO DE SUBCATEGORIA \r\n SubCategoria borrada correctamente {Time} - {@informacion}", DateTime.Now, informacion);

            return Json(new { success = true, message = "SubCategoria Borrada Correctamente" });
        }

        #endregion
    }
}

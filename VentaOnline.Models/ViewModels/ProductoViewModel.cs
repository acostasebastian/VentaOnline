using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaOnline.Models.ViewModels
{
    public class ProductoViewModel : Producto
    {

        public IEnumerable<SelectListItem>? ListaCategorias { get; set; }

        public IEnumerable<SelectListItem>? ListaMarcas { get; set; }

        public IEnumerable<SelectListItem>? ListaTamanios { get; set; }

        public IEnumerable<SelectListItem>? ListaSubCategorias { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //  [Range(1, double.MaxValue, ErrorMessage = "Debe ingresar un precio entre {1} y {2}")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public string? sPrecio { get; set; }
    }
}

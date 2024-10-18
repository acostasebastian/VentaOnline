using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; //NuGet >> Microsoft.AspNetCore.Mvc.ViewFeatures

namespace VentaOnline.Models.ViewModels
{
    public class SubCategoriaViewModel : SubCategoria
    {
        //[Display(Name = "Categoría")]
        //[Required(ErrorMessage = "Debe ingresar la Categoría")]
        public int CategoriaVMId { get; set; }

        public IEnumerable<SelectListItem>? ListaCategorias { get; set; }
    }
}

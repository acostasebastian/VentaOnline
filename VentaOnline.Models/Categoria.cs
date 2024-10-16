using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VentaOnline.Models
{
    [Index("Nombre", IsUnique = true, Name = "IX_Categorias_Nombre")]
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar la Categoría")]
        [StringLength(50, ErrorMessage = "El campo debe tener entre {2} y {1} caracteres", MinimumLength = 3)]
        [Display(Name = "Categoría")]
        public string Nombre { get; set; }
      
    }
}

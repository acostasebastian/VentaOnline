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
    [Index("Nombre", IsUnique = true, Name = "IX_Tamanios_Nombre")]
    public class Tamanio
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar el Tamaño")]
        [StringLength(50, ErrorMessage = "El campo debe tener entre {2} y {1} caracteres", MinimumLength = 1)]
        [Display(Name = "Tamaño")]
        public string Nombre { get; set; }
    }
}

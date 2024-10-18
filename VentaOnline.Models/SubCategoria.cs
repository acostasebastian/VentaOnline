using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace VentaOnline.Models
{

    //[Index("Nombre", IsUnique = true, Name = "IX_SubCategorias_Nombre")]
    public class SubCategoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar la SubCategoría")]
        [StringLength(50, ErrorMessage = "El campo debe tener entre {2} y {1} caracteres", MinimumLength = 3)]
        [Display(Name = "SubCategoría")]        
        public string Nombre { get; set; }


        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
     
        public Categoria? Categoria { get; set; }
    }
}

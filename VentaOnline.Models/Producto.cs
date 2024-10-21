using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaOnline.Models
{
    //ESTA ES LA FORMA DE CREAR EL INDEX DESDE LA CLASE Y NO DESDE EL modelBuilder DEL ApplicationDbContext
    //  [Index(nameof(CategoriaId), nameof(SubCategoriaId),nameof(MarcaId), nameof(Nombre), IsUnique = true, Name = "IX_Productos_CategoriaId_SubCategoriaId_MarcaId_Nombre")]
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Categoría")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }


        [ForeignKey("CategoriaId")]
        public Categoria? Categoria { get; set; }



        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una SubCategoría")]
        [Display(Name = "SubCategoría")]
        public int SubCategoriaId { get; set; }

        [ForeignKey("SubCategoriaId")]
        public SubCategoria? SubCategoria { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una Marca")]
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [ForeignKey("MarcaId")]
        public Marca? Marca { get; set; }

               
        
        [Display(Name = "Tamaño")]
        public int? TamanioId { get; set; }

        [ForeignKey("TamanioId")]
        public Tamanio? Tamanio { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo debe tener entre {2} y {1} caracteres", MinimumLength = 2)]       
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
      //  [Range(1, double.MaxValue, ErrorMessage = "Debe ingresar un precio entre {1} y {2}")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Precio { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Foto")]
        public string? UrlImagen { get; set; }

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string? Observaciones { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Debe ingresar un Stock entre {1} y {2}")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Stock { get; set; }

        [Display(Name = "Activo")]
        public bool Estado { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaOnline.Models
{
    public class Carrito
    {
        [Key]
        public int Id { get; set; }

        public string CartId { get; set; }

        public int ProductoId { get; set; }


        [ForeignKey("ProductoId")]

        public Producto? Producto { get; set; }
        public int Cantidad { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }
    }
}

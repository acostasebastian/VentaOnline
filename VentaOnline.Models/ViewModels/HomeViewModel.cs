using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaOnline.Models.ViewModels
{
    public class HomeViewModel : Producto  
    {

        public IEnumerable<Categoria>? ListaCategorias { get; set; }

        public IEnumerable<SubCategoria>? ListaSubCategorias { get; set; }

        public IEnumerable<Producto>? ListaProductos { get; set; }
    }
}

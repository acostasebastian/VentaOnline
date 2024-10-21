using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;

namespace VentaOnline.DataAccess.Data.Repository
{
    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Producto producto)
        {
            var objDesdeDb = _db.Producto.FirstOrDefault(s => s.Id == producto.Id);
            objDesdeDb.Nombre = producto.Nombre;
            objDesdeDb.CategoriaId = producto.CategoriaId;
            objDesdeDb.SubCategoriaId= producto.SubCategoriaId;
            objDesdeDb.MarcaId = producto.MarcaId;  
            objDesdeDb.TamanioId = producto.TamanioId;  
            objDesdeDb.Observaciones = producto.Observaciones;
            objDesdeDb.Precio  = producto.Precio;
            objDesdeDb.Stock = producto.Stock;
            objDesdeDb.UrlImagen = producto.UrlImagen;
            objDesdeDb.Estado = producto.Estado;    

        }

    }
}

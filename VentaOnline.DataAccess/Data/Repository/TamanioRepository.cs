using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class TamanioRepository : Repository<Tamanio>, ITamanioRepository
    {
        private readonly ApplicationDbContext _db;

        public TamanioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Tamanio tamanio)
        {
            var objDesdeDb = _db.Tamanio.FirstOrDefault(s => s.Id == tamanio.Id);
            objDesdeDb.Nombre = tamanio.Nombre;

        }

        //public IEnumerable<SelectListItem>? GetListaCategorias()
        //{
        //    return _db.Categoria.Select(i => new SelectListItem()
        //    {
        //        Text = i.Nombre,
        //        Value = i.Id.ToString(),

        //    });
        //}
    }
}

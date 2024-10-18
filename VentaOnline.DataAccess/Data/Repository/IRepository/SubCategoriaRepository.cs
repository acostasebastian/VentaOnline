using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Data;
using VentaOnline.Models;

namespace VentaOnline.DataAccess.Data.Repository.IRepository
{
    public class SubCategoriaRepository : Repository<SubCategoria>, ISubCategoriaRepository
    {

        private readonly ApplicationDbContext _db;

        public SubCategoriaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(SubCategoria subCategoria)
        {
            var objDesdeDb = _db.SubCategoria.FirstOrDefault(s => s.Id == subCategoria.Id);
            objDesdeDb.Nombre = subCategoria.Nombre;
            objDesdeDb.CategoriaId = subCategoria.CategoriaId;

        }

        //public IEnumerable<SelectListItem>? GetListaPlataformas()
        //{
        //    return _db.Plataforma.Select(i => new SelectListItem()
        //    {
        //        Text = i.URL,
        //        Value = i.Id.ToString(),

        //    });
        //}
    }
}

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

        public IEnumerable<SelectListItem>? GetListaSubCategoriasPorCategoria(int categoriaId)
        {
            return _db.SubCategoria.Where(c => c.CategoriaId == categoriaId).Select(i => new SelectListItem()
            {
                Text = i.Nombre,
                Value = i.Id.ToString(),

            });
        }
    }
}

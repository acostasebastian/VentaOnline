﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;
using VentaOnline.Models;

namespace VentaOnline.DataAccess.Data.Repository
{
    public class MarcaRepository : Repository<Marca>, IMarcaRepository
    {

        private readonly ApplicationDbContext _db;

        public MarcaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Marca marca)
        {
            var objDesdeDb = _db.Marca.FirstOrDefault(s => s.Id == marca.Id);
            objDesdeDb.Nombre = marca.Nombre;

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

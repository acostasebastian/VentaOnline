﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Data;
using VentaOnline.DataAccess.Data.Repository.IRepository;

namespace VentaOnline.DataAccess.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {

        private readonly ApplicationDbContext _db;

        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Categoria = new CategoriaRepository(_db);
            SubCategoria = new SubCategoriaRepository(_db);
            Marca = new MarcaRepository(_db);
            Tamanio = new TamanioRepository(_db);
            Producto = new ProductoRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
        }


        public ICategoriaRepository Categoria { get; private set; }

        public ISubCategoriaRepository SubCategoria { get; private set; }

        public IMarcaRepository Marca { get; private set; }

        public ITamanioRepository Tamanio { get; private set; }

        public IProductoRepository Producto { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

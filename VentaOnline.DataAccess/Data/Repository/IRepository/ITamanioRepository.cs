using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Models;

namespace VentaOnline.DataAccess.Data.Repository.IRepository
{
    public interface ITamanioRepository : IRepository<Tamanio>
    {
        void Update(Tamanio tamanio);

       // IEnumerable<SelectListItem>? GetListaCategorias();
    }
}

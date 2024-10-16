using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Models;

namespace VentaOnline.DataAccess.Data.Repository.IRepository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        void Update(Categoria categoria);

      //  IEnumerable<SelectListItem>? GetListaPlataformas();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaOnline.DataAccess.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        //Aquí se deben de ir agregando los diferentes repositorios
        ICategoriaRepository Categoria { get; }

        ISubCategoriaRepository SubCategoria { get; }

        IMarcaRepository Marca { get; }

        ITamanioRepository Tamanio { get; }

        void Save();
    }
}

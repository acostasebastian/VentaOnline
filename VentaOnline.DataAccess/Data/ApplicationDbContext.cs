using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VentaOnline.Models;

namespace VentaOnline.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //CREO UN INDICE PARA SUBCATEGORIA QUE INCLUYE CategoríaId Y Nombre >> para no repetir nombre para una misma categoria
            modelBuilder.Entity<SubCategoria>()
                //.HasAlternateKey(a => new { a.Nombre, a.CategoriaId }).IsUnique();
                .HasIndex(a => new { a.Nombre, a.CategoriaId })
                .HasDatabaseName("IX_SubCategorias_CategoriaId_Nombre")
                .IsUnique();
        }

        public DbSet<Categoria> Categoria { get; set; }

        public DbSet<Marca> Marca { get; set; }

        public DbSet<SubCategoria> SubCategoria { get; set; }
    }
}

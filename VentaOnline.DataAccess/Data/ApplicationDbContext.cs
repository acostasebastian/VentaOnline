using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
                .HasIndex(a => new { a.Nombre, a.CategoriaId })
                .HasDatabaseName("IX_SubCategorias_CategoriaId_Nombre")                
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(a => new { a.CategoriaId, a.SubCategoriaId, a.MarcaId, a.Nombre })
                .HasDatabaseName("IX_Productos_CategoriaId_SubCategoriaId_MarcaId_Nombre")
                .IsUnique();

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

            }

        }

        public DbSet<Categoria> Categoria { get; set; }

        public DbSet<Marca> Marca { get; set; }

        public DbSet<SubCategoria> SubCategoria { get; set; }

        public DbSet<Tamanio> Tamanio { get; set; }

        public DbSet<Producto> Producto { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}

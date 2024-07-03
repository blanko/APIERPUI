using Microsoft.EntityFrameworkCore;
using APIERP.Entidades;

namespace APIERP
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear el procedimiento almacenado
            //modelBuilder.HasDbFunction(typeof(ApplicationDbContext).GetMethod(nameof(GetNewGuid))).HasName("GetNewGuid");

            modelBuilder.Entity<Category>().Property(p => p.Name).HasMaxLength(60);
            modelBuilder.Entity<Category>().Property(p => p.ImageUrl).IsUnicode();


            //Configurar el mapeo de Guid para cada entidad
            //modelBuilder.Entity<Category>(entity =>
            //{
            //    entity.Property(e => e.Guid).IsRequired().HasColumnType("uniqueidentifier");
            //});

            //modelBuilder.Entity<Store>(entity =>
            //{
            //    entity.Property(e => e.Guid).IsRequired().HasColumnType("uniqueidentifier");
            //});

            //modelBuilder.Entity<Order>(entity =>
            //{
            //    entity.Property(e => e.Guid).IsRequired().HasColumnType("uniqueidentifier");
            //});

            //modelBuilder.Entity<OrderDetail>(entity =>
            //{
            //    entity.Property(e => e.Guid).IsRequired().HasColumnType("uniqueidentifier");
            //});

            //modelBuilder.Entity<Product>(entity =>
            //{
            //    entity.Property(e => e.Guid).IsRequired().HasColumnType("uniqueidentifier");
            //});

            //modelBuilder.Entity<Raincheck>(entity =>
            //{
            //    entity.Property(e => e.Guid).IsRequired().HasColumnType("uniqueidentifier");
            //});

            // Mas en el futuro
        }

        public Guid GetNewGuid()
        {
            throw new Exception("Este método debe ser usado solo en consultas de LINQ a SQL.");
        }



        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Raincheck> Rainchecks { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Error> Errores { get; set; }
    }
}

//namespace yummyApp.Models.Data
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using yummyApp.Models;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CategoriaComida> CategoriaComidas { get; set; }
    public DbSet<CategoriaOrigen> CategoriaOrigenes { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetalleVentas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Producto>()
            .Property(p => p.precio)
            .HasPrecision(10, 2);

        builder.Entity<DetalleVenta>()
            .Property(d => d.precioProd)
            .HasPrecision(10, 2);
    }

public DbSet<yummyApp.Models.ProductoCarrito> ProductoCarrito { get; set; } = default!;
}

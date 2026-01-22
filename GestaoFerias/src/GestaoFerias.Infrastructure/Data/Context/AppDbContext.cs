using GestaoFerias.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoFerias.Infrastructure.Data.Context;

public class AppDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Usuario>(entity =>
    {
        entity.HasIndex(u => u.Matricula)
              .IsUnique();

        entity.Property(u => u.Matricula)
              .IsRequired();
    });
}

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
}

using GestaoFerias.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoFerias.Infrastructure.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
}

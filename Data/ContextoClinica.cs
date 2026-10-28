using Microsoft.EntityFrameworkCore;
using SistemaDeCitasVeterinarias.Models;

namespace SistemaDeCitasVeterinarias.Data;

public class ContextoClinica : DbContext
{
    public ContextoClinica(DbContextOptions<ContextoClinica> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();
    public DbSet<Veterinario> Veterinarios => Set<Veterinario>();
    public DbSet<Cita> Citas => Set<Cita>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Mascota>()
            .HasOne(m => m.Cliente).WithMany(p => p.Mascotas)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Cita>()
            .HasOne(c => c.Mascota).WithMany(m => m.Citas)
            .OnDelete(DeleteBehavior.Restrict);
        b.Entity<Cita>()
            .HasOne(c => c.Veterinario).WithMany(v => v.Citas)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Cita>().Property(c => c.Estado).HasConversion<string>();
    }
}
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TrabajoPracticoCripto.Models;

namespace TrabajoPracticoCripto.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Transaccion> Transacciones => Set<Transaccion>();

    }
}

using ApiPuntoVenta.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPuntoVenta.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Article> Articles { get; set; }
    }
}

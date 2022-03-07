using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TeTacApi.Models
{
  public class TeTacApiContext : DbContext
  {

    private readonly HttpContext _httpContext;

    public TeTacApiContext(DbContextOptions<TeTacApiContext> options, IHttpContextAccessor httpContextAccessor = null)
        : base(options)
    {
      _httpContext = httpContextAccessor?.HttpContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Droit>()
          .HasKey(o => new { o.IdApp, o.IdSalon, o.IdUtilisateur });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      
     
    }

   public DbSet<Salon> Salons { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Droit> Droits { get; set; }
  }
}


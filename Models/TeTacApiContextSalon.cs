using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TeTacApi.Models
{
  public class TeTacApiContextSalon : DbContext
  {

    private readonly HttpContext _httpContext;

    public TeTacApiContextSalon(DbContextOptions<TeTacApiContextSalon> options, IHttpContextAccessor httpContextAccessor = null)
        : base(options)
    {
      _httpContext = httpContextAccessor?.HttpContext;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      
      if (!optionsBuilder.IsConfigured)
      {
        //retrieve connection string from user claim (populated on authentication process in LoginController.cs)
        var _connectionstring = _httpContext?.User.FindFirst("connectionstring");      
        if (_connectionstring!=null) optionsBuilder.UseSqlServer(_connectionstring.Value + ";User Id=sa;Password=T&3511Vk1");



      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //modelBuilder
      //    .Entity<Passage>()
      //    .Property(e => e.IdContact)
      //    .HasConversion<int>();
    }

    public DbSet<Passage> Passages { get; set; }
  }
}


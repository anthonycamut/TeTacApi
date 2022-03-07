using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeTacApi.Models
{
  public class Salon
  {
    [System.ComponentModel.DataAnnotations.Key]
    public string IdSalon { get; set; }
    public string SalonServerName { get; set; }
    public string SalonDatabaseName { get; set; }
    
  }
}

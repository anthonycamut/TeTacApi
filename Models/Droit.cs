using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeTacApi.Models
{
  public class Droit
  {
    
    public int IdUtilisateur { get; set; }
    
    public string IdSalon { get; set; }
    
    public string IdApp { get; set; }
    
    
  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeTacApi.Models
{
  public class Utilisateur
  {
    [System.ComponentModel.DataAnnotations.Key]
    public int IdUtilisateur { get; set; }
    public string Nom { get; set; }

    public string Prenom { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    
  }
}

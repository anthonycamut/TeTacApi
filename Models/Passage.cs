using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeTacApi.Models
{
  [Table("vwApiPassages")]
  public class Passage
  {
    [System.ComponentModel.DataAnnotations.Key]
    [Column(TypeName = "decimal(18,0)")]
    public decimal AutoId { get; set; }
    public string IdContact { get; set; }
    public string IdCategorie { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Societe { get; set; }
    public string Tel { get; set; }
    public string Email { get; set; }
    public string IdLieuPassage { get; set; }
    public string CodeBarreLu { get; set; }
    public string SensPassage { get; set; }
    public Int64 DatePassage { get; set; }
    
    public DateTime DatePassage2 { get; set; }

  }
}

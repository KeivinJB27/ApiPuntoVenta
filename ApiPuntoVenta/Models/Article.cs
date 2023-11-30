using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPuntoVenta.Models
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArticleID { get; set; }

        [Required,StringLength(30)]
        public string? Name { get; set; }

        [Required,StringLength(500)]
        public string? Description { get; set; }
    }
}

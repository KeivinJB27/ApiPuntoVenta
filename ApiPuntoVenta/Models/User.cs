using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPuntoVenta.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Required, StringLength(30)]
        public string? Name { get; set; }

        [Required, StringLength(50)]
        public string? LastName { get; set; }

        [Required, StringLength(20)]
        public string? UserName { get; set; }

        [Required, StringLength(20)]
        public string? Password { get; set; }

        [Required, StringLength(100)]
        public string? Email { get; set; }

        [Required]
        public int PhoneNumber { get; set; }
    }
}

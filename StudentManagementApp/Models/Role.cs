using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty; // Admin, Student
    }
}

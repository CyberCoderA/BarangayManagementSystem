using System.ComponentModel.DataAnnotations;

namespace BarangayManagementSystem.API.Models
{
    public class IndigencyRequest
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; init; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Address { get; init; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Purpose { get; init; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BarangayManagementSystem.API.Models
{
    public class BarangayIdRequest
    {
        [Required]
        [StringLength(150)]
        public string Name { get; init; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string CivilStatus { get; init; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Gender { get; init; } = string.Empty;

        [Required]
        public DateOnly? DateOfBirth { get; init; }

        [Required]
        [StringLength(250)]
        public string HomeAddress { get; init; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ContactPerson { get; init; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string ContactNumber { get; init; } = string.Empty;

        [Required]
        public IFormFile? Photo { get; init; }
    }
}
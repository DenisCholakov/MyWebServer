using System;
using System.ComponentModel.DataAnnotations;

using static CarShop.Data.DataConstants;

namespace CarShop.Data.Models
{
    public class User
    {
        [Required]
        [MaxLength(IdMaxLength)]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(DefaultMaxLength)]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [MaxLength(DefaultMaxLength)]
        public string Password { get; set; }

        public bool IsMechanic { get; set; }
    }
}

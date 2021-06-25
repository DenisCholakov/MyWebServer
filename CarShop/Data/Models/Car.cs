using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static CarShop.Data.DataConstants;

namespace CarShop.Data.Models
{
    public class Car
    {
        public Car()
        {
            this.Issues = new HashSet<Issue>();
        }

        [Required]
        [MaxLength(IdMaxLength)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(DefaultMaxLength)]
        public string Model { get; set; }

        public int Year { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        [Required]
        [MaxLength(PlateNumberMaxLength)]
        public string PlateNumber { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public User Owner { get; set; }

        public IEnumerable<Issue> Issues { get; set; }
    }
}

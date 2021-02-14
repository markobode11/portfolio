using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Item
    {
        public int ItemId { get; set; }

        [MaxLength(128), StringLength(128, MinimumLength = 3,
             ErrorMessage = "Description has to be between 3 and 128 chars.")]
        public string Name { get; set; } = default!;

        [MaxLength(128), StringLength(128, MinimumLength = 3,
             ErrorMessage = "Location has to be between 3 and 128 chars.")]
        public string Location { get; set; } = default!;

        [Display(Name = "Current quantity")] public int CurrentQuantity { get; set; }

        [Display(Name = "Optimal quantity")] public int OptimalQuantity { get; set; }

        public int Price { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<JobItem>? JobItems { get; set; }
    }
}
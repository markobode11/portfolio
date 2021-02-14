using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Category
    {
        public int CategoryId { get; set; }

        [MaxLength(128, ErrorMessage = "Maximum 128 chars."), MinLength(3, ErrorMessage = "Minimum 3 chars.")]
        public string Name { get; set; } = default!;

        public ICollection<Item>? Items { get; set; }
    }
}
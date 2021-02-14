using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class JobItem
    {
        public int JobItemId { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public int JobId { get; set; }
        public Job? Job { get; set; }

        [Display(Name = "Quantity needed")]
        public int QuantityNeeded { get; set; }
    }
}
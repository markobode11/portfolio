using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class PerformedJob
    {
        public int PerformedJobId { get; set; }

        public int JobId { get; set; }
        public Job? Job { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Perform date")]
        public DateTime PerformDate { get; set; }
    }
}
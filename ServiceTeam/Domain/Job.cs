using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Job
    {
        public int JobId { get; set; }

        [MaxLength(128),
         StringLength(128,
             MinimumLength = 3,
             ErrorMessage = "Description has to be between 3 and 128 chars.")]
        public string Description { get; set; } = default!;

        [Display(Name = "Job Items")]
        public ICollection<JobItem>? JobItems { get; set; }
        public ICollection<PerformedJob>? PerformedJobs { get; set; }
    }
}
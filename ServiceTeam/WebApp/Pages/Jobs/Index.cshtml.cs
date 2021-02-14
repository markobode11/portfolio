using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Jobs
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Job> Job { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                await PerformJob((int) id);
                return Redirect("/PerformedJobs/Index");
            }
            Job = await _context.Jobs
                .Include(job => job.JobItems)
                .ThenInclude(item => item.Item)
                .ToListAsync();
            return Page();
        }

        private async Task PerformJob(int id)
        {
            var job = await _context.Jobs
                .Include(x => x.JobItems)
                .ThenInclude(x => x.Item)
                .FirstAsync(x => x.JobId == id);
            foreach (var jobItem in job.JobItems!)
            {
                jobItem.Item!.CurrentQuantity -= jobItem.QuantityNeeded;
            }

            _context.Add(new PerformedJob
            {
                JobId = id,
                PerformDate = DateTime.Now.Date
            });

            await _context.SaveChangesAsync();
        }

        public bool CanPerform(ICollection<JobItem>? jobItems)
        {
            if (jobItems == null) return true;

            return !jobItems.Any(item => item.Item!.CurrentQuantity < item.QuantityNeeded);
        }
    }
}
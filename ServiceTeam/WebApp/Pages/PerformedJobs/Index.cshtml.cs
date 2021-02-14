using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.PerformedJobs
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<PerformedJob> PerformedJob { get; set; } = default!;

        public async Task OnGetAsync()
        {
            PerformedJob = await _context.PerformedJobs
                .Include(p => p.Job)
                .ThenInclude(job => job!.JobItems)
                .ThenInclude(jobItem => jobItem.Item)
                .ToListAsync();
        }
    }
}
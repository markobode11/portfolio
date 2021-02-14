using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.JobItems
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<JobItem> JobItem { get; set; } = default!;

        public async Task OnGetAsync()
        {
            JobItem = await _context.JobItems
                .Include(j => j.Job)
                .Include(j => j.Item).ToListAsync();
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.JobItems
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        public SelectList Jobs { get; set; } = default!;
        public SelectList Items { get; set; } = default!;

        [BindProperty] public JobItem JobItem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JobItem = await _context.JobItems
                .Include(j => j.Job)
                .Include(j => j.Item).FirstOrDefaultAsync(m => m.JobItemId == id);

            if (JobItem == null)
            {
                return NotFound();
            }

            Jobs = new SelectList(_context.Jobs, "JobId", "Description");
            Items = new SelectList(_context.Items, "ItemId", "Name");
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(JobItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobItemExists(JobItem.JobItemId))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool JobItemExists(int id)
        {
            return _context.JobItems.Any(e => e.JobItemId == id);
        }
    }
}
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.JobItems
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public SelectList Jobs { get; set; } = default!;
        public SelectList Items { get; set; } = default!;
        
        [BindProperty] public JobItem JobItem { get; set; } = default!;

        public IActionResult OnGet()
        {
            Jobs = new SelectList(_context.Jobs, "JobId", "Description");
            Items = new SelectList(_context.Items, "ItemId", "Name");
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.JobItems.AddAsync(JobItem);
            await _context.SaveChangesAsync();

            if (id.HasValue)
            {
                return Redirect("/Jobs/Index");
            }

            return RedirectToPage("./Index");
        }
    }
}
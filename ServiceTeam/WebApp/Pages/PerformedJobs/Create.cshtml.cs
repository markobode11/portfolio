using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.PerformedJobs
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty] public PerformedJob PerformedJob { get; set; } = default!;
        public SelectList Jobs { get; set; } = default!;

        public IActionResult OnGet()
        {
            Jobs = new SelectList(_context.Jobs, "JobId", "Description");
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.PerformedJobs.AddAsync(PerformedJob);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
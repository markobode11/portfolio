using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Items
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get; set; } = default!;
        [MinLength(1), StringLength(128)][BindProperty(SupportsGet = true)] public string? Search { get; set; }
        [BindProperty(SupportsGet = true)] public bool Exclusion { get; set; }

        public async Task OnGetAsync(string? searchItems)
        {
            Item = await _context.Items
                .Include(i => i.Category)
                .ToListAsync();
            if (searchItems != null && Search != null)
            {
                var filtered = new List<Item>();
                if (searchItems == "name")
                {
                    filtered = Item
                        .Where(i => i.Name.ToLower().Contains(Search!.ToLower().Trim())).ToList();
                }
                else if (searchItems == "category")
                {
                    filtered = Item
                        .Where(i => i.Category!.Name.ToLower().Contains(Search!.ToLower().Trim())).ToList();
                }

                Item = Exclusion ? Item.Where(x => !filtered.Contains(x)).ToList() : filtered;
            }

        }

        public string GetClass(Item item)
        {
            string classString;
            if (item.CurrentQuantity == 0)
            {
                classString = "alert-danger";
            }
            else if (item.CurrentQuantity < item.OptimalQuantity)
            {
                classString = "alert-warning";
            }
            else
            {
                classString = "alert-success";
            }

            return classString;
        }
    }
}
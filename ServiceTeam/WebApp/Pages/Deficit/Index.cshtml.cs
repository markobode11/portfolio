using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Deficit
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Item> Item { get;set; } = default!;

        public async Task OnGetAsync(int? id, bool? refillAll)
        {
            if (id.HasValue)
            {
                var refilled = await _context.Items.FirstAsync(x => x.ItemId == id);
                refilled.CurrentQuantity = refilled.OptimalQuantity;
                await _context.SaveChangesAsync();
            }
            
            Item = await _context.Items.Where(item => item.OptimalQuantity > item.CurrentQuantity)
                .Include(i => i.Category).ToListAsync();
            
            if (refillAll.HasValue)
            {
                foreach (var item in Item)
                {
                    item.CurrentQuantity = item.OptimalQuantity;
                }

                await _context.SaveChangesAsync();
                Item = await _context.Items.Where(item => item.OptimalQuantity > item.CurrentQuantity)
                    .Include(i => i.Category).ToListAsync();
            }
        }

        public string GetClass(Item item)
        {
            string classString = item.CurrentQuantity == 0 ? "alert-danger" : "alert-warning";

            return classString;
        }

        public int GetTotal()
        {
            return Item.Sum(item => item.Price * (item.OptimalQuantity - item.CurrentQuantity));
        }
    }
}
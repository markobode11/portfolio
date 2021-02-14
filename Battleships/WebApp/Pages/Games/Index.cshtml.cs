using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<Game> Games { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Games = await _context.Games
                .Include(g => g.GameOption)
                .Include(g => g.Player1)
                .Include(g => g.Player2)
                .Include(g => g.GameOption)
                .ToListAsync();
        }
    }
}

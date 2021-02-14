using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Settings
{
    public class Index : PageModel
    {
        private readonly AppDbContext _context;

        public Index(AppDbContext context)
        {
            _context = context;
        }

        public IList<Boat> Boats { get; set; } = default!;

        [BindProperty] public List<GameOptionBoat> GameOptionBoats { get; set; } = default!;

        [BindProperty] public ENextMoveAfterHit NextMoveAfterHit { get; set; }
        [BindProperty] public EBoatsCanTouch BoatsCanTouch { get; set; }
        [Range(5, 20)] [BindProperty] public int X { get; set; }

        [Range(5, 20)] [BindProperty] public int Y { get; set; }


        public async Task OnGetAsync()
        {
            Boats = await _context.Boats.ToListAsync();
            GameOptionBoats = new List<GameOptionBoat>();
            foreach (var boat in Boats)
                GameOptionBoats.Add(new GameOptionBoat
                {
                    Boat = boat
                });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Boats = await _context.Boats.ToListAsync();

            foreach (var boat in GameOptionBoats)
            {
                var current = Boats.First(x => x.BoatId == boat.BoatId);
                boat.Boat = current;
            }

            if (!ModelState.IsValid) return Page();

            var brain = new BattleshipsBrain();
            brain.SetGame(GameOptionBoats, BoatsCanTouch, NextMoveAfterHit, X, Y);
            var gameId = brain.SaveGame("default name");

            return Redirect("/SetNames/Index?GameId=" + gameId);
        }
    }
}
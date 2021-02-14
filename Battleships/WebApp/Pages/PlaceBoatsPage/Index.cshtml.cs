using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.PlaceBoatsPage
{
    public class Index : PageModel
    {
        public Boat Boat { get; set; } = default!;
        public BoardSquareState[,] StaticBoard { get; set; } = default!;
        [BindProperty] public int BoatSize { get; set; }

        [BindProperty(SupportsGet = true)] public int BoatsPlaced { get; set; }
        [BindProperty(SupportsGet = true)] public int GameId { get; set; }

        [RegularExpression(@"^[A-Za-z]{1}([0-9]{1,2})", ErrorMessage = "Invalid start coordinates!")]
        [BindProperty]
        [StringLength(3)]
        [Display(Name = "Start coordinates")]
        public string StartCoordinates { get; set; } = default!;

        [RegularExpression(@"^[A-Za-z]{1}([0-9]{1,2})", ErrorMessage = "Invalid end coordinates!")]
        [BindProperty]
        [StringLength(3)]
        [Display(Name = "End coordinates")]
        public string EndCoordinates { get; set; } = default!;
        [BindProperty(SupportsGet = true)] public bool P1Turn { get; set; } = false;


        public async Task OnGetAsync()
        {
            await SetUpInfo();
        }

        public async Task<IActionResult> OnPostAsync(bool? isRandom)
        {
            var brain = new BattleshipsBrain();
            var currentGame = await brain.GetGame(GameId);
            brain.SetGameFromDb(currentGame);
            brain.NextMoveByPlayer1 = P1Turn;

            if (isRandom.HasValue)
            {
                brain.PlaceBoatsRandomly();
                brain.UpdateGame();
                return RedirectToCorrect();
            }

            if (!ModelState.IsValid)
            {
                await SetUpInfo();
                return Page();
            }

            var start = new Tuple<char, int>(char.ToUpper(StartCoordinates[0]), int.Parse(StartCoordinates.Substring(1)));
            var end = new Tuple<char, int>(char.ToUpper(EndCoordinates[0]), int.Parse(EndCoordinates.Substring(1)));
            if (brain.TryToPlaceBoat(start, end, BoatSize))
            {
                brain.PlaceTheBoat(start, end, BoatSize);
                brain.NextMoveByPlayer1 = true;
                brain.UpdateGame();
            }
            else
            {
                ModelState.AddModelError("invalid", "Invalid coordinates for the boat!");
                await SetUpInfo();
                return Page();
            }


            var boatList = new List<Boat>();

            foreach (var optionBoat in currentGame.GameOption!.GameOptionBoats)
            {
                for (int i = 0; i < optionBoat.Amount; i++)
                {
                    boatList.Add(optionBoat.Boat!);
                }
            }

            if (boatList.Count == BoatsPlaced + 1)
            {
                return RedirectToCorrect();
            }

            return Redirect($"/PlaceBoatsPage/Index?GameId={GameId}&BoatsPlaced={BoatsPlaced + 1}&P1Turn={P1Turn}");
        }

        private async Task SetUpInfo()
        {
            var brain = new BattleshipsBrain();
            var currentGame = await brain.GetGame(GameId);
            brain.SetGameFromDb(currentGame);

            StaticBoard = P1Turn ? brain.Player1Board : brain.Player2Board;

            var boats = currentGame.GameOption!.GameOptionBoats;
            var sum = BoatsPlaced;
            foreach (var optionBoat in boats)
            {
                if (sum < optionBoat.Amount)
                {
                    Boat = optionBoat.Boat!;
                    break;
                }

                sum -= optionBoat.Amount;
            }
        }

        private RedirectResult RedirectToCorrect()
        {
            return !P1Turn
                ? Redirect("/SwitchPlayers/Index?GameId=" + GameId)
                : Redirect($"/SwitchPlayers/Index?GameId={GameId}&BoatPlace=true");
        }
    }
}
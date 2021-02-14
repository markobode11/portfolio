using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.SwitchPlayers
{
    public class Index : PageModel
    {
        [BindProperty(SupportsGet = true)] public int GameId { get; set; }
        public string PageToGoTo { get; set; } = default!;
        public string? Message { get; set; }

        public void OnGet(bool? boatPlace, MoveResult? moveResult)
        {
            PageToGoTo = boatPlace.HasValue
                ? "/PlaceBoatsPage/Index"
                : "/BoardPage/Index";

            if (moveResult.HasValue)
            {
                Message = "Last move's result: " + moveResult;
            }
        }
    }
}
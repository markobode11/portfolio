using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.ModeSelect
{
    public class Index : PageModel
    {
        public IActionResult OnPost(int userChoice)
        {
            var brain = new BattleshipsBrain();

            switch (userChoice)
            {
                case 1:
                {
                    brain.GetClassicalGame();
                    int gameId = brain.SaveGame("classical game");
                    return Redirect("/SetNames/Index?GameId=" + gameId);
                }
                case 2:
                {
                    brain.GetSmallGame();
                    int gameId = brain.SaveGame("small game");
                    return Redirect("/SetNames/Index?GameId=" + gameId);
                }
                default:
                    return Redirect("/Settings/Index");
            }
        }
    }
}
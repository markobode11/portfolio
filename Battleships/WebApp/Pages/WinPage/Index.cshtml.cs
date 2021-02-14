using System.Threading.Tasks;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.WinPage
{
    public class Index : PageModel
    {
        public string Message { get; set; } = default!;
        public BoardSquareState[,] FirstBoard { get; set; } = default!;
        public BoardSquareState[,] SecondBoard { get; set; } = default!;

        public async Task OnGet(int gameId, int pId, bool tie)
        {
            var brain = new BattleshipsBrain();
            var currentGame = await brain.GetGame(gameId);
            brain.SetGameFromDb(currentGame);

            FirstBoard = currentGame.Player1.PlayerId == pId ? brain.Player1Board : brain.Player2Board;
            SecondBoard = currentGame.Player1.PlayerId == pId ? brain.Player2Board : brain.Player1Board;

            if (tie)
            {
                Message = "Tie game!";
            }
            else
            {
                var winner = currentGame.Player1.PlayerId == pId ? currentGame.Player1 : currentGame.Player2;
                Message = "You won " + winner.Name;   
            }

        }
    }
}
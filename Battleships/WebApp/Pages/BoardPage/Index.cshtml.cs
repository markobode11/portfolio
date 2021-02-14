using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.BoardPage
{
    public class Index : PageModel
    {
        public string? LastMoveMessage { get; set; }
        public string NextMoveMessage { get; set; } = default!;
        public BoardSquareState[,] ClickableBoard { get; set; } = default!;
        public BoardSquareState[,] StaticBoard { get; set; } = default!;
        [BindProperty(SupportsGet = true)] public int GameId { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync(int? x, int? y)
        {
            var brain = new BattleshipsBrain();
            var currentGame = await brain.GetGame(GameId);
            brain.SetGameFromDb(currentGame);

            if (x.HasValue && y.HasValue)
            {
                var moveResult = brain.TryToMakeAMove((int) x, (int) y);

                brain.UpdateGame();

                if (moveResult != MoveResult.Invalid &&
                    (moveResult == MoveResult.Miss ||
                     currentGame.GameOption!.ENextMoveAfterHit == ENextMoveAfterHit.OtherPlayer))
                    return Redirect($"/SwitchPlayers/Index?GameId={GameId}&moveResult={moveResult}");

                LastMoveMessage = moveResult == MoveResult.Invalid ? "Invalid cell to bomb!" : moveResult.ToString();
            }

            if (brain.WinCheck())
            {
                var tie = false;
                brain.NextMoveByPlayer1 = !brain.NextMoveByPlayer1;
                if (brain.WinCheck()) tie = true;

                var winner = currentGame.Player1;
                if (brain.MoveHistory.Count != 0)
                    winner = brain.MoveHistory.Last().Item3 ? currentGame.Player1 : currentGame.Player2;

                return Redirect($"/WinPage/Index?gameId={GameId}&tie={tie}&pId=" + winner.PlayerId);
            }

            NextMoveMessage = "It's your turn ";
            NextMoveMessage += brain.NextMoveByPlayer1
                ? currentGame.Player1.Name
                : currentGame.Player2.Name;

            StaticBoard = brain.NextMoveByPlayer1 ? brain.Player1Board : brain.Player2Board;
            ClickableBoard = brain.HideBoats(brain.NextMoveByPlayer1 ? brain.Player2Board : brain.Player1Board);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var brain = new BattleshipsBrain();
            var currentGame = await brain.GetGame(GameId);
            brain.SetGameFromDb(currentGame);
            var prevMover = brain.NextMoveByPlayer1;
            brain.UndoMove();
            brain.UpdateGame();

            return Redirect(prevMover != brain.NextMoveByPlayer1
                ? $"/SwitchPlayers/Index?GameId={GameId}"
                : $"/BoardPage/Index?GameId={GameId}");
        }
    }
}
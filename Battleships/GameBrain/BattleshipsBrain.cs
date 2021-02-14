using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameBrain
{
    public class BattleshipsBrain
    {
        public BoardSquareState[,] CurrentBoard = null!;
        public Game? Game;
        public GameOption? GameOption;
        public ICollection<GameOptionBoat> GameOptionBoatsList = new List<GameOptionBoat>();
        public List<Tuple<int, int, bool>> MoveHistory = new();
        public bool NextMoveByPlayer1 = true;
        public BoardSquareState[,] Player1Board = null!;
        public BoardSquareState[,] Player2Board = null!;

        public DbContextOptions<AppDbContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(
                @"
                    Server=barrel.itcollege.ee,1533;
                    User Id=student;
                    Password=Student.Bad.password.0;
                    Database=mabode_battleship;
                    MultipleActiveResultSets=true;
                    "
            ).Options;
        }

        public void SetGame(
            List<GameOptionBoat> gameBoats,
            EBoatsCanTouch canTouch,
            ENextMoveAfterHit moveAfterHit,
            int x,
            int y)
        {
            SetBoardSize(y, x);
            GameOption = new GameOption
            {
                Name = DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString(),
                EBoatsCanTouch = canTouch,
                ENextMoveAfterHit = moveAfterHit
            };
            GameOptionBoatsList = gameBoats;
        }

        public void GetSmallGame()
        {
            Player1Board = new BoardSquareState[5, 5];
            Player2Board = new BoardSquareState[5, 5];
            FillBoard(Player1Board);
            FillBoard(Player2Board);
            CurrentBoard = Player2Board;
            GameOption = new GameOption
            {
                Name = DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString(),
                EBoatsCanTouch = EBoatsCanTouch.No,
                ENextMoveAfterHit = ENextMoveAfterHit.SamePlayer
            };
            using var dbContext = new AppDbContext(GetDbOptions());
            foreach (var boat in dbContext.Boats)
                if (boat.Name == "Destroyer")
                    GameOptionBoatsList.Add(new GameOptionBoat
                    {
                        Boat = boat,
                        Amount = 1
                    });
        }

        public void GetClassicalGame()
        {
            Player1Board = new BoardSquareState[10, 10];
            Player2Board = new BoardSquareState[10, 10];
            FillBoard(Player1Board);
            FillBoard(Player2Board);
            CurrentBoard = Player2Board;
            GameOption = new GameOption
            {
                Name = DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString(),
                EBoatsCanTouch = EBoatsCanTouch.No,
                ENextMoveAfterHit = ENextMoveAfterHit.SamePlayer
            };
            using var dbContext = new AppDbContext(GetDbOptions());
            foreach (var boat in dbContext.Boats)
            {
                var amount = 6 - boat.Size;
                if (amount > 2) amount = 2;
                GameOptionBoatsList.Add(new GameOptionBoat
                {
                    Boat = boat,
                    Amount = amount
                });
            }
        }

        public void PlaceTheBoat(Tuple<char, int> start, Tuple<char, int> end, int boatId)
        {
            var (startX, startY) = start;
            var (endX, endY) = end;

            var xStartInt = ConvertCharToInt(startX);
            var xEndInt = ConvertCharToInt(endX);

            if (startY > endY)
            {
                endY += startY;
                startY = endY - startY;
                endY -= startY;
            }

            if (xStartInt > xEndInt)
            {
                xEndInt += xStartInt;
                xStartInt = xEndInt - xStartInt;
                xEndInt -= xStartInt;
            }


            if (xStartInt == xEndInt)
                for (var i = 0; i < Math.Abs(endY - startY) + 1; i++)
                    CurrentBoard![startY - 1 + i, xStartInt].BoatId = boatId;
            else if (startY == endY)
                for (var i = 0; i < Math.Abs(xEndInt - xStartInt) + 1; i++)
                    CurrentBoard![startY - 1, xStartInt + i].BoatId = boatId;

            if (NextMoveByPlayer1)
                Player1Board = CurrentBoard;
            else
                Player2Board = CurrentBoard;
        }

        private Tuple<int, int> ChangeValues(int value1, int value2)
        {
            value1 += value2;
            value2 = value1 - value2;
            value1 -= value2;
            return new Tuple<int, int>(value1, value2);
        }

        public bool TryToPlaceBoat(Tuple<char, int> start, Tuple<char, int> end, int size)
        {
            CurrentBoard = NextMoveByPlayer1 ? Player1Board : Player2Board;
            var (startX, startY) = start;
            var (endX, endY) = end;

            var xStartInt = ConvertCharToInt(startX);
            var xEndInt = ConvertCharToInt(endX);

            if (startY > endY) (endY, startY) = ChangeValues(endY, startY);

            if (xStartInt > xEndInt) (xEndInt, xStartInt) = ChangeValues(xEndInt, xStartInt);

            if (xStartInt != xEndInt && startY != endY) // cant put boats diagonally
                return false;

            if (xStartInt == xEndInt)
            {
                if (endY - startY + 1 != size) return false;

                for (var i = 0; i < endY - startY + 1; i++)
                    if (!CheckCellSurroundings(startY - 1 + i, xStartInt))
                        return false;
            }
            else if (startY == endY)
            {
                if (xEndInt - xStartInt + 1 != size) return false;

                for (var i = 0; i < xEndInt - xStartInt + 1; i++)
                    if (!CheckCellSurroundings(startY - 1, xStartInt + i))
                        return false;
            }

            return true;
        }

        public void PlaceBoatsRandomly()
        {
            CurrentBoard = NextMoveByPlayer1 ? Player1Board : Player2Board;
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXY".ToCharArray();

            var random = new Random();


            foreach (var optionBoat in GameOptionBoatsList.OrderByDescending(x => x.Boat!.Size))
                for (var i = 0; i < optionBoat.Amount; i++)
                {
                    var size = optionBoat.Boat!.Size;

                    while (true)
                    {
                        var x = random.Next(0, CurrentBoard.GetLength(1) - size + 1);
                        var y = random.Next(1, CurrentBoard.GetLength(0) - size + 1);

                        while (CurrentBoard[y, x].BoatId != null)
                        {
                            x = random.Next(0, CurrentBoard.GetLength(1) - size);
                            y = random.Next(1, CurrentBoard.GetLength(0) - size + 1);
                        }

                        Tuple<char, int> start = new(alphabet[x], y + 1);

                        var end = new Tuple<char, int>(alphabet[x + size - 1], y + 1);

                        if (TryToPlaceBoat(start, end, size))
                        {
                            PlaceTheBoat(start, end, size);
                            break;
                        }

                        end = new Tuple<char, int>(alphabet[x], y + size);
                        if (TryToPlaceBoat(start, end, size))
                        {
                            PlaceTheBoat(start, end, size);
                            break;
                        }
                    }
                }
        }

        private bool CheckCellSurroundings(int y, int x)
        {
            return GameOption!.EBoatsCanTouch switch
            {
                EBoatsCanTouch.No => CheckSides(y, x) && CheckDiagonals(y, x),
                EBoatsCanTouch.Yes => CurrentBoard![y, x].ToString() == " ",
                EBoatsCanTouch.Corner => CheckSides(y, x),
                _ => true // should never get here
            };
        }

        private bool CheckDiagonals(int y, int x)
        {
            if (y != CurrentBoard!.GetLength(0) - 1 && x != CurrentBoard.GetLength(1) - 1)
                if (CurrentBoard![y + 1, x + 1].ToString() != " ")
                    return false;

            if (y != CurrentBoard!.GetLength(0) - 1 && x != 0)
                if (CurrentBoard![y + 1, x - 1].ToString() != " ")
                    return false;

            if (y != 0 && x != CurrentBoard.GetLength(1) - 1)
                if (CurrentBoard![y - 1, x + 1].ToString() != " ")
                    return false;

            if (y != 0 && x != 0)
                if (CurrentBoard![y - 1, x - 1].ToString() != " ")
                    return false;

            return true;
        }

        private bool CheckSides(int y, int x)
        {
            if (CurrentBoard![y, x].ToString() != " ") return false;

            if (y != CurrentBoard.GetLength(0) - 1)
                if (CurrentBoard![y + 1, x].ToString() != " ")
                    return false;

            if (y != 0)
                if (CurrentBoard![y - 1, x].ToString() != " ")
                    return false;

            if (x != CurrentBoard.GetLength(1) - 1)
                if (CurrentBoard![y, x + 1].ToString() != " ")
                    return false;

            if (x != 0)
                if (CurrentBoard![y, x - 1].ToString() != " ")
                    return false;

            return true;
        }

        private void SetBoardSize(int row, int column)
        {
            Player1Board = new BoardSquareState[row, column];
            Player2Board = new BoardSquareState[row, column];
            FillBoard(Player1Board);
            FillBoard(Player2Board);
            CurrentBoard = Player2Board;
        }

        private void FillBoard(BoardSquareState[,] board)
        {
            for (var i = 0; i < board.GetUpperBound(0) + 1; i++)
            for (var j = 0; j < board.GetUpperBound(1) + 1; j++)
                board[i, j] = new BoardSquareState();
        }

        public int ConvertCharToInt(char x)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXY";
            var counter = 0;
            foreach (var letter in alphabet)
            {
                if (letter == x) break;

                counter++;
            }

            return counter;
        }

        public MoveResult TryToMakeAMove(int x, int y)
        {
            if (CurrentBoard![y, x].Bomb) return MoveResult.Invalid;

            MoveHistory.Add(new Tuple<int, int, bool>(x, y, NextMoveByPlayer1));

            var result = MoveResult.Invalid;

            if (CurrentBoard![y, x].ToString() == " ")
            {
                CurrentBoard[y, x].Bomb = true;
                result = MoveResult.Miss;
            }

            if (CurrentBoard![y, x].ToString() == "B")
            {
                CurrentBoard![y, x].Bomb = true;
                result = CheckIfSunken(y, x) ? MoveResult.Sunken : MoveResult.Hit;

                if (GameOption!.ENextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                {
                    if (NextMoveByPlayer1)
                        Player2Board = CurrentBoard;
                    else
                        Player1Board = CurrentBoard;

                    return result;
                }
            }

            ChangeCurrentPlayer();

            return result;
        }

        private void ChangeCurrentPlayer()
        {
            if (NextMoveByPlayer1)
            {
                Player2Board = CurrentBoard;
                CurrentBoard = Player1Board;
            }
            else
            {
                Player1Board = CurrentBoard;
                CurrentBoard = Player2Board;
            }

            NextMoveByPlayer1 = !NextMoveByPlayer1;
        }

        private bool CheckIfSunken(int y, int x)
        {
            var id = CurrentBoard![y, x].BoatId;

            for (var i = 0; i < CurrentBoard.GetLength(0); i++)
            for (var j = 0; j < CurrentBoard.GetLength(1); j++)
            {
                if (i == x && j == y) continue;

                if (CurrentBoard![j, i].BoatId == id && !CurrentBoard![j, i].Bomb) return false;
            }

            return true;
        }

        public bool WinCheck()
        {
            BoardSquareState[,] boardToCheck = NextMoveByPlayer1 ? Player2Board : Player1Board;
            for (var i = 0; i < boardToCheck.GetLength(1); i++)
            for (var j = 0; j < boardToCheck.GetLength(0); j++)
                if (boardToCheck[j, i].BoatId != null && !boardToCheck[j, i].Bomb)
                    return false;

            return true;
        }

        public bool UndoMove()
        {
            if (MoveHistory.Count == 0) return false;

            var lastMove = MoveHistory.Last();
            MoveHistory.Remove(lastMove);

            if (GameOption!.ENextMoveAfterHit == ENextMoveAfterHit.OtherPlayer ||
                lastMove.Item3 != NextMoveByPlayer1)
                ChangeCurrentPlayer();

            CurrentBoard[lastMove.Item2, lastMove.Item1] = GetPreviousSquareState(lastMove.Item1, lastMove.Item2);

            if (NextMoveByPlayer1)
                Player2Board = CurrentBoard;
            else
                Player1Board = CurrentBoard;

            return true;
        }

        private BoardSquareState GetPreviousSquareState(int x, int y)
        {
            BoardSquareState currentState = CurrentBoard[y, x];

            currentState.Bomb = false;
            return currentState;
        }

        private BoardSquareState[][] GetStateBoard(BoardSquareState[,] board)
        {
            BoardSquareState[][] stateBoard = new BoardSquareState[board.GetLength(0)][];
            for (var i = 0; i < stateBoard.Length; i++) stateBoard[i] = new BoardSquareState[board.GetLength(1)];

            for (var x = 0; x < board.GetLength(1); x++)
            for (var y = 0; y < board.GetLength(0); y++)
                stateBoard[y][x] = board[y, x];

            return stateBoard;
        }

        public void SetGameFromDb(Game savedGame)
        {
            var player1SavedBoard = JsonSerializer.Deserialize<BoardSquareState[][]>
                (savedGame.Player1BoardState!.PlayerBoardState);

            var player2SavedBoard = JsonSerializer.Deserialize<BoardSquareState[][]>
                (savedGame.Player2BoardState!.PlayerBoardState);

            var height = player1SavedBoard!.Length;
            var width = player1SavedBoard[0].Length;

            NextMoveByPlayer1 = savedGame.Player1.PlayerTurn;
            Player1Board = new BoardSquareState[height, width];
            Player2Board = new BoardSquareState[height, width];
            CurrentBoard = NextMoveByPlayer1 ? Player2Board : Player1Board;

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                Player1Board[y, x] = player1SavedBoard[y][x];
                Player2Board[y, x] = player2SavedBoard![y][x];
            }

            if (savedGame.History != null)
                MoveHistory = JsonSerializer.Deserialize<List<Tuple<int, int, bool>>>(savedGame.History)!;

            GameOption = savedGame.GameOption;
            GameOptionBoatsList = savedGame.GameOption!.GameOptionBoats;
            Game = savedGame;
        }

        public BoardSquareState[,] HideBoats(BoardSquareState[,] board)
        {
            BoardSquareState[,] newBoard =
                new BoardSquareState[board.GetUpperBound(0) + 1, board.GetUpperBound(1) + 1];

            for (var i = 0; i < board.GetUpperBound(0) + 1; i++)
            for (var j = 0; j < board.GetUpperBound(1) + 1; j++)
                if (board[i, j].ToString() == "B")
                    newBoard[i, j] = new BoardSquareState();
                else
                    newBoard[i, j] = board[i, j];

            return newBoard;
        }

        public void UpdateGame()
        {
            using var dbContext = new AppDbContext(GetDbOptions());

            var player1SerializedBoard = JsonSerializer.Serialize(
                GetStateBoard(Player1Board!));

            var player2SerializedBoard = JsonSerializer.Serialize(
                GetStateBoard(Player2Board!));

            Game!.Player1BoardState!.PlayerBoardState = player1SerializedBoard;
            Game.Player2BoardState!.PlayerBoardState = player2SerializedBoard;

            Game.Player1.PlayerTurn = NextMoveByPlayer1;
            Game.Player2.PlayerTurn = !NextMoveByPlayer1;

            Game.History = JsonSerializer.Serialize(MoveHistory);

            dbContext.Update(Game);
            dbContext.SaveChanges();
        }

        public int SaveGame(string saveName)
        {
            using var dbContext = new AppDbContext(GetDbOptions());

            var player1SerializedBoard = JsonSerializer.Serialize(
                GetStateBoard(Player1Board!));
            var player2SerializedBoard = JsonSerializer.Serialize(
                GetStateBoard(Player2Board!));

            var player1 = new Player
            {
                Name = "Player1",
                PlayerTurn = NextMoveByPlayer1,
                EPlayerType = EPlayerType.Human
            };
            var player2 = new Player
            {
                Name = "Player2",
                PlayerTurn = !NextMoveByPlayer1,
                EPlayerType = EPlayerType.Human
            };


            var boardState1 = new BoardState
            {
                Player = player1,
                PlayerBoardState = player1SerializedBoard
            };
            var boardState2 = new BoardState
            {
                Player = player2,
                PlayerBoardState = player2SerializedBoard
            };

            var game = new Game
            {
                Description = saveName,
                GameOption = GameOption!,
                Player1 = player1,
                Player2 = player2,
                Player1BoardState = boardState1,
                Player2BoardState = boardState2
            };

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            foreach (var gameOptionBoat in GameOptionBoatsList)
                dbContext.GameOptionBoats.Add(new GameOptionBoat
                {
                    Amount = gameOptionBoat.Amount,
                    BoatId = gameOptionBoat.Boat!.BoatId,
                    GameOptionId = game.GameOption.GameOptionId
                });


            dbContext.SaveChanges();
            return game.GameId;
        }

        public async Task<Game> GetGame(int gameId)
        {
            await using var dbContext = new AppDbContext(GetDbOptions());
            var dbGame = await dbContext.Games.Where(game => game.GameId == gameId)
                .Include(game => game.Player1)
                .Include(game => game.Player2)
                .Include(game => game.Player1BoardState)
                .Include(game => game.Player2BoardState)
                .Include(game => game.GameOption)
                .ThenInclude(gameOption => gameOption!.GameOptionBoats)
                .ThenInclude(optionBoat => optionBoat.Boat)
                .FirstAsync();
            return dbGame;
        }
    }
}
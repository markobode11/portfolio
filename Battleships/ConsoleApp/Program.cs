using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using GameConsoleUI;
using MenuSystem;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    internal static class Program
    {
        private static BattleshipsBrain? _brain;
        private static void Main()
        {
            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==========> Battleships <==========");
            Console.ResetColor();

            var gameMenu = new Menu(MenuLevel.Level2);

            gameMenu.AddMenuItem(new MenuItem("Make a move", "1", MoveAction));
            gameMenu.AddMenuItem(new MenuItem("Undo previous move", "2", UndoAction));
            gameMenu.AddMenuItem(new MenuItem("Return to main menu", "m", BattleshipsUi.ReturnToMainAction));
            gameMenu.AddMenuItem(new MenuItem("Save game", "s", SaveGame));
            gameMenu.AddMenuItem(new MenuItem("Exit", "x", BattleshipsUi.ExitMenuAction));

            var startGameMenu = new Menu(MenuLevel.Level1);

            startGameMenu.AddMenuItem(
                new MenuItem("Classic 10x10 game", "s", StartGameClassic, gameMenu.RunMenu));
            startGameMenu.AddMenuItem(
                new MenuItem("Custom rules", "p", StartGame, gameMenu.RunMenu));
            startGameMenu.AddMenuItem(new MenuItem("Return to main menu", "m", BattleshipsUi.ReturnToMainAction));
            startGameMenu.AddMenuItem(new MenuItem("Exit", "x", BattleshipsUi.ExitMenuAction));

            var menu = new Menu(MenuLevel.Level0);

            menu.AddMenuItem(new MenuItem("Start new game", "1", startGameMenu.RunMenu));
            menu.AddMenuItem(new MenuItem("Load game", "l", LoadGame, gameMenu.RunMenu));
            menu.AddMenuItem(new MenuItem("Exit", "x", BattleshipsUi.ExitMenuAction));
            
            menu.RunMenu();
        }

        private static void LoadGame()
        {
            _brain = new BattleshipsBrain();
            List<Game> saves = GetDbSaves();
            BattleshipsUi.DisplaySaves(saves);
            if (saves.Count == 0) return;
            Game userSave = BattleshipsUi.GetUserSave(saves);
            _brain.SetGameFromDb(userSave);

            BattleshipsUi.DrawBoardsNextToEachOther(_brain);
        }

        private static void SaveGame()
        {
            if (_brain!.Game != null)
            {
                _brain.UpdateGame();
                BattleshipsUi.DisplayInfo("Game updated!");
            }
            else
            {
                string saveName = BattleshipsUi.GetSaveName(_brain.GameOption!.Name);
                _brain.SaveGame(saveName);
                BattleshipsUi.DisplayInfo("Game saved!");
            }
        }

        private static string MoveAction()
        {
            var currentMover = _brain!.NextMoveByPlayer1;
            while (true)
            {
                var (x, y) = BattleshipsUi.GetCoordinates("Give bomb coordinates: ", _brain.CurrentBoard);
                var xInt = _brain.ConvertCharToInt(x);

                var moveResult = _brain.TryToMakeAMove(xInt, y - 1);
                if (moveResult == MoveResult.Invalid)
                {
                    BattleshipsUi.DisplayError("Given square is already bombed! \n Choose new cell!");
                    continue;
                }

                if (_brain.WinCheck())
                {
                    BattleshipsUi.DisplayWin(currentMover);
                    return "m";
                }

                if (moveResult == MoveResult.Hit &&
                    _brain.GameOption!.ENextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    BattleshipsUi.DrawBoardsNextToEachOther(_brain, moveResult: moveResult, changePlayer: false);
                else
                    BattleshipsUi.DrawBoardsNextToEachOther(_brain, moveResult: moveResult);


                return "";
            }
        }

        private static void UndoAction()
        {
            if (!_brain!.UndoMove())
                BattleshipsUi.DisplayError("No moves done!");
            else
                BattleshipsUi.DrawBoardsNextToEachOther(_brain);
        }

        private static void StartGame()
        {
            _brain = new BattleshipsBrain();
            var (y, x) = BattleshipsUi.GetBoardSize();
            var canTouch = BattleshipsUi.GetBoatsTouch();
            var nextMove = BattleshipsUi.GetNextMoveAfterHit();

            using var dbContext = new AppDbContext(_brain.GetDbOptions());

            var gameBoatList = new List<GameOptionBoat>();
            foreach (var boat in dbContext.Boats)
            {
                var amount = BattleshipsUi.GetGameOptionBoat(boat);
                gameBoatList.Add(new GameOptionBoat
                {
                    Boat = boat,
                    Amount = amount
                });
            }

            _brain.SetGame(gameBoatList, canTouch, nextMove, x, y);

            if (BattleshipsUi.RandomBoatPlacement())
            {
                _brain.PlaceBoatsRandomly();
            }
            else
            {
                BattleshipsUi.DrawBoardsNextToEachOther(_brain, false);

                PlaceBoats();

                _brain.NextMoveByPlayer1 = !_brain.NextMoveByPlayer1;
                BattleshipsUi.DrawBoardsNextToEachOther(_brain);

                PlaceBoats();

                _brain.NextMoveByPlayer1 = !_brain.NextMoveByPlayer1;
            }

            BattleshipsUi.DrawBoardsNextToEachOther(_brain);
        }

        private static void StartGameClassic()
        {
            _brain = new BattleshipsBrain();
            _brain.GetClassicalGame();

            if (BattleshipsUi.RandomBoatPlacement())
            {
                _brain.PlaceBoatsRandomly();
                _brain.NextMoveByPlayer1 = !_brain.NextMoveByPlayer1;
                _brain.PlaceBoatsRandomly();
            }
            else
            {
                BattleshipsUi.DrawBoardsNextToEachOther(_brain, false);

                PlaceBoats();

                _brain.NextMoveByPlayer1 = !_brain.NextMoveByPlayer1;
                BattleshipsUi.DrawBoardsNextToEachOther(_brain);

                PlaceBoats();
            }

            _brain.NextMoveByPlayer1 = !_brain.NextMoveByPlayer1;
            BattleshipsUi.DrawBoardsNextToEachOther(_brain);
        }

        private static void PlaceBoats()
        {
            if (!_brain!.GameOptionBoatsList.Any()) return;

            foreach (var optionBoat in _brain.GameOptionBoatsList)
                for (var i = 0; i < optionBoat.Amount; i++)
                    do
                    {
                        Tuple<char, int> start =
                            BattleshipsUi.GetCoordinates(
                                $"Give {optionBoat.Boat!.Name} ({optionBoat.Boat.Size}x1) start coordinates: ",
                                _brain.CurrentBoard);

                        Tuple<char, int> end = BattleshipsUi.GetCoordinates(
                            $"Give {optionBoat.Boat.Name} ({optionBoat.Boat.Size}x1) end coordinates: ",
                            _brain.CurrentBoard);

                        if (_brain.TryToPlaceBoat(start, end, optionBoat.Boat.Size))
                        {
                            _brain.PlaceTheBoat(start, end, optionBoat.Boat.BoatId);
                            BattleshipsUi.DrawBoardsNextToEachOther(_brain, false);
                            break;
                        }

                        BattleshipsUi.DisplayError("Invalid boat coordinates.");
                    } while (true);
        }

        private static List<Game> GetDbSaves()
        {
            using var dbContext = new AppDbContext(_brain!.GetDbOptions());
            return dbContext.Games
                .Include(game => game.Player1)
                .Include(game => game.Player2)
                .Include(game => game.Player1BoardState)
                .Include(game => game.Player2BoardState)
                .Include(game => game.GameOption)
                .ThenInclude(gameOption => gameOption!.GameOptionBoats)
                .ToList();
        }
    }
}
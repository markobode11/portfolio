using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Domain;
using Domain.Enums;
using GameBrain;

namespace GameConsoleUI

{
    public static class BattleshipsUi
    {
        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void DisplayMoveResult(MoveResult moveResult)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(moveResult + "!");
            Console.ResetColor();
        }

        public static void DisplayInfo(string info)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(info);
            Console.ResetColor();
        }

        public static void DisplayWin(bool player1Wins)
        {
            string winner = player1Wins ? "Player1" : "Player2";
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{winner} wins!");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void DisplaySaves(List<Game> saves)
        {
            if (!saves.Any())
            {
                Console.WriteLine("No saves available!");
                return;
            }

            var counter = 1;
            Console.WriteLine("Available saves:");
            foreach (var save in saves)
            {
                Console.WriteLine($"{counter}) {save.Description}");
                counter++;
            }
        }

        public static Game GetUserSave(List<Game> saves)
        {
            var userSave = new Game();
            do
            {
                Console.Write("Choose save:");
                var saveNo = Console.ReadLine();
                if (saveNo == null || !int.TryParse(saveNo.Trim(), out var saveNoInt))
                {
                    DisplayError("Save number has to be a number!");
                    continue;
                }

                if (saveNoInt > saves.Count() || saveNoInt < 1)
                {
                    DisplayError("No such save number available!");
                    continue;
                }

                var counter = 1;
                foreach (var save in saves)
                {
                    if (counter == saveNoInt)
                    {
                        userSave = save;
                        break;
                    }

                    counter++;
                }

                break;
            } while (true);

            return userSave;
        }


        private static void DrawBoard(BoardSquareState[,] board, int cursorLeft)
        {
            // add plus 1, since this is 0 based. length 0 is returned as -1;
            var width = board.GetUpperBound(1) + 1; // x
            var height = board.GetUpperBound(0) + 1; // y

            string spacer = "";
            string cells = "";

            for (var colIndex = 0; colIndex < width; colIndex++)
            {
                if (colIndex == width - 1)
                {
                    spacer += "-----";
                    break;
                }

                spacer += "----";
            }

            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXY";
            string spacerLetters = "   ";

            for (var colIndex = 0; colIndex < width; colIndex++) spacerLetters += "  " + alphabet[colIndex] + " ";

            SetCursorPos(cursorLeft);
            Console.WriteLine(spacerLetters);

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                SetCursorPos(cursorLeft);
                Console.WriteLine("   " + spacer);

                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    if (colIndex == width - 1)
                    {
                        cells += $"| {board[rowIndex, colIndex]} |";
                        break;
                    }

                    cells += $"| {board[rowIndex, colIndex]} ";
                }

                SetCursorPos(cursorLeft);
                if (rowIndex < 9)
                    Console.WriteLine($" {rowIndex + 1} " + cells + $" {rowIndex + 1}");
                else
                    Console.WriteLine($"{rowIndex + 1} " + cells + $" {rowIndex + 1}");

                cells = "";
            }

            SetCursorPos(cursorLeft);
            Console.WriteLine("   " + spacer);
            SetCursorPos(cursorLeft);
            Console.WriteLine(spacerLetters);
        }

        public static void DrawBoardsNextToEachOther(
            BattleshipsBrain brain,
            bool changePlayer = true,
            MoveResult moveResult = MoveResult.Invalid)
        {
            for (var i = 0; i < Console.WindowHeight; i++)
            {
                Console.WriteLine();
                if (changePlayer)
                    if (i == Console.WindowHeight / 2)
                    {
                        if (moveResult != MoveResult.Invalid)
                        {
                            DisplayMoveResult(moveResult);
                            Console.WriteLine();
                        }

                        var playerName = brain.NextMoveByPlayer1 ? "Player1" : "Player2";

                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine($"{playerName} moves next");
                        Console.WriteLine("You have 2 seconds to switch players!");
                        Console.ResetColor();
                    }
            }

            if (changePlayer) Thread.Sleep(2000);

            var cursorLeft1 = Console.WindowWidth / 2 - ((brain.Player1Board!.GetUpperBound(1) + 1) * 3 + 25);
            var cursorLeft2 = Console.WindowWidth / 2 + 15;

            var cursorTop = Console.CursorTop;

            Console.SetCursorPosition(cursorLeft1, cursorTop);
            Console.WriteLine("Your board");

            Console.SetCursorPosition(cursorLeft2, cursorTop);
            Console.WriteLine("Opponents board");

            if (brain.NextMoveByPlayer1)
            {
                DisplayInfo("It`s your turn Player1!");

                DrawBoard(brain.Player1Board, cursorLeft1);

                Console.SetCursorPosition(cursorLeft1, cursorTop + 2);

                DrawBoard(brain.HideBoats(brain.Player2Board!), cursorLeft2);
            }
            else
            {
                DisplayInfo("It`s your turn Player2!");

                DrawBoard(brain.Player2Board!, cursorLeft1);

                Console.SetCursorPosition(cursorLeft1, cursorTop + 2);

                DrawBoard(brain.HideBoats(brain.Player1Board), cursorLeft2);
            }
        }

        private static void SetCursorPos(int cursorLeft)
        {
            Console.SetCursorPosition(cursorLeft, Console.CursorTop);
        }

        public static Tuple<char, int> GetCoordinates(string var, BoardSquareState[,] board)
        {
            Tuple<char, int> tuple;
            Console.WriteLine(var);
            do
            {
                Console.Write("Give X(letter):");
                var x = Console.ReadLine();
                char xChar;
                const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXY";
                if (x != null && x.Trim().Length == 1)
                {
                    if (char.IsLetter(x.Trim().ToUpper()[0]))
                    {
                        xChar = x.Trim().ToUpper()[0];
                        if (alphabet.IndexOf(xChar) > board.GetUpperBound(1))
                        {
                            DisplayError("X too big!");
                            continue;
                        }
                    }
                    else
                    {
                        DisplayError("X has to be a letter!");
                        continue;
                    }
                }
                else
                {
                    DisplayError("X has to be one letter!");
                    continue;
                }

                do
                {
                    Console.Write("Give Y(number):");
                    var y = Console.ReadLine();
                    int number;
                    if (y != null && y.Length < 3)
                    {
                        if (!int.TryParse(y.Trim(), out number))
                        {
                            DisplayError("Y has to be a number!");
                            continue;
                        }
                    }
                    else
                    {
                        DisplayError("Y has to be a number!");
                        continue;
                    }

                    if (number > board.GetUpperBound(0) + 1)
                    {
                        DisplayError("Y to big!");
                        continue;
                    }

                    if (number < 1)
                    {
                        DisplayError("Y to small!");
                        continue;
                    }

                    tuple = new Tuple<char, int>(xChar, number);
                    break;
                } while (true);

                break;
            } while (true);

            return tuple;
        }

        public static EBoatsCanTouch GetBoatsTouch()
        {
            Console.WriteLine("Can boats touch?");
            do
            {
                Console.WriteLine("1) Yes");
                Console.WriteLine("2) No");
                Console.WriteLine("3) Corners can touch");
                Console.Write(">");
                string userChoice = Console.ReadLine() ?? "";
                switch (userChoice)
                {
                    case "1":
                        return EBoatsCanTouch.Yes;
                    case "2":
                        return EBoatsCanTouch.No;
                    case "3":
                        return EBoatsCanTouch.Corner;
                    default:
                        DisplayError("I do not have this option!");
                        continue;
                }
            } while (true);
        }


        public static ENextMoveAfterHit GetNextMoveAfterHit()
        {
            Console.WriteLine("Who moves after bomb hits the ship?");
            do
            {
                Console.WriteLine("1) Same player");
                Console.WriteLine("2) Other player");
                Console.Write(">");
                string userChoice = Console.ReadLine() ?? "";
                switch (userChoice)
                {
                    case "1":
                        return ENextMoveAfterHit.SamePlayer;
                    case "2":
                        return ENextMoveAfterHit.OtherPlayer;
                    default:
                        DisplayError("I do not have this option!");
                        continue;
                }
            } while (true);
        }

        public static Tuple<int, int> GetBoardSize()
        {
            var maxBoardSize = (int) GetMaxBoardSize();

            Console.WriteLine("Max board size is " + maxBoardSize + " x " + maxBoardSize);
            do
            {
                Console.Write($"Enter board width from 5 to {maxBoardSize} (both included):");
                var widthNumeric = int.TryParse(Console.ReadLine(), out var column);
                if (!widthNumeric)
                {
                    DisplayError("Please enter a number!");
                    continue;
                }

                if (column < 5 || column > maxBoardSize)
                {
                    DisplayError($"Entered value has to be between 5 and {maxBoardSize}, both included.");
                    continue;
                }

                do
                {
                    Console.Write($"Enter board height from 5 to {maxBoardSize} (both included):");
                    var heightNumeric = int.TryParse(Console.ReadLine(), out var row);

                    if (!heightNumeric)
                    {
                        DisplayError("Please enter a number!");
                        continue;
                    }

                    if (row < 5 || row > maxBoardSize)
                    {
                        DisplayError($"Entered value has to be between 5 and {maxBoardSize}, both included.");
                        continue;
                    }

                    return new Tuple<int, int>(row, column);
                } while (true);
            } while (true);
        }

        private static double GetMaxBoardSize()
        {
            var size = Math.Floor((Console.WindowWidth - 35) / 8.0 / 16 * 9);

            if (size > 25) size = 25;

            return size;
        }

        public static int GetGameOptionBoat(Boat boat)
        {
            Console.Write($"Choose the amount of {boat.Name} (Size: {boat.Size}x1):");
            do
            {
                var userChoice = Console.ReadLine() ?? "";
                if (!int.TryParse(userChoice, out var userChoiceInt))
                {
                    DisplayError("Amount of the ship has to be a number!");
                    continue;
                }

                if (userChoiceInt < 0 || userChoiceInt > 5)
                {
                    DisplayError("Min amount is 0 and max amount is 5!");
                    continue;
                }

                return userChoiceInt;
            } while (true);
        }

        public static string GetSaveName(string gameOptionName)
        {
            var defaultName = "Battleships save " + gameOptionName;
            Console.WriteLine($"Default save name is: {defaultName})");
            Console.Write("Press enter to save game with default name or type new save name:");
            var saveName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(saveName)) saveName = defaultName;

            return saveName;
        }

        public static void ExitMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("Closing down...");
            Console.ResetColor();
        }

        public static void ReturnToMainAction()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("Returning to main menu");
            Console.WriteLine();
            Console.ResetColor();
        }

        public static bool RandomBoatPlacement()
        {
            Console.WriteLine("Place boats randomly for both players?");
            Console.WriteLine("1) Yes");
            Console.WriteLine("2) No");
            while (true)
            {
                Console.Write(">");
                var userChoice = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userChoice))
                    DisplayError("Invalid input!");
                else if (userChoice.Trim() == "1")
                    return true;
                else if (userChoice == "2")
                    return false;
                else
                    DisplayError("Invalid input!");
            }
        }
    }
}
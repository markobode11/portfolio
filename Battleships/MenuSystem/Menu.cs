using System;
using System.Collections.Generic;

namespace MenuSystem
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2
    }

    public class Menu
    {
        private readonly MenuLevel _menuLevel;

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }

        private Dictionary<string, MenuItem> MenuItems { get; } = new();

        public void AddMenuItem(MenuItem item)
        {
            if (item.UserChoice == "") throw new ArgumentException("UserChoice can not be empty.");

            if (item.UserChoice.Length != 1) throw new ArgumentException("UserChoice must be 1 letter or number!");

            if (MenuItems.ContainsKey(item.UserChoice))
                throw new ArgumentException($"UserChoice {item.UserChoice} is already taken!");

            MenuItems.Add(item.UserChoice, item);
        }

        public string RunMenu()
        {
            string userChoice;
            do
            {
                userChoice = GetInput();
                if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    userChoice = _menuLevel switch
                    {
                        MenuLevel.Level0 => HandleLevel0(userChoice, userMenuItem),
                        MenuLevel.Level1 => HandleLevel1(userChoice, userMenuItem),
                        MenuLevel.Level2 => HandleLevel2(userChoice, userMenuItem),
                        _ => userChoice
                    };
                else
                    HandleWrongInput();
            } while (userChoice != "x" && userChoice != "m" && userChoice != "s");

            return userChoice;
        }

        private static string HandleLevel2(string userChoice, MenuItem userMenuItem)
        {
            switch (userChoice)
            {
                // make a move
                case "1":
                    userChoice = userMenuItem.MethodToExecute();
                    break;
                case "2": // undo
                case "s": // save
                    userMenuItem.MethodDependingOnMenuLevel();
                    break;
            }

            return userChoice;
        }

        private string HandleLevel1(string userChoice, MenuItem userMenuItem)
        {
            if (userChoice == "s" || userChoice == "p")
                //started classical game or own custom game
            {
                userMenuItem.MethodDependingOnMenuLevel();
                userChoice = userMenuItem.MethodToExecute();
            }

            if (userChoice == "m") MenuItems[userChoice].MethodDependingOnMenuLevel();

            return userChoice;
        }

        private string HandleLevel0(string userChoice, MenuItem userMenuItem)
        {
            if (userChoice == "1") // new game
                userChoice = userMenuItem.MethodToExecute();

            if (userChoice == "l") //load game
            {
                userMenuItem.MethodDependingOnMenuLevel();
                userChoice = userMenuItem.MethodToExecute();
            }

            if (userChoice == "m" || userChoice == "s") userChoice = "";

            if (userChoice == "x") //exit
                MenuItems[userChoice].MethodDependingOnMenuLevel();

            return userChoice;
        }

        private string GetInput()
        {
            foreach (var menuItem in MenuItems.Values) Console.WriteLine(menuItem);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(">");
            Console.ResetColor();

            var userChoice = Console.ReadLine()?.ToLower().Trim() ?? "Empty";

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                Console.WriteLine("Your choice was: " + userMenuItem);
            else
                Console.WriteLine("Your choice was: " + userChoice);

            Console.ResetColor();
            Console.WriteLine();
            return userChoice;
        }

        private void HandleWrongInput()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("I do not have this option!");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
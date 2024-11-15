using MathGame.Models;

namespace MathGame;

internal class Menu
{
    private GameEngine gameClass = new();

    internal void ShowMenu(string name, DateTime date)
    {
        Console.Clear();
        Console.WriteLine($"Hello {name}. It's {date}. This is a math game.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.WriteLine("\n");

        var gameRunning = true;

        while (gameRunning)
        {
            Console.Clear();
            Console.WriteLine(@$"What game would you like to play today? Choose from the option below:
V - View Previous Games
A - Addition
S - Subtraction
M - Multiplication
D - Division
R - Random
Q - Quit the program");
            Console.WriteLine("---------------------------------------------");

            var gameSelected = Console.ReadLine();

            switch (gameSelected.Trim().ToLower())
            {
                case "v":
                    Helpers.PrintGames();
                    break;
                case "a":
                    ChooseDifficulty(GameType.Addition);
                    break;
                case "s":
                    ChooseDifficulty(GameType.Subtraction);
                    break;
                case "m":
                    ChooseDifficulty(GameType.Multiplication);
                    break;
                case "d":
                    ChooseDifficulty(GameType.Division);
                    break;
                case "r":
                    ChooseDifficulty(GameType.Random);
                    break;
                case "q":
                    Console.WriteLine("Goodbye!");
                    gameRunning = false;
                    break;
                default:
                    Console.WriteLine("Please choose a valid option");
                    break;
            }
        }
        
    }

    internal void ChooseDifficulty(GameType gameType)
    {
        Console.Clear();
        Console.WriteLine($"""
                           Choose a difficulty:
                           E - Easy
                           N - Normal
                           H - Hard
                           """);
        Console.WriteLine("---------------------------------------------");
        
        var input = Console.ReadLine();
        input = input.Trim().ToLower();

        while (string.IsNullOrEmpty(input) || (input != "e" && input != "n" && input != "h"))
        {
            Console.WriteLine("Please choose a valid difficulty");
            input = Console.ReadLine();
        }

        Difficulty difficulty =  input switch
        {
            "e" => Difficulty.Easy,
            "n" => Difficulty.Normal,
            "h" => Difficulty.Hard,
            _ => throw new ArgumentException("Invalid difficulty.")
        };
        
        gameClass.Game(gameType, difficulty);
    }
}
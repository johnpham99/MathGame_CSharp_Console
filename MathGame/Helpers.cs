using MathGame.Models;

namespace MathGame;

internal class Helpers
{
    internal static List<GameInfo> gameHistory = new List<GameInfo>
    {
        new GameInfo { Date = DateTime.Now.AddDays(1), Difficulty = Difficulty.Easy, Type = GameType.Addition, Score = 5, SecondsElapsed = 60},
        new GameInfo { Date = DateTime.Now.AddDays(2), Difficulty = Difficulty.Normal, Type = GameType.Multiplication, Score = 4, SecondsElapsed = 60},
        new GameInfo { Date = DateTime.Now.AddDays(3), Difficulty = Difficulty.Normal, Type = GameType.Division, Score = 4, SecondsElapsed = 60},
        new GameInfo { Date = DateTime.Now.AddDays(4), Difficulty = Difficulty.Normal, Type = GameType.Subtraction, Score = 3, SecondsElapsed = 60},
    };

    internal static int[] GetDivisionNumbers()
    {
        var random = new Random();
        var value1 = random.Next(1, 101);
        var value2 = random.Next(1, 101);

        var result = new int[2];

        while (value1 % value2 != 0)
        {
            value1 = random.Next(1, 101);
            value2 = random.Next(1, 101);
        }
        
        result[0] = value1;
        result[1] = value2;

        return result;
    }

    internal static void AddToHistory(int gameScore, GameType gameType, Difficulty difficulty, double secondsElapsed)
    {
        gameHistory.Add(new GameInfo
        {
            Date = DateTime.Now, 
            Score = gameScore, 
            Type = gameType,
            Difficulty = difficulty,
            SecondsElapsed = secondsElapsed
        });
    }

    internal static void PrintGames()
    {
        Console.Clear();
        Console.WriteLine("Games History");
        Console.WriteLine("---------------------------");
        foreach (var game in gameHistory)
        {
            Console.WriteLine($"{game.Date} - ({game.Difficulty}) {game.Type}: {game.Score}pts - {game.SecondsElapsed} seconds");
        }
        Console.WriteLine("---------------------------\n");
        Console.WriteLine("Press any key to return to Main Menu");
        Console.ReadKey();
    }

    internal static string ValidateInput(string? input)
    {
        while (string.IsNullOrEmpty(input) || !Int32.TryParse(input, out _))
        {
            Console.WriteLine("Valid number not given.");
            input = Console.ReadLine();
        }

        return input;
    }

    internal static string GetName()
    {
        Console.Write("Please enter your name: ");
        var name = Console.ReadLine();

        while (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name can't be empty");
            name = Console.ReadLine();
        }

        return name;
    }

    internal static int[] GetNumbers(GameType gameType, Difficulty difficulty)
    {
        int[] numbers = new int[2];
        var random = new Random();

        int normalUpperBound = difficulty switch
        {
            Difficulty.Easy => random.Next(1, 11),
            Difficulty.Normal => random.Next(1, 51),
            Difficulty.Hard => random.Next(1, 101),
            _ => random.Next(1, 101),
        };
        
        int divisionUpperBound = difficulty switch
        {
            Difficulty.Easy => random.Next(1, 101),
            Difficulty.Normal => random.Next(1, 501),
            Difficulty.Hard => random.Next(1, 1001),
            _ => random.Next(1, 101),
        };
        
        numbers[0] = random.Next(1, normalUpperBound);
        numbers[1] = random.Next(1, normalUpperBound);
        
        switch (gameType)
        {
            case GameType.Addition:
                break;
            case GameType.Subtraction:
                while (numbers[1] > numbers[0])
                {
                    numbers[0] = random.Next(1, normalUpperBound);
                    numbers[1] = random.Next(1, normalUpperBound);
                }
                break;
            case GameType.Multiplication:
                break;
            case GameType.Division:
                do
                {
                    numbers[0] = random.Next(1, divisionUpperBound);
                    numbers[1] = random.Next(1, divisionUpperBound);         
                }
                while (numbers[0] % numbers[1] != 0);
                break;
            default:
                throw new ArgumentException("Invalid game type");
        }

        return numbers;
    }
}
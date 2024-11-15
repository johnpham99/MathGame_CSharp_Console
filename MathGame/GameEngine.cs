using System.Diagnostics;
using MathGame.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;

namespace MathGame;

internal class GameEngine
{
 internal async Task Game(GameType gameType, Difficulty difficulty)
 {
  
  // Load Configuration and setup Azure Speech
  var config = new ConfigurationBuilder()
   .SetBasePath(AppContext.BaseDirectory)
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .Build();

  Console.WriteLine($"This is a change.");
  var subscriptionKey = config["AzureSpeech:SubscriptionKey"];
  var region = config["AzureSpeech:Region"];

  var speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
  speechConfig.SpeechRecognitionLanguage = "en-US";

  using var recognizer = new SpeechRecognizer(speechConfig);
  
  // Game 
  var score = 0;
  var random = new Random();
  bool randomGame = gameType == GameType.Random;

  var stopwatch = Stopwatch.StartNew();
  for (int i = 0; i < 5; i++)
  {
   if (randomGame)
   {
    gameType = (GameType)random.Next(0, 4);
   }
   
   var numbers = Helpers.GetNumbers(gameType, difficulty);
   var value1 = numbers[0];
   var value2 = numbers[1];
   
   string operatorString = gameType switch
   {
    GameType.Addition => "+",
    GameType.Subtraction => "-",
    GameType.Multiplication => "*",
    GameType.Division => "/",
    _ => throw new ArgumentException($"Invalid game type {gameType}.")
   };

   int correctAnswer = gameType switch
   {
    GameType.Addition => value1 + value2,
    GameType.Subtraction => value1 - value2,
    GameType.Multiplication => value1 * value2,
    GameType.Division => value1 / value2,
    _ => throw new ArgumentException($"Invalid game type {gameType}.")
   };
   
   Console.Clear();
   Console.WriteLine($"{value1} {operatorString} {value2} = ");
   
   using var cts = new CancellationTokenSource();
   var speechTask = ListenForSpeechAsync(recognizer, cts.Token);
   var consoleTask = MonitorConsoleInputAsync(cts.Token);
   
   var completedTask = await Task.WhenAny(speechTask, consoleTask);
   cts.Cancel();

   var input = completedTask == speechTask ? await speechTask : await consoleTask;
   if (completedTask == speechTask)
   {
    input = input.Substring(0, input.Length - 1);
   }
   input = Helpers.ValidateInput(input);
   if (int.Parse(input) == correctAnswer)
   {
    Console.WriteLine("Correct! Press any key for the next question.");
    score++;
   }
   else
   {
    Console.WriteLine($"Incorrect. The correct answer was {correctAnswer}. Press any key for the next question.");
   }
   
   if (i == 4)
   {
    stopwatch.Stop();
   }
   
   Console.ReadKey();
  }
  Console.Clear();
  Console.WriteLine($"Game over. Your final score is {score}. This took {stopwatch.Elapsed.TotalSeconds} seconds.");
  Console.WriteLine("Press any key to return to the main menu.");
  Console.ReadKey();
  Helpers.AddToHistory(score, randomGame ? GameType.Random : gameType, difficulty, stopwatch.Elapsed.TotalSeconds);
 }

 private static async Task<string> ListenForSpeechAsync(SpeechRecognizer recognizer,
  CancellationToken token)
 {
  var tcs = new TaskCompletionSource<string>();

  recognizer.Recognized += (s, e) =>
  {
   if (e.Result.Reason == ResultReason.RecognizedSpeech)
   {
    tcs.TrySetResult(e.Result.Text);
   }
   else if (e.Result.Reason == ResultReason.NoMatch)
   {
    tcs.TrySetResult(string.Empty);
   }
  };

  recognizer.Canceled += (s, e) =>
  {
   if (e.Reason == CancellationReason.Error)
   {
    tcs.TrySetResult(string.Empty);
   }
  };
  
  await recognizer.StartContinuousRecognitionAsync();
  
  await using (token.Register(() => tcs.TrySetCanceled()))
  {
   try
   {
    var result = await tcs.Task;
    await recognizer.StopContinuousRecognitionAsync();
    return result;
   }
   catch (TaskCanceledException)
   {
    await recognizer.StopContinuousRecognitionAsync();
    return "";
   }
  }
 }

 private static async Task<string> MonitorConsoleInputAsync(CancellationToken token)
 {
  return await Task.Run(() =>
  {
   if (token.IsCancellationRequested) return ""; 
   var input = Console.ReadLine();
   return input ?? ""; 
  });
 }
}
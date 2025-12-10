using Games.Interfaces;
using Games.Minesweeper;
using Games.Wordle;

namespace Games.controllers;

public class AppController : IProgramController
{
    private readonly WordleGame _wordleGame;
    private readonly MinesweeperGame _minesweeperGame;
    private bool _programRunning;
    private bool _gamePlaying;

    public AppController(WordleGame wordle, MinesweeperGame minesweeper)
    {
        _wordleGame = wordle;
        _minesweeperGame = minesweeper;
        _programRunning = true;
        _gamePlaying = true;
    }
    public async Task Run()
    { 
        DisplayOptions();
        var input = Console.ReadLine();
        // Calculator calculator = new ();
        while (_programRunning)
        {
            await HandleMainMenuChoice(input);
            DisplayOptions();
            input = Console.ReadLine();
        }
    }

    private void DisplayOptions()
    {
        Console.WriteLine("Main menu");
        Console.WriteLine("1: Wordle");
        Console.WriteLine("2: Minesweeper");
        Console.WriteLine("8: All Stats");
        Console.WriteLine("9: Quit");
    }
    private async Task HandleMainMenuChoice(string? choice)
    {
        switch (choice)
        {
            case "1":
                await RunGame(_wordleGame);
                break;
            case "2":
                await RunGame(_minesweeperGame);
                break;
            case "8":
                Console.WriteLine("Wordle");
                _wordleGame.Stats();
                Console.WriteLine("----");
                Console.WriteLine("Minesweeper");
                _minesweeperGame.Stats();
                Console.WriteLine("----");
                break;
            case "9":
                _programRunning = false;
                break;
            default:
                Console.WriteLine("Invalid Choice");
                break;
        }
    }

    private async Task RunGame(IGame game)
    {
        _gamePlaying = true;
        while (_gamePlaying)
        {
            Console.Clear();
            Console.WriteLine($"Welcome to {game.Name}");
            Console.WriteLine(" 1: Play");
            Console.WriteLine(" 2: Stats");
            Console.WriteLine(" 9: Quit");
            var choice = Console.ReadLine();
            await EvaluateChoice(choice, game);
        }
    }
    
    private async Task EvaluateChoice(string? choice, IGame game)
    {
        switch (choice)
        {
            case "1":
                await game.Play();
                break;
            case "2":
                game.Stats();
                break;
            case "9":
                _gamePlaying = false;
                Console.Clear();
                break;
            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }
}
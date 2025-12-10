using System.Text.RegularExpressions;
using Games.Interfaces;
using Games.Minesweeper.Components;
using Games.Minesweeper.Exceptions;

namespace Games.Minesweeper;

public class MinesweeperGame : IGame
{
    private Board? _board;
    private int _wins = 0;
    private int _losses = 0;
    public Task Play()
    {
        var height = Int32.Parse(GetInt("Height of the board"));
        int width = Int32.Parse(GetInt("Width of the board"));
        int mines = Int32.Parse(GetInt("Number of mines"));
        _board = new Board(height, width, mines);
        string gameState = Constants.GameState.InProgress;
        DisplayBoard();
        while (gameState == Constants.GameState.InProgress)
        {
            var nextMove = GetMove();
            try
            {
                gameState = _board.PlayMoveRecursive(nextMove.ToArray());
                DisplayBoard();
            }
            catch (InvalidMoveException)
            {
                Console.WriteLine("Invalid move");
            }
        }

        string endGameMessage;
        if (gameState == Constants.GameState.Won)
        {
            _wins++;
            endGameMessage = "Congratulations! You WON!";
        }
        else
        {
            _losses++;
            endGameMessage = "You Lost. Better luck next game!";
        }
        Console.WriteLine(endGameMessage);
        return Task.CompletedTask;
    }

    public void Stats()
    {
        Console.WriteLine($"Played: {_wins + _losses}");
        Console.WriteLine($"Wins: {_wins}, Losses: {_losses}");
    }
    
    string IGame.Name => "Minesweeper";
    
    private string GetInt(string message)
    {
        Console.WriteLine(message);
        string response = Console.ReadLine();
        if (!ValidInput(response, Constants.ValidIntRegex))
        {
            Console.WriteLine("InValid input");
            response = GetInt(message);
        }

        return response;
    }

    private IEnumerable<int> GetMove()
    {
        string move = AskForMove();
        

        string[] coords = move.Split(",");
        var moveCoords = coords.Select(x=>Int32.Parse(x)).Reverse();
        return moveCoords;
    }

    private string AskForMove()
    {
        Console.Write("Next Move: ");
        string move = Console.ReadLine();
        if (!ValidInput(move, Constants.ValidMoveRegex))
        {
            move = AskForMove();
        }

        return move;
    }

    private bool ValidInput(string input, Regex pattern)
    {
        return pattern.Match(input).Success;
    }

    private void DisplayBoard()
    {
        Console.WriteLine(_board?.ToString());
    }
}
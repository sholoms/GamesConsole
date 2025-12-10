using Games.Interfaces;
using Games.Wordle.Services;

namespace Games.Wordle;

public class WordleGame : IGame
{
    private readonly IWordClient _wordClient;
    private int _wins;
    private int _losses;

    public WordleGame(IWordClient wordClient)
    {
        _wordClient = wordClient;
        _wins = 0;
        _losses = 0;
    }
    
    private readonly List<string> _guesses = new();

    private bool GameWon { get; set; }
    private bool GameInProgress { get; set; } = true;
    string IGame.Name => "Wordle";


    public async Task Play()
    {
        var word = await _wordClient.GetWord();
        while (GameInProgress)
        {
            Console.Clear();
            Console.WriteLine("Next guess");
            Console.WriteLine(Guesses());
            AddGuess(word, await GetGuess());
        }

        var endMessage = GameWon ? "Congratulation You win" : $"Game Over \nCorrect word was {word.ToUpper()}";

        Console.Clear();
        Console.WriteLine(Guesses());
        Console.WriteLine("-----------------------");
        Console.WriteLine(endMessage);
    }




    public void Stats()
    {
        Console.WriteLine($"Played: {_wins + _losses}");
        Console.WriteLine($"Wins: {_wins}, Losses: {_losses}");
    }



    private string Guesses()
    {
        return string.Join("\n", _guesses);
    }

    private void AddGuess(string word, string guess)
    {
        if (guess == word)
        {
            _guesses.Add($"{guess} - *****");
            GameWon = true;
            GameInProgress = false;
            _wins++;
        }
        
        var result = new char[] { 'X', 'X', 'X', 'X', 'X' };;
        var guessChars = guess.ToCharArray();
        var remainingWIndexes = new List<int> { 0, 1, 2, 3, 4 };
        var remainingGIndexes = new List<int> { 0, 1, 2, 3, 4 };

        for (var i = 0; i < 5; i++)
        {
            if (guessChars[i] == word[i])
            {
                result[i] = '*';
                remainingGIndexes.Remove(i);
                remainingWIndexes.Remove(i);
            }
        }

        foreach (var gi in remainingGIndexes)
        {
            foreach (var wi in remainingWIndexes)
            {
                if (guess[gi] == word[wi])
                {
                    result[gi] = '?';
                    remainingWIndexes.Remove(wi);
                    break;
                }
            }
        }
        _guesses.Add($"{guess} - {new string(result)}");
        if (_guesses.Count == 5)
        {
            GameWon = false;
            GameInProgress = false;
            _losses++;
        }
    }
    
    



    private async Task<string> GetGuess()
    {
        var input = Console.ReadLine();
        var valid = await _wordClient.ValidateWord(input);
        while (!valid)
        {
            Console.WriteLine("Invalid word, Guess again");
            input = Console.ReadLine();
            valid = await _wordClient.ValidateWord(input);
        }
        
        return input.ToUpper();
    }
    

}
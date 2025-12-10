namespace Games.Wordle.Services;

public interface IFileService
{
    Task<bool> ValidateWord(string word);
    Task WriteFile(List<string> words);
}

public class FileService : IFileService
{
    private readonly string _filePath = "C:/Users/sholo/repos/personal/Wordle/Games/Wordle/Utils/Words.txt";
    
    public async Task<bool> ValidateWord(string word)
    {
        using var reader = new StreamReader(_filePath);
        var words = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line?.ToLower() == word.ToLower())
            {
                return true;
            }
        }
        return false;
    }
    
    public async Task WriteFile(List<string> words)
    {
        await using var writer = new StreamWriter(_filePath, append: true);
        foreach (var word in words)
        {
            await writer.WriteLineAsync(word);
        }
    }
}
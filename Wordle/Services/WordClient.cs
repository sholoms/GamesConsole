namespace Games.Wordle.Services;

public interface IWordClient
{
    Task<string> GetWord();
    Task<bool> ValidateWord(string word);
}

public class WordClient : IWordClient
{
    private readonly IFileService _fileService;
    private readonly HttpClient _client = new();

    public WordClient(IFileService fileService)
    {
        _fileService = fileService;
    }
    public async Task<string> GetWord()
    {
        string result = "";
        try
        {
            using HttpResponseMessage response = await _client.GetAsync("https://random-word-api.vercel.app/api?words=1&length=5");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            result = responseBody[2..7];
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
        return result.ToUpper();

    }

    public async Task<bool> ValidateWord(string word)
    {
        return await _fileService.ValidateWord(word);
    }

    public async Task GetAllWords()
    {
        try
        {
            using HttpResponseMessage response = await _client.GetAsync("https://random-word-api.herokuapp.com/all");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var words = responseBody[2..^2].Split("\",\"").ToList();
            await _fileService.WriteFile(words.Where(word => word.Length == 5).ToList());
            Console.WriteLine("finished");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
}
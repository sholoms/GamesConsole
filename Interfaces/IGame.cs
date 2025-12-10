namespace Games.Interfaces;

public interface IGame
{
    Task Play();
    void Stats();
    string Name { get; }
    
}
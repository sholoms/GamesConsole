namespace Games.Minesweeper.Components;

public class Cell
{
    public readonly bool IsMine;
    private bool Revealed { get; set;  }
    private int Value { get; set; }

    public Cell(bool isMine)
    {
        IsMine = isMine;
        Value = 0;
        Revealed = false;
    }

    public override string ToString()
    {
        if (Revealed)
        {
            return IsMine ? "X" : Value.ToString();
        }
        return " ";
    }

    public void Increment()
    {
        if (!IsMine)
        {
            Value++; 
        }
    }

    public bool Reveal()
    {
        if (Revealed)
            return false;
        
        Revealed = true;
        return true;
    }
    
    public bool IsEmpty()
    {
        return Value == 0;
    }
}
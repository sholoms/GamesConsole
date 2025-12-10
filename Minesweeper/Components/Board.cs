using System.Text;
using Games.Minesweeper.Exceptions;

namespace Games.Minesweeper.Components;

public class Board
{
    private readonly int _rows;
    private readonly int _columns;
    private readonly int _mines;
    private readonly Cell[,] _board;
    private readonly int[][] _mineCoords;
    private int _remainingCells;
    public int times = 0;

    public Board(int rows, int columns, int mines)
    {
        _rows = rows;
        _columns = columns;
        _mines = mines;
        _remainingCells = rows * columns;
        _board = new Cell[rows, columns];
        _mineCoords = new int[_mines][];
        GenerateBoard();
        _remainingCells -= _mines;
    }

    public string PlayMoveIterative(int[] coords)
    {
        if (coords[0] > _rows || coords[1] > _columns|| coords[0] < 1 || coords[1] < 1 )
            throw new InvalidMoveException();
        
        string gameState;
        Cell cell = _board[coords[0] - 1, coords[1] - 1];
        cell.Reveal();

        if (cell.IsMine)
        {
            gameState = Constants.GameState.Lost;
            RevealMines();
        }
        else
        {
            if (cell.IsEmpty())
            {
                RevealSurroundingCellsIterative(coords);
            }
            _remainingCells -= 1;
            gameState = _remainingCells == 0 ? Constants.GameState.Won : Constants.GameState.InProgress;
        }
        return gameState;
    }

    private void RevealSurroundingCellsIterative(int[] coords)
    {
        var cellQueue = new Queue<int[]>();
        foreach (var surroundingCell in GetSurroundingCells(coords))
        {
            cellQueue.Enqueue(surroundingCell);
        }
        
        while (cellQueue.Any())
        {
            times += 1;
            var cellCoords = cellQueue.Dequeue();
            var cell = _board[cellCoords[0], cellCoords[1]];
            var revealedNow = cell.Reveal();
            if (revealedNow)
            {
                _remainingCells--;
                if (cell.IsEmpty())
                {
                    var surroundingCells = GetSurroundingCells(cellCoords);
                    foreach (var surroundingCell in surroundingCells)
                    {
                        cellQueue.Enqueue(surroundingCell);
                    }
                }
            }
        }
    }

    
    public string PlayMoveRecursive(int[] coords)
    {
        if (coords[0] > _rows || coords[1] > _columns|| coords[0] < 1 || coords[1] < 1 )
            throw new InvalidMoveException();
        
        string gameState;
        Cell cell = _board[coords[0] - 1, coords[1] - 1];
        cell.Reveal();

        if (cell.IsMine)
        {
            gameState = Constants.GameState.Lost;
            RevealMines();
        }
        else
        {
            if (cell.IsEmpty())
            {
                RevealSurroundingCellsRecursive(coords);
            }
            _remainingCells -= 1;
            gameState = _remainingCells == 0 ? Constants.GameState.Won : Constants.GameState.InProgress;
        }
        return gameState;
    }
    private void RevealSurroundingCellsRecursive(int[] coords)
    {
        times += 1;
        var cell = _board[coords[0], coords[1]];
        var revealedNow = cell.Reveal();
        if (revealedNow)
        {
            _remainingCells--;
            if (cell.IsEmpty())
            {
                var surroundingCells = GetSurroundingCells(coords);
                foreach (var surroundingCell in surroundingCells)
                {
                    RevealSurroundingCellsRecursive(surroundingCell);
                }
            }
        }

    }

    private void RevealMines()
    {
        foreach (var mine in _mineCoords)
        {
            _board[mine[0],mine[1]].Reveal();
        }
    }

    
    
    
    private void GenerateBoard()
    {
        Cell cell;
        int remainingMines = _mines;
        int remainingCells = _remainingCells;
        for (int row = 0; row < _rows; row++)
        {
            for (int column = 0; column < _columns; column++)
            {
                cell = GenerateCell(remainingCells, remainingMines);
                if (cell.IsMine)
                {
                    remainingMines--;
                    _mineCoords[remainingMines] = new[] {row, column};
                }

                _board[row, column] = cell;
                remainingCells--;
            }
        }
        IncrementCells(_mineCoords);
    }

    private List<int[]> GetSurroundingCells(int[] cell)
    {
        List<int[]> cellCoords = new List<int[]>();
        var startRow = cell[0] - 1;
        var endRow = cell[0] + 1;
        var startCol = cell[1] - 1;
        var endCol = cell[1] + 1;
        if (startCol < 0)
        {
            startCol = 0;
        }
        if (startRow < 0)
        {
            startRow = 0;
        }
        if (endCol >= _columns)
        {
            endCol = _columns - 1;
        }
        if (endRow >= _rows)
        {
            endRow = _rows -1;
        }

        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                cellCoords.Add(new[] {row, col});
            }
        }

        return cellCoords;
    }
    
    
    
    
    private void IncrementCells(int[][] mineCoords)
    {
        foreach (var mine in mineCoords)
        {
            var cells = GetSurroundingCells(mine);
            foreach (var cell in cells)
            {
                if(cell != null)
                    _board[cell[0], cell[1]].Increment();
            }
            
        }
    }

    private Cell GenerateCell(int remainingCells, int remainingMines)
    {
        Cell cell;
        if (remainingMines == 0)
        {
            cell = new Cell(false);
        }
        else
        {
            Random randomGenerator = new Random();
            int random = randomGenerator.Next(remainingCells);
            cell = random < remainingMines ? new Cell(true) : new Cell(false);
        }
        
        return cell;
    }

    public override string ToString()
    {
        var boardBuilder = new StringBuilder();
        boardBuilder.Append("  ");
        for (int column = 0; column < _columns; column++)
        {
            boardBuilder.Append($"     {column + 1}");
        }
        boardBuilder.AppendLine();
        boardBuilder.Append("--");
        for (int column = 0; column < _columns; column++)
        {
            boardBuilder.Append("------");
        }
        boardBuilder.AppendLine();
        for (int row = 0; row < _rows; row++)
        {
            boardBuilder.Append($"{row + 1} ");
            for (int column = 0; column < _columns; column++)
            {
                boardBuilder.Append($"  |  {_board[row, column]}");
            }
            boardBuilder.AppendLine();
        }

        return boardBuilder.ToString();
    }
}
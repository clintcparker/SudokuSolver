namespace sudoku_core;

public class Cell
{
    public Board Board { get; private set; }
    public CellCollection Row { get; private set; }
    public CellCollection Column { get; private set; }
    public CellCollection Box { get; private set; }
    public int? Value { get; private set; }
    public List<int> PossibleValues { get; private set; }
    public int Index { get; private set; }
    public Cell(Board board, int index, int? value = null)
    {
        Board = board;
        Index = index;
        Row = getRow();
        Column = getColumn();
        Box = getBox();
        PossibleValues = Enumerable.Range(1, 9).ToList();
        if (value != null)
        {
            SetValue(value.Value);
        }
    }

    public void SetValue(int value)
    {
        if(Value == null && Board.debug)
        {
            Console.WriteLine($"Setting cell {Index} to {value}");
        }
        Value = value;
        PossibleValues.Remove(value);
        if (this.IsValid())
        {
            Row.CantBe(value);
            Column.CantBe(value);
            Box.CantBe(value);
            PossibleValues.Clear();
        } else
        {
            Value = null;
            PossibleValues.Add(value);
        }
    }

    public void SetInitialValue(int value)
    {
        Value = value;
    }

    public bool IsValid()
    {
        if (Value == null)
        {
            return true;
        }
        var rowIsValid = this.Row.IsValid();
        var columnIsValid = this.Column.IsValid();
        var boxIsValid =  this.Box.IsValid();

        return rowIsValid && columnIsValid && boxIsValid;
    }

    public void CantBe(int value)
    {
        if (Value == null)
        {
            if (Board.debug)
            {
                if (PossibleValues.Contains(value))
                {
                    Console.WriteLine($"Cell {Index} can't be {value}");
                }
            }
            PossibleValues.Remove(value);
            if (PossibleValues.Count == 1)
            {
                if (Board.debug)
                {
                    Console.WriteLine($"Cell {Index} can only be {PossibleValues[0]}");
                }
                SetValue(PossibleValues[0]);
            }
        }
    }

    private CellCollection getRow()
    {
        return Board.Rows[(Index - 1) / 9];
    }

    private CellCollection getColumn()
    {
        return Board.Columns[(Index - 1) % 9];
    }

    private CellCollection getBox()
    {
        return Board.Boxes[((Index - 1) / 3) % 3 + ((Index - 1) / 27) * 3];
    }
    
}


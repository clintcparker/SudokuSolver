namespace sudoku_core;

public class CellCollection : List<Cell>
{
    public Board Board { get; }
    public string CollectionType { get; }
    public int Index { get; }

    public CellCollection(Board board, string type, int i)
    {
        Board = board;
        CollectionType = type;
        Index = i;
    }

    public bool IsValid()
    {
        //the collection is valid if all the values are unique, and there are only 9 values and all the values are between 1 and 9
        var values = this.Where(c => c.Value != null).Select(c => c.Value.Value).ToList(); //all the non-null values
        var valuesAreDistinct = values.Count == values.Distinct().Count(); // the values we have are distinct
        var valuesAreValid = values.All(v => v >= 1 && v <= 9); //values are between 1 and 9
        var countIsNine = this.Count == 9;  //we 9 cells in the collection
        return valuesAreDistinct && valuesAreValid && countIsNine;
    }

    public void CantBe(int value)
    {
        foreach (var cell in this)
        {
            cell.CantBe(value);
        }
    }

    public void Solve()
    {
        SolveCellWithOnlyPossibleValue();
        FindPairsAndReduce();
        ColumnAndRowReduce();
    }

    //get the cell that contains the only instance of a possible value in the whole collection
    public void SolveCellWithOnlyPossibleValue()
    {
        for (int i = 1; i <=9; i++)
        {
            var cellsWithPossibleValue = this.Where(c => c.PossibleValues.Contains(i)).ToList();
            if (cellsWithPossibleValue.Count == 1)
            {
                if (Board.debug)
                {
                    Console.WriteLine($"Cell {cellsWithPossibleValue[0].Index} is the only {i} in {CollectionType}-{Index}");
                }
                cellsWithPossibleValue[0].SetValue(i);
            }
        }
    }

    public void FindPairsAndReduce()
    {
        var cellsWithTwoPossible = this.Where(c => c.PossibleValues.Count == 2);
        foreach (var cell in cellsWithTwoPossible)
        {
            var possibleValue1 = cell.PossibleValues[0];
            var possibleValue2 = cell.PossibleValues[1];
            var IndexesCellsWithJustTheseTwoPossible = this.Where(c => c.PossibleValues.Count == 2 && c.PossibleValues[0] == possibleValue1 && c.PossibleValues[1] == possibleValue2).ToList();
            if (IndexesCellsWithJustTheseTwoPossible.Count() == 2)
            {
                if (Board.debug)
                {
                    Console.WriteLine($"Found pair {{{possibleValue1},{possibleValue2}}} in {CollectionType}-{Index}");
                }
                foreach (var c in this.Where(c => c != IndexesCellsWithJustTheseTwoPossible.First() && c != IndexesCellsWithJustTheseTwoPossible.Last()))
                {
                    c.CantBe(possibleValue1);
                    c.CantBe(possibleValue2);
                }
            }
        }
    }

    public void ColumnAndRowReduce()
    {
        //find all the cells in the box collection that contain the number 1 as a possible value
        if (CollectionType == "box")
        {
            for (int i = 1; i <=9; i++)
            {
                var columns = this.Where(c => c.PossibleValues.Contains(i)).Select(c => c.Column).Distinct();
                var rows = this.Where(c => c.PossibleValues.Contains(i)).Select(c => c.Row).Distinct();
                if (columns.Count() == 1)
                {
                    columns.First().ValueMustBeInBox(i, this);
                }
                if (rows.Count() == 1)
                {
                    rows.First().ValueMustBeInBox(i, this);
                }
            }
        }
    }

    private void ValueMustBeInBox(int value, CellCollection box)
    {
        if (Board.debug)
        {
            Console.WriteLine($"{this.CollectionType}-{this.Index} {value} must be in box-{box.Index}");
        }
        foreach (var cell in this.Where(c => c.Box != box))
        {
            cell.CantBe(value);
        }
    }
}


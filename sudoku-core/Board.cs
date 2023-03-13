using System.Text;
using System.Runtime.CompilerServices;
//Show this off to our friends in the unit tests
[assembly:InternalsVisibleTo("sudoku-tests")]

namespace sudoku_core;
public class Board
{
    internal bool debug = false;
    private static HttpClient _httpClient = new HttpClient();
    public List<Cell> Cells { get; set; } = new List<Cell>();
    public List<CellCollection> Rows { get; set; } = new List<CellCollection>();
    public List<CellCollection> Columns { get; set; } = new List<CellCollection>();
    public List<CellCollection> Boxes { get; set; } = new List<CellCollection>();
    public Board()
    {
        for (int i = 1; i <= 9; i++)
        {
            Rows.Add(new CellCollection(this, "row",i));
            Columns.Add(new CellCollection(this, "column",i));
            Boxes.Add(new CellCollection(this, "box",i));
        }
        for (int i = 1; i <= 81; i++)
        {
            var cell = new Cell(this, i);
            Cells.Add(cell);
            cell.Row.Add(cell);
            cell.Column.Add(cell);
            cell.Box.Add(cell);
        }
    }



    //example url https://www.websudoku.com/?level=1&set_id=1234567890
    //get the table element table#puzzle_grid
    //parse the html table to a board
    public void ParseUrl(string boardUrl)
    {

        var html = _httpClient.GetStringAsync(boardUrl).Result;
        LoadFromHtmlString(html);

    }

    public void Parse(string boardString)
    {
        /* Example
        904100000
        200000400
        000502090
        080060300
        040207080
        001090070
        060305000
        008000003
        000004502
        */
        var lines = boardString.Split(Environment.NewLine);
        var index = 1;
        foreach (var line in lines)
        {
            if (String.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            if (line.Length != 9)
            {
                throw new Exception($"Invalid board string: {line}");
            }
            foreach (var c in line)
            {
                var intVal = int.Parse(c.ToString(), System.Globalization.NumberStyles.Integer);
                if (intVal != 0)
                {
                    this.Cells[index-1].SetInitialValue(intVal);
                }
                index++;
            }
        }
    }

    internal void LoadFromHtmlString(string html)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        var table = doc.DocumentNode.Descendants("table").Where(x => x.Id == "puzzle_grid").First();
        var rows = table.SelectNodes("tr");
        var index = 1;
        if (debug)
            Console.WriteLine("Loading board from html");
        for (int i = 0; i < 9; i++)
        {
            var cells = rows[i].SelectNodes("td");
            for (int j = 0; j < 9; j++)
            {
                var cell = cells[j].ChildNodes[0];
                var value = cell.GetAttributeValue("value", null);
                int intVal = 0;
                if (!String.IsNullOrWhiteSpace(value))
                {   
                    intVal = int.Parse(value, System.Globalization.NumberStyles.Integer);
                    this.Cells[index-1].SetInitialValue(intVal);
                }
                if (debug)
                    Console.Write($"{intVal}");  
                index++;
            }
            if (debug)
                Console.Write(Environment.NewLine);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                var cell = this.Cells[i * 9 + j];
                var intVal = 0;
                if (cell.Value.HasValue)
                {
                    intVal = cell.Value.Value;
                }
                sb.Append(intVal);
            }
            sb.Append(Environment.NewLine);
        }
        return sb.ToString();
    }

    public bool IsValid()
    {
        var rowsAreValid = this.Rows.All(x => x.IsValid());
        var columnsAreValid = this.Columns.All(x => x.IsValid());
        var boxesAreValid = this.Boxes.All(x => x.IsValid());
        return rowsAreValid && columnsAreValid && boxesAreValid;
    }

    public bool IsSolved()
    {
        var IsValid = this.IsValid();
        var allCellsHaveValue = this.Cells.All(x => x.Value.HasValue);
        return IsValid && allCellsHaveValue;
    }
    public void Solve()
    {
        if (debug)
            Console.WriteLine("Solver Round 1");
        Cells.ForEach(x => {
            if (x.Value.HasValue)
            {
                x.SetValue(x.Value.Value);
            }
        });
        var n = 1;
        for (int i = 0; i < 3; i++)
        {
            var lastRound = "";
            while (!IsSolved() && lastRound != this.ToString())
            {
                n++;
                if (debug)
                    Console.WriteLine($"Solver Round {n}");
                lastRound = this.ToString();
                Rows.ForEach(x => x.Solve());
                Columns.ForEach(x => x.Solve());
                Boxes.ForEach(x => x.Solve());
            }
        }
        
        if (debug)
            Console.WriteLine($"Stopped in {n} rounds");
        if (debug)
            Console.WriteLine($"IsSolved: {IsSolved()}");
    }

    public Cell Cell(int index)
    {
        return this.Cells[index-1];
    }
}
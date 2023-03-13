namespace sudoku_tests;

[TestClass]
public class BoardTests
{
    [TestMethod]
    public void CollectionValidTests()
    {
        var b = new Board();
        for (int i = 0; i < 9; i++)
        {
            b.Cells[i].SetValue(i + 1);
        }
        Assert.IsTrue(b.Rows[0].IsValid());
    }

    [TestMethod]
    public void BoardSetup()
    {
        var board = new Board();
        Assert.AreEqual(81, board.Cells.Count);
        Assert.AreEqual(9, board.Rows.Count);
        Assert.AreEqual(9, board.Columns.Count);
        Assert.AreEqual(9, board.Boxes.Count);
        Assert.AreEqual(3, board.Cell(18).Box.Index);
        Assert.AreEqual(9, board.Cell(18).Column.Index);
        Assert.AreEqual(2, board.Cell(18).Row.Index);
        Assert.AreEqual(4, board.Cell(28).Box.Index);
    }

    [TestMethod]
    public void ParseBoard()
    {
        var board = new Board();
        // board.ParseUrl("http://five.websudoku.com/?level=1&set_id=4216331824");
        var html = System.IO.File.ReadAllText("4216331824-easy-html.txt");
        board.LoadFromHtmlString(html);
        Console.WriteLine(board.ToString());
        Assert.AreEqual(81, board.Cells.Count);
        Assert.AreEqual(1, board.Cells.First(c => c.Index == 1).Value);
        Assert.IsTrue(board.IsValid());
    }

    [TestMethod]
    public void ParseEvilBoard()
    {
        var board = new Board();
        // board.ParseUrl("http://five.websudoku.com/?level=1&set_id=4216331824");
        var html = System.IO.File.ReadAllText("807089539-evil-html.txt");
        board.LoadFromHtmlString(html);
        Console.WriteLine(board.ToString());
        Assert.AreEqual(81, board.Cells.Count);
        Assert.IsTrue(board.IsValid());
    }

    [TestMethod]
    public void SolveEasyBoard()
    {
        var board = new Board();
        // board.ParseUrl("http://five.websudoku.com/?level=1&set_id=4216331824");
        var html = System.IO.File.ReadAllText("4216331824-easy-html.txt");
        board.LoadFromHtmlString(html);
        Console.WriteLine(board.ToString());
        Assert.AreEqual(81, board.Cells.Count);
        Assert.AreEqual(1, board.Cells.First(c => c.Index == 1).Value);
        Assert.IsTrue(board.IsValid());
        board.Solve();
        Assert.IsTrue(board.IsValid());
        Assert.AreEqual(0,board.Cells.Count(c=>c.Value==null));
    }

    [TestMethod]
    public void SolveEvilBoard()
    {
        var board = new Board();
        board.debug = true;
        // board.ParseUrl("http://five.websudoku.com/?level=1&set_id=4216331824");
        var html = System.IO.File.ReadAllText("807089539-evil-html.txt");
        board.LoadFromHtmlString(html);
        //Console.WriteLine(board.ToString());
        Assert.AreEqual(81, board.Cells.Count);
        Assert.IsTrue(board.IsValid());
        board.Solve();
        Assert.IsTrue(board.IsValid());
        Assert.AreEqual(0,board.Cells.Count(c=>c.Value==null));
        //Console.WriteLine(board.ToString());
        // foreach (var cell in board.Cells)
        // {
        //         Console.WriteLine($"{cell.Index}\t:{cell.Value}\t: {string.Join(",", cell.PossibleValues)}");
        // }
        Assert.IsTrue(board.IsSolved());
    }
}